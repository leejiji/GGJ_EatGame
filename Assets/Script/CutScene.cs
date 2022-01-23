using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SceneState { Ready, Start, End }

public class CutScene : MonoBehaviour
{
    public SceneState state;

    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject camObject;

    [SerializeField]
    private TileAnimation moniterAni;
    [SerializeField]
    private CanvasGroup CutScenePanel;    
    [SerializeField]
    private Animator CutSceneAni;
    [SerializeField]
    private TextMeshProUGUI dialText;
    [SerializeField]
    private string[] Dialog;
    

    [SerializeField]
    private float speed;
    public bool isOver = false;
    bool init = false;
    bool dialEvent = false;

    private void Start()
    {
        state = SceneState.Ready;
    }
    void Update()
    {
        switch (GameManager.instance.state)
        {
            case GameState.Ready:
                {
                    if (!init)
                    {
                        int _pptType = Random.Range(0, 3);
                        CutSceneAni.Play("Start");
                        moniterAni.ChangeFrame(_pptType, 21, 1, 0.15f, true);
                        CutScenePanel.alpha = 0;
                        state = SceneState.Ready;
                        camObject.SetActive(true);
                        isOver = false;
                        dialEvent = false;
                        init = true;
                    }
                }
                break;

            case GameState.Start:
                if (!dialEvent)
                    StartCoroutine(DialogEvent(0));
                break;

            case GameState.Stop:                
                break;

            case GameState.End:
                if (state == SceneState.Ready)
                {
                    camObject.SetActive(false);

                    float angle = Quaternion.Angle(mainCamera.transform.rotation,
                        Quaternion.LookRotation(Vector3.forward));

                    if (angle <= 0)
                    {
                        state = SceneState.Start; 
                    }
                    else
                    {
                        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, Quaternion.LookRotation(Vector3.forward), Time.deltaTime * speed);
                    }
                }
                else if(state == SceneState.Start)
                {
                    if(isOver == false)
                    {
                        if (GameManager.instance.isClear)
                        {
                            StartCoroutine(GameClearScene());
                        }
                        else
                        {
                            StartCoroutine(DialogEvent(1));
                            StartCoroutine(GameOverCutScene());
                        }
                    }                              
                }
                break;
        }
    }

    //0 = start / 1 = gameOver / 2 = clear //
    IEnumerator DialogEvent(int _type)
    {
        dialEvent = true;
        string[] _dialog;
        _dialog = Dialog[_type].Split('/');

        for(int i = 0; i < _dialog.Length; i++)
        {
            dialText.text = _dialog[i];
            yield return new WaitForSeconds(2f);
        }
        dialText.text = "";
    }

    IEnumerator GameClearScene()
    {
        isOver = true;

        yield return new WaitForSeconds(0.5f); // 깜빡
        CutScenePanel.alpha = 1;
        StartCoroutine(DialogEvent(2));
        CutSceneAni.Play("GameClear", -1, 0f);
        SoundManager.Instance.PlaySFXSound("clear");
        yield return new WaitForSeconds(7f); //전체화면 애니

        state = SceneState.End;
        init = false;
    }

    IEnumerator GameOverCutScene()
    {
        isOver = true;
        int _rand = Random.Range(1, 3);
        
        moniterAni.ChangeFrame(3, 31, 12, 1, true);
        yield return new WaitForSeconds(3f); //모니터 애니

        CutScenePanel.alpha = 1;
        yield return new WaitForSeconds(0.5f); // 깜빡

        CutSceneAni.Play("GameOver_0", -1, 0f);
        yield return new WaitForSeconds(1f);
        CutSceneAni.Play("GameOver_" + _rand.ToString(), -1, 0f);
        SoundManager.Instance.PlaySFXSound("fail");
        yield return new WaitForSeconds(2.5f); //전체화면 애니
        
        state = SceneState.End;
        init = false;
    }
}
