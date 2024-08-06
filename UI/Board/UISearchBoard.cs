using System.Collections;
using Character;
using Manager;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Http;


namespace UI
{
    public class UISearchBoard : MonoBehaviour
    {
        public GameObject listSearchBtn; // 搜索页面的列表

        public TextMeshProUGUI uiTitle; // 标题
        public TextMeshProUGUI uiIntroduction; // node 节点的介绍
        public TextMeshProUGUI uiSearchText; // 搜索的文本
        public TextMeshProUGUI uiDetailIntroduction; // leaf 节点的介绍

        public ScrollingObjectCollection cScrollingObjectCollection;

        public GameObject uiImage;//节点图像

        private TouchScreenKeyboard _objKeyboard; // 键盘
        private string lastPlayLeafName = null;

        #region UI 按钮相关
        /// <summary>
        /// 当激活的时候触发
        /// </summary>
        public void OnActiveBoard()
        {
            string lastContent = SearchBehavior.GetLatestSearchContent();
            StartCoroutine(LoadContent(lastContent));
        }

        /// <summary>
        /// 当点击【上一级】的时候触发
        /// </summary>
        public void OnClickBack()
        {
            
            cScrollingObjectCollection.MoveToIndex(-1);
            string prevContent = SearchBehavior.GetPrevSearchContent();
            StartCoroutine(LoadContent(prevContent));
            AudioManager.Instance.Stop(lastPlayLeafName);
            // Text2AudioManager.Instance.Stop();
        }


        /// <summary>
        /// 当点击【上滑】时触发
        /// </summary>
        public void OnClickScrollUp()
        {
            cScrollingObjectCollection.MoveByTiers(-1);
        }

        /// <summary>
        /// 当点击【下滑】时触发
        /// </summary>
        public void OnClickScrollDown()
        {
            cScrollingObjectCollection.MoveByTiers(1);
        }


        /// <summary>
        /// 当点击【键盘搜索】时触发
        /// </summary>
        public void OnClickKeyboardSearch()
        {
            _objKeyboard = TouchScreenKeyboard.Open("");
        }


        /// <summary>
        /// 当点击【搜索】时触发
        /// </summary>
        public void OnClickSearch()
        {
            StartCoroutine(LoadFuzzyContent(uiSearchText.text));
        }


        /// <summary>
        /// 当点击【关闭页面】时触发
        /// </summary>
        public void OnClickCloseBoard()
        {
            GuideBehavior.Instance.mState = GuideState.IDLE;
            AudioManager.Instance.Stop(lastPlayLeafName);
            AudioManager.Instance.Play(GuideAudio.sInfo);
            // Text2AudioManager.Instance.Stop();
        }

        #endregion
        


        /// <summary>
        /// 当点击查询资料节点btn时触发
        /// </summary>
        /// <param name="btn"></param>
        private void OnClickNodeBtn(GameObject btn)
        {
            
            string content = btn.GetComponent<ButtonConfigHelper>().MainLabelText;
            // Text2AudioManager.Instance.Stop();
            // Text2AudioManager.Instance.Speak(content);
            // Text2AudioManager.Instance.GetAudioFromText(content);
            StartCoroutine(LoadContent(content));
        }

        

