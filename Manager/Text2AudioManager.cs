using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;
using Util.Http;
// using SpeechLib;

namespace Manager
{
    public class Text2AudioManager : MonoBehaviour
    {
        public static Text2AudioManager Instance;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        // SpVoice voice;

        private void Start()
        {
           // GetAudioFromText("啦啦啦你好啊，我是派蒙，这是测试");

            // // 设置 Windows内置SpVoice
            // //实例化 SpVoice 对象
            // voice = new SpVoice();
            // // //管理语音属性
            // voice.Voice = voice.GetVoices(string.Empty, string.Empty).Item(0);
            // //语音速度，范围-10到10，默认是0
            // voice.Rate = 0;
            // //语音音量，范围0到100，默认是100
            // voice.Volume = 100;
        }


        // /// <summary>
        // /// 停止播放
        // /// </summary>
        // public void Stop()
        // {
        //     voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        // }
        
        // /// <summary>
        // /// 文本转语音播放
        // /// </summary>
        // /// <param name="content"></param>
        // public void Speak(string content)
        // {
        //     //异步朗读
        //     voice.Speak(content, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        // }


        #region Useless

        const int HEADER_SIZE = 44;
        /// <summary>
        /// 通过文本获得语音
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerator GetAudioFromText(string text)
        {
            Debug.Log(Application.dataPath);
            string audioStr = GuideAudio.sTextAddr(text);
            Debug.Log(audioStr);
            if (File.Exists(audioStr + ".wav.meta"))
            {
                Debug.Log("Contain!");
                AudioManager.Instance.Play(GuideAudio.sText(text));
                yield break;
            }


            string url = $"{HttpConfig.httpARMuseumText2Audio}?text={text}";
            HttpDownload download = new HttpDownload();
            DownloadConfig down = new DownloadConfig(url, GuideAudio.sTextAddr(text));
            StartCoroutine(download.DownLoadFileByUrl(down));
            // StartCoroutine(LoadAudio(audioStr + ".wav"));
            // StartCoroutine(SpeakText(GuideAudio.sText(text)));
        }

        public IEnumerator SpeakText(string text)
        {
            string audioStr = GuideAudio.sTextAddr(text);
            while (!File.Exists(audioStr + ".wav.meta"))
            {
                yield return null;
            }

            AudioManager.Instance.Play(GuideAudio.sText(text));
        }


        /// <summary>
        /// 读取音频
        /// </summary>
        /// <param name="audioPath"></param>
        /// <returns></returns>
        IEnumerator LoadAudio(string audioPath)
        {
            Debug.Log(audioPath);
            WWW www = new WWW(audioPath);
            yield return www;
            var clip = www.GetAudioClip(false, true, AudioType.WAV);
            AudioSource audioSource = AudioSourceManager.Instance.GetIdleAudioSource();
            audioSource.clip = clip;
            audioSource.Play();
        }
        
        
        /// <summary>
        /// 保存音频
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        IEnumerator SaveAudio(string fileName, AudioClip clip)
        {
            if (!fileName.ToLower().EndsWith(".wav"))
            {
                fileName += ".wav";
            }

            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);
            Debug.Log(filePath);
            //创建头
            FileStream fs = CreateEmpty(filePath);
            //写语音数据
            ConvertAndWrite(fs, clip);
            //重写真正的文件头
            WriteHeader(fs, clip);
            fs.Flush();
            fs.Close();

            yield break;
        }

        /// <summary>
        /// 创建头
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        FileStream CreateEmpty(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Create);
            byte emptyByte = new byte();
            for (int i = 0; i < HEADER_SIZE; i++)
            {
                fileStream.WriteByte(emptyByte);
            }

