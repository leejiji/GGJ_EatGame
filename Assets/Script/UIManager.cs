using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public void OnCanvasGroup(CanvasGroup _group)
    {
        _group.alpha = 1;
        _group.blocksRaycasts = true;
        _group.interactable = true;
    }
    public void OffCanvasGroup(CanvasGroup _group)
    {
        _group.alpha = 0;
        _group.blocksRaycasts = false;
        _group.interactable = false;
    }
    public void GameStart()
    {
        GameManager.instance.state = GameState.Start;
        GameManager.instance.FoodsReset();
    }
    public void GameExit()
    {
        Application.Quit();
    }
    //★설정 재시작 버튼
    public void GameReStart()
    {
        if(GameManager.instance.state == GameState.Stop)
            GameManager.instance.state = GameState.Start;
    }

    //0-Ready, 1-Start, 2-End
    public void ChangeState(int _state)
    {
        GameManager.instance.state = (GameState)_state;
    }
}
