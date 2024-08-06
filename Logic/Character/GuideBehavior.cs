using System.Collections;
using Manager;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using Util;

namespace Character
{
    public class GuideBehavior : MonoBehaviour
    {
        public static GuideBehavior Instance;

        public GuideState mState; // 向导状态机


        private void Awake()
        {
            Instance = this;
            mState = GuideState.IDLE;
        }


        // 向导运动相关
        public float amplitude = 0.0003f; // 幅度
        public float frequency = 1f; // 频率


        // 向导触摸倒计时器
        private TimeCountDown mAudioCountDown = new TimeCountDown(2f);
        private bool _bInPlayAudio;


        // 向导照相倒计时器
        private TimeCountDown mTakePhotoCountDown = new TimeCountDown(1f);
        private bool _bInTakingPhoto;

        // 向导语音触发倒计时器
        private TimeCountDown mCallingCountDown = new TimeCountDown(3f);
        private bool _bIsCallingActive;
        public bool CallActive => _bIsCallingActive; // 调用，知道向导是否激活


        // 向导动画状态机
        public Animator aAnimator;


        private void Update()
        {
            UpdateMovement();

            if (_bInPlayAudio)
            {
                mAudioCountDown.Tick(TimeManager.DeltaTime);
                if (mAudioCountDown.TimeOut)
                {
                    _bInPlayAudio = false;
                    mAudioCountDown.FillTime();
                }
            }

            if (_bIsCallingActive)
            {
                mCallingCountDown.Tick(TimeManager.DeltaTime);
                if (mCallingCountDown.TimeOut)
                {
                    _bIsCallingActive = false;
                    mCallingCountDown.FillTime();
                }
            }
        }

        /// <summary>
        /// 开始倒计时 -> 协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartCountDown()
        {
            _bInTakingPhoto = true;
            // 倒计时 3s
            int cnt = 3;
            AudioManager.Instance.Play(GuideAudio.sCountDown(cnt));
            while (cnt >= 0)
            {
                mTakePhotoCountDown.Tick(TimeManager.DeltaTime);
                if (mTakePhotoCountDown.TimeOut)
                {
                    cnt--;
                    mTakePhotoCountDown.FillTime();
                    AudioManager.Instance.Play(GuideAudio.sCountDown(cnt));
                }

                yield return null;
            }

            PhotoManager.Instance.StartCapture();
            mTakePhotoCountDown.FillTime();
        }


        /// <summary>
        /// 当结束照相时触发
        /// </summary>
        public void OnFinishTakePhoto()
        {
            AudioManager.Instance.Play(GuideAudio.sFinishTakePhoto);
            _bInTakingPhoto = false;
        }

        /// <summary>
        /// 向导运动的更新
        /// </summary>
        private void UpdateMovement()
        {
            var charTransform = transform;

            Vector3 transPos = charTransform.position;
            transPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude; // 只需要修改 y 坐标
            charTransform.position = transPos; // 修改物体的坐标

            Vector3 transRot = charTransform.eulerAngles;
            transRot.x = 0f;
            charTransform.eulerAngles = transRot;
        }


        /// <summary>
        /// 当说出【跟着我】的时候触发
        /// </summary>
        public void OnCallFollowMe()
        {
            if (CallActive)
            {   AudioManager.Instance.Play(GuideAudio.sYes);
                gameObject.GetComponent<FollowMeToggle>().SetFollowMeBehavior(true);
            }
        }


        /// <summary>
        /// 当说出【停下】的时候触发
        /// </summary>
        public void OnCallCancelFollowMe()
        {
            if (CallActive)
            {
                AudioManager.Instance.Play(GuideAudio.sYes);
                gameObject.GetComponent<FollowMeToggle>().SetFollowMeBehavior(false);
            }
        }


        /// <summary>
        /// 当说出【拍照】时触发
        /// </summary>
        public void OnCallTakePhoto()
        {
            if (CallActive && !_bInTakingPhoto)
            {
                OnClickTakePhoto();
            }
        }

        /// <summary>
        /// 点击开始拍照的时候
        /// </summary>
        public void OnClickTakePhoto()
        {
            StartCoroutine(StartCountDown());
        }

        /// <summary>
        /// 触摸头部交互
        /// </summary>
        public void OnTouchHead()
        {
            OnTouch("Head");
        }


        /// <summary>
        /// 触摸手部交互
        /// </summary>
        public void OnTouchHand()
        {
            OnTouch("Hand");
        }


        /// <summary>
        /// 触摸脚部交互
        /// </summary>
        public void OnTouchFoot()
        {
            OnTouch("Foot");
        }


        /// <summary>
        /// 当语音交互叫喊【派蒙】的时候
        /// </summary>
        public void OnCallName()
        {
            AudioManager.Instance.Play(GuideAudio.sRespond);
            mCallingCountDown.FillTime();
            _bIsCallingActive = true;
        }

        private void OnTouch(string part)
        {
            if (!_bInPlayAudio)
            {
                _bInPlayAudio = true;
                switch (part)
                {
                    case "Head":
                        AudioManager.Instance.Play(GuideAudio.sTouchHead);
                        break;
                    case "Hand":
                        AudioManager.Instance.Play(GuideAudio.sTouchHand);
                        break;
                    case "Foot":
                        AudioManager.Instance.Play(GuideAudio.sTouchFoot);
                        break;
                }
            }
        }
    }
}