            return fileStream;
        }

        /// <summary>
        /// 写音频数据
        /// </summary>
        /// <param name="fileSteam"></param>
        /// <param name="clip"></param>
        void ConvertAndWrite(FileStream fileSteam, AudioClip clip)
        {
            var samples = new float[clip.samples];
            clip.GetData(samples, 0);
            Int16[] intData = new Int16[samples.Length];
            Byte[] bytesData = new Byte[samples.Length * 2];
            int rescaleFactor = 32767;
            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                Byte[] byteArray = new byte[2];
                byteArray = BitConverter.GetBytes(intData[i]);
                byteArray.CopyTo(bytesData, i * 2);
            }

            fileSteam.Write(bytesData, 0, bytesData.Length);
        }

        /// <summary>
        /// 重写真正的文件头
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="clip"></param>
        void WriteHeader(FileStream fileStream, AudioClip clip)
        {
            var hz = clip.frequency;
            var channels = clip.channels;
            var samples = clip.samples;
            fileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
            fileStream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fileStream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            fileStream.Write(subChunk1, 0, 4);

            Byte[] audioFormat = BitConverter.GetBytes(1);
            fileStream.Write(audioFormat, 0, 2);


            Byte[] numChannels = BitConverter.GetBytes(channels);
            fileStream.Write(numChannels, 0, 2);
            Byte[] sampleRate = BitConverter.GetBytes(hz);
            fileStream.Write(sampleRate, 0, 4);
            Byte[] byRate = BitConverter.GetBytes(hz * channels * 2);
            fileStream.Write(byRate, 0, 4);

            UInt16 blockAlign = (ushort)(channels * 2);
            fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            fileStream.Write(bitsPerSample, 0, 2);

            Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
            fileStream.Write(dataString, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
            fileStream.Write(subChunk2, 0, 4);
        }
        #endregion
        
    }

    #region Useless2
    public class DownloadConfig
    {
        public string url;
        public string path;

        public DownloadConfig(string URL, string PATH)
        {
            url = URL;
            path = PATH;
        }
    }

    class HttpDownload
    {
        public IEnumerator DownLoadFileByUrl(object down)
        {
            string tempPath = Path.GetDirectoryName((down as DownloadConfig)?.path);
            string tempFile = tempPath + @"\" + Path.GetFileName((down as DownloadConfig)?.path) + ".wav"; //临时文件
            
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile); //存在则删除
            }


            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            // 设置参数
            var requestUriString = (down as DownloadConfig)?.url;

            HttpWebRequest request = WebRequest.Create(requestUriString) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            while (response == null)
            {
                response = request.GetResponse() as HttpWebResponse;
                yield return null;
            }

            if (response != null)
            {
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                byte[] bArr = new byte[4096 * 5];
                if (responseStream != null)
                {
                    int size = responseStream.Read(bArr, 0, bArr.Length);
                    while (size > 0)
                    {
                        fs.Write(bArr, 0, size);
                        size = responseStream.Read(bArr, 0, bArr.Length);
                    }

                    BytesToAudioClip(bArr);
                }

                responseStream?.Close();
            }
            fs.Close();
        }
        
        /// <summary>
        /// byte转AudioClip进行播放
        /// </summary>
        /// <param name="data"></param>
        public void BytesToAudioClip(byte[] data)
        {
            float[] clipData = BytesToFloat(data); //进行数据转换。将byte转成AudioClip可读取的float
            AudioSource audioSource = AudioSourceManager.Instance.GetIdleAudioSource();
            audioSource.clip = AudioClip.Create("audioClip", 16000 * 10, 1, 16000, false); //不进行这一步，audioSource.clip会报空！
            audioSource.clip.SetData(clipData, 0); //完成赋值
            audioSource.Play();
        }

        public static float[] BytesToFloat(byte[] byteArray) //byte[]数组转化为AudioClip可读取的float[]类型
        {
            float[] sounddata = new float[byteArray.Length / 2];
            for (int i = 0; i < sounddata.Length; i++)
            {
                sounddata[i] = BytesToFloat(byteArray[i * 2], byteArray[i * 2 + 1]);
            }
            return sounddata;
        }

        static float BytesToFloat(byte firstByte, byte secondByte)
        {
            //小端和大端顺序要调整
            short s;
            if (BitConverter.IsLittleEndian)
                s = (short)((secondByte << 8) | firstByte);
            else
                s = (short)((firstByte << 8) | secondByte);
            return s / 32768.0F;
        }
    }
    

    #endregion
}