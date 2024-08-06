using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class AudioSourceManager : MonoBehaviour
    {
        private List<AudioSource> sourceList = new List<AudioSource>();


        private static AudioSourceManager instance;
        

        public static AudioSourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "AudioSourceManager";
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<AudioSourceManager>();
                    for (int i = 0; i < 2; i++)
                    {
                        AudioSource item = obj.AddComponent<AudioSource>();
                        instance.sourceList.Add(item);
                    }
                }
                return instance;
            }
        }
        
        
        /// <summary>
        /// 获得闲置的 AudioSource
        /// </summary>
        /// <returns></returns>
        public AudioSource GetIdleAudioSource() {
            for (int i = 0; i < Instance.sourceList.Count; i++)
            {
                if (Instance.sourceList[i].isPlaying == false)
                {
                    return Instance.sourceList[i];
                }
            }
 
            AudioSource item = gameObject.AddComponent<AudioSource>();
            Instance.sourceList.Add(item);
            
            return item;
        }


        /// <summary>
        /// 暂停播放音乐
        /// </summary>
        /// <param name="audioStr"></param>
        public void Stop(string audioStr)
        {
            if (string.IsNullOrEmpty(audioStr))
            {
                return;
            }

            foreach (var audio in Instance.sourceList)
            {
                if (audio.isPlaying && audioStr.Contains(audio.clip.name))
                {
                    audio.Stop();
                }
            }
        }

    }
}