        /// <summary>
        /// 载入内容 -> content: 点击指定的btn进入
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IEnumerator LoadContent(string content)
        {
            PrevLoadContent();
            SearchNode node = SearchBehavior.GetNodeByContent(content);
            if (node == null)
            {
                HttpRequest request = new HttpRequest();
                string url = $"{HttpConfig.httpARMuseumDB}?node_name={content}";
                StartCoroutine(request.Get(url));
                while (!request.isComplete)
                {
                    yield return null;
                }
                node = SearchNode.GetNodeByJson(request.value);
                if (node.sNodeName == "null")
                {
                    uiTitle.text = "暂未查询到相关内容,请重新搜索";
                    yield break;
                }
                SearchBehavior.AddNode(content, node);
            }
            SetImageContent(node.listPicturePath[0]);
            uiTitle.text = node.sNodeName;
            if (node.sNodeType == "node")
            {
                cScrollingObjectCollection.MoveToIndex(-1);
                listSearchBtn.SetActive(true);
                uiIntroduction.text = node.sIntroduction != "null" ? node.sIntroduction : "";
                // Text2AudioManager.Instance.Speak(uiIntroduction.text);
                uiDetailIntroduction.text = "";
                int subNodeCnt = node.listSubNodes.Count;
                for (int i = 0; i < listSearchBtn.transform.childCount; i++)
                {
                    if (i + 1 > subNodeCnt)
                    {
                        listSearchBtn.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        if (node.listSubNodes[i] == "null")
                        {
                            listSearchBtn.transform.GetChild(i).gameObject.SetActive(false);
                            continue;
                        }

                        GameObject btn = listSearchBtn.transform.GetChild(i).gameObject;
                        btn.SetActive(true);
                        string subName = node.listSubNodes[i];
                        btn.GetComponent<ButtonConfigHelper>().MainLabelText = subName;
                        btn.GetComponent<ButtonConfigHelper>().OnClick.RemoveAllListeners();
                        btn.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate { OnClickNodeBtn(btn); });
                    }
                }
            }
            else if (node.sNodeType == "leaf")
            {
                listSearchBtn.SetActive(false);
                uiIntroduction.text = "";
                uiDetailIntroduction.text = node.sIntroduction != "null" ? node.sIntroduction : "";
                var audioManagerIns = AudioManager.Instance;
                var path = GuideAudio.sText(node.sNodeName);
                if (lastPlayLeafName != null)
                {
                    audioManagerIns.Stop(lastPlayLeafName);
                }

                lastPlayLeafName = path;
                audioManagerIns.Play(path);
                // Text2AudioManager.Instance.SpeakText(uiDetailIntroduction.text);
            }
        }

        /// <summary>
        /// 搜索内容 -> 模糊搜索时使用
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IEnumerator LoadFuzzyContent(string content)
        {
            PrevLoadContent();
            uiTitle.text = "以下是你的查询结果: ";
            HttpRequest request = new HttpRequest();
            string url = $"{HttpConfig.httpARMuseumFuzzy}?node_name={content}";
            StartCoroutine(request.Get(url));
            while (!request.isComplete)
            {
                yield return null;
            }

            if (int.Parse(request.value[0]["cnt"].ToString()) == 0)
            {
                uiIntroduction.text = "";
                uiDetailIntroduction.text = "没有找到相关资料哦";
            }
            else
            {
                cScrollingObjectCollection.MoveToIndex(-1);
                int cnt = int.Parse(request.value[0]["cnt"].ToString());
                listSearchBtn.SetActive(true);
                uiIntroduction.text = "";
                uiDetailIntroduction.text = "";
                for (int i = 0; i < listSearchBtn.transform.childCount; i++) 
                {
                    if (i + 1 > cnt)
                    {
                        listSearchBtn.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        GameObject btn = listSearchBtn.transform.GetChild(i).gameObject;
                        btn.SetActive(true);
                        string nodeName = request.value[i + 1]["name"].ToString();
                        btn.GetComponent<ButtonConfigHelper>().MainLabelText = nodeName;
                        btn.GetComponent<ButtonConfigHelper>().OnClick.RemoveAllListeners();
                        btn.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate { OnClickNodeBtn(btn); });
                    }
                }
            }
        }

        
        /// <summary>
        /// 搜索前的准备
        /// </summary>
        private void PrevLoadContent()
        {
            for (int i = 0; i < listSearchBtn.transform.childCount; i++)
            {
                listSearchBtn.transform.GetChild(i).gameObject.SetActive(false);
                uiTitle.text = "资料正在载入,请稍后!";
            }
        }
        
        
        /// <summary>
        /// 载入图片内容
        /// </summary>
        /// <param name="path"></param>
        private void SetImageContent(string path)
        {
            if (path != "null")
            {
                uiImage.SetActive(true);
                uiImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/"+path);
            }
            else
            {
                uiImage.SetActive(false);
            }
        }

        private void Start()
        {
            // 允许显示输入
            TouchScreenKeyboard.hideInput = false;
        }

        private void Update()
        {
            // 键盘的更新
            if (_objKeyboard != null && _objKeyboard.active)
            {
                uiSearchText.text = _objKeyboard.text;
            }
            else
            {
                uiSearchText.text = string.Empty;
            }
        }
        
    }
}