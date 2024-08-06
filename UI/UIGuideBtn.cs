using System.Collections;
using System.Collections.Generic;
using Character;
using Manager;
using UI;
using UnityEngine;

public class UIGuideBtn : MonoBehaviour
{
    public GameObject objSearchBoard;
    public GameObject objGameBoard;
    public GameObject objNavigationBoard;
    public GameObject objExplanationBoard;

    private Dictionary<GuideState, GameObject> _dicState2Board = new Dictionary<GuideState, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // 初始化
        _dicState2Board.Add(GuideState.SEARCH, objSearchBoard);
        _dicState2Board.Add(GuideState.GAME, objGameBoard);
        _dicState2Board.Add(GuideState.NAVIGATION, objNavigationBoard);
        _dicState2Board.Add(GuideState.EXPLANATION, objExplanationBoard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 当说出【查询】时触发
    /// </summary>
    public void OnCallSearch()
    {
        if (GuideBehavior.Instance.CallActive)
        {
            OnClickSearchBtn();
        }
    }

    /// <summary>
    /// 当说出【导航】时触发
    /// </summary>
    public void OnCallNavigation()
    {
        if (GuideBehavior.Instance.CallActive)
        {
            OnClickNavigationBtn();
        } 
    }
    
    /// <summary>
    /// 当说出【讲解】时触发
    /// </summary>
    public void OnCallExplanation()
    {
        if (GuideBehavior.Instance.CallActive)
        {
            OnClickExplanationBtn();
        } 
    }

    /// <summary>
    /// 当说出【游戏】时触发
    /// </summary>
    public void OnCallGame()
    {
        if (GuideBehavior.Instance.CallActive)
        {
            OnClickGameBtn();
        } 
    }
    /// <summary>
    /// 点击【查询】按钮时触发
    /// </summary>
    public void OnClickSearchBtn()
    {
        OnChangeGuideState(GuideState.SEARCH);
        objSearchBoard.GetComponent<UISearchBoard>().OnActiveBoard();
    }


    /// <summary>
    /// 点击【导航】按钮时触发
    /// </summary>
    public void OnClickNavigationBtn()
    {
        OnChangeGuideState(GuideState.NAVIGATION);
    }

    /// <summary>
    /// 点击【讲解】按钮时触发
    /// </summary>
    public void OnClickExplanationBtn()
    {
        OnChangeGuideState(GuideState.EXPLANATION);
    }

    /// <summary>
    /// 点击【游戏】按钮时触发
    /// </summary>
    public void OnClickGameBtn()
    {
        OnChangeGuideState(GuideState.GAME);
    }


    /// <summary>
    /// 改变状态的时候触发
    /// </summary>
    /// <param name="state"></param>
    private void OnChangeGuideState(GuideState state)
    {
        foreach (var board in _dicState2Board.Values)
        {
            board.SetActive(false);
        }
        _dicState2Board[state].SetActive(true);
        GuideBehavior.Instance.mState = state;
        AudioManager.Instance.Play(GuideAudio.sInfo);
        GuideBehavior.Instance.aAnimator.Play("Jump1");
    }

}
