using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "AudioManager";
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<AudioManager>();
                }

                return instance;
            }
        }

        /// <summary>
        /// 是否能够找到指定的音效
        /// </summary>
        /// <param name="audioStr"></param>
        /// <returns></returns>
        public bool ContainAudio(string audioStr)
        {
            AudioClip clip = Resources.Load<AudioClip>(audioStr);
            return clip != null;
        }

        /// <summary>
        /// 播放指定音乐
        /// </summary>
        /// <param name="audioStr"></param>
        /// <param name="isLoop"></param>
        public void Play(string audioStr, bool isLoop = false)
        {
            SingleClip clip = AudioClipManager.Instance.GetClip(audioStr);
            AudioSource source = AudioSourceManager.Instance.GetIdleAudioSource();
            clip?.Play(source, isLoop);
        }

        /// <summary>
        /// 停止指定音乐
        /// </summary>
        /// <param name="audioStr"></param>
        public void Stop(string audioStr)
        {
            AudioSourceManager.Instance.Stop(audioStr);
        }


        /// <summary>
        /// 返回指定音乐所在 AudioSource -> 可以做进一步调整
        /// </summary>
        /// <param name="audioStr"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        public AudioSource Get(string audioStr, bool isLoop = false)
        {
            SingleClip clip = AudioClipManager.Instance.GetClip(audioStr);
            AudioSource source = AudioSourceManager.Instance.GetIdleAudioSource();
            clip?.Set(source);
            return source;
        }
    }
}