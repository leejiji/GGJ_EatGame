using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KeyManager : MonoBehaviour
{
    [SerializeField] CanvasGroup settingPanel;
    [SerializeField] GameObject creditBtn;
    void Update()
    {
        switch (GameManager.instance.state)
        {
            case GameState.Start:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    settingPanel.alpha = 1;
                    settingPanel.blocksRaycasts = true;
                    settingPanel.interactable = true;
                    creditBtn.SetActive(false);
                    GameManager.instance.state = GameState.Stop;
                }
                break;

            case GameState.Stop :
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    settingPanel.alpha = 0;
                    settingPanel.blocksRaycasts = false;
                    settingPanel.interactable = false;
                    creditBtn.SetActive(true);
                    GameManager.instance.state = GameState.Start;
                }
                break;
        }
    }
}
