using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Manager
{


    public struct GuideAudio
    {

        public static string sInfo => $"Audio/Guide/{GuideBehavior.Instance.mState.ToString()}/info";

        public static string sFinishTakePhoto => "Audio/Guide/Others/FinishTakePhoto";

        public static string sRespond => "Audio/Guide/Others/Respond";
        public static string sYes => "Audio/Guide/Others/Yes";

        public static string sTextAddr(string text)
        {
            return Application.dataPath + $"/Resources/Audio/Guide/Text/{text}";
        }

        public static string sText(string text)
        {
            return $"Audio/Guide/Text/{text}";
        }

        public static string sCountDown(int cnt)
        {
            return $"Audio/Guide/CountDown/{cnt}";
        }

        public static string sTouchHead
        {
            get
            {
                int random = Random.Range(1, 4);
                string str = $"Audio/Guide/{GuideBehavior.Instance.mState.ToString()}/Head{random}";
                return str;
            }
        }
        
        
        public static string sTouchHand
        {
            get
            {
                int random = Random.Range(1, 4);
                string str = $"Audio/Guide/{GuideBehavior.Instance.mState.ToString()}/Hand{random}";
                return str;
            }
        }
        
        public static string sTouchFoot
        {
            get
            {
                int random = Random.Range(1, 4);
                string str = $"Audio/Guide/{GuideBehavior.Instance.mState.ToString()}/Foot{random}";
                return str;
            }
        }

    }

    public struct GameAudio
    {
        public static string sButtonPressed=$"Audio/Guide/GAME/MRTK_ButtonPress";
        public static string sGameEnd=$"Audio/Guide/GAME/GameEnd";
    }
    

    public class SingleClip
    {
        AudioClip clip;
 
        public SingleClip(AudioClip clip) {
            this.clip = clip;
        }
 
        public void Play(AudioSource audioSource,bool isLoop=false) {
            audioSource.clip = clip;
            audioSource.loop = isLoop;
            audioSource.Play();
        }

        public void Set(AudioSource audioSource, bool isLoop = false)
        {
            audioSource.clip = clip;
            audioSource.loop = isLoop;
        }
    }

    public class AudioClipManager : MonoBehaviour
    {
        private static AudioClipManager instance;
        
        
        private Dictionary<string, SingleClip> clipPool = new Dictionary<string, SingleClip>();

        
        public static AudioClipManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "AudioClipManager";
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<AudioClipManager>();
                }

                return instance;
            }
        }
        
        

        public SingleClip GetClip(string clipStr)
        {
            if (clipPool.ContainsKey(clipStr))
            {
                return clipPool[clipStr];
            }

            AudioClip clip = Resources.Load<AudioClip>(clipStr);
            SingleClip singleClip = new SingleClip(clip);
            clipPool.Add(clipStr, singleClip);
            return singleClip;
        }

        public void Clear()
        {
            clipPool.Clear();
        }
        
    }
}