using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    private float[] bestScore = new float[5];
    private string[] bestName = new string[5];

    // 내 정보 텍스트
    [SerializeField]
    InputField nameField;
    [SerializeField]
    Text RankNameCurrent,RankScoreCurrent, meText;

    // 랭크들 텍스트
    [SerializeField]
    Text[] RankScoreText, RankNameText, RankText; 

    // 랭크 저장용
    float[] rankScore = new float[5]; 
    string[] rankName = new string[5];
    
    private void Start()
    {        
        for (int i = 0; i < 5; i++)
        {
            if(PlayerPrefs.HasKey(i + "BestScore") == false)
            {
                PlayerPrefs.SetFloat(i + "BestScore", 5999);
                PlayerPrefs.SetString(i + "BestName", "");
            }            
        }
    }

    //현재 플레이어의 점수와 이름을 받아서 실행
    public void ScoreSet()
    {
        if(nameField.text != "")
        {
            string currentName = nameField.text;
            float currentScore = GameManager.instance.allTimer >= 5999 ? 5999 : GameManager.instance.allTimer;

            //일단 현재에 저장하고 시작
            PlayerPrefs.SetString("CurrentPlayerName", currentName);
            PlayerPrefs.SetFloat("CurrentPlayerScore", currentScore);

            float tmpScore = 0f;
            string tmpName = "";

            for (int i = 0; i < 5; i++)
            {
                //저장된 최고점수와 이름을 가져오기
                bestScore[i] = PlayerPrefs.GetFloat(i + "BestScore");
                bestName[i] = PlayerPrefs.GetString(i + "BestName");

                //현재 점수가 랭킹에 오를 수 있을 때
                while (bestScore[i] > currentScore)
                {
                    //자리바꾸기!
                    tmpScore = bestScore[i];
                    tmpName = bestName[i];
                    bestScore[i] = currentScore;
                    bestName[i] = currentName;

                    //랭킹에 저장
                    PlayerPrefs.SetFloat(i + "BestScore", currentScore);
                    PlayerPrefs.SetString(i.ToString() + "BestName", currentName);

                    //다음 반복을 위한 준비
                    currentScore = tmpScore;
                    currentName = tmpName;
                }
            }
            //랭킹에 맞춰 점수와 이름 저장
            for (int i = 0; i < 5; i++)
            {
                PlayerPrefs.SetFloat(i + "BestScore", bestScore[i]);
                PlayerPrefs.SetString(i.ToString() + "BestName", bestName[i]);
            }
        }
        else
        {
            float currentScore = GameManager.instance.allTimer >= 5999 ? 5999 : GameManager.instance.allTimer;
            PlayerPrefs.SetString("CurrentPlayerName", "내 기록");
            PlayerPrefs.SetFloat("CurrentPlayerScore", currentScore);
        }

        ScoreGet();
    }

    public void ScoreGet()
    {
        //플레이어 이름, 점수 텍스트를 현재 '나'의 이름과 점수에 표시
        RankNameCurrent.text = PlayerPrefs.GetString("CurrentPlayerName");
        RankScoreCurrent.text = string.Format("{0:D2}:{1:D2}", (int)PlayerPrefs.GetFloat("CurrentPlayerScore")/60, (int)PlayerPrefs.GetFloat("CurrentPlayerScore") % 60);

        //랭킹에 맞춰 불러온 점수와 이름을 표시하는 부분
        for (int i = 0; i < 5; i++)
        {
            Color common = new Color(0, 0, 0);
            RankText[i].color = common;
            RankNameText[i].color = common;
            RankScoreText[i].color = common;

            rankScore[i] = PlayerPrefs.GetFloat(i + "BestScore");
            RankScoreText[i].text = rankScore[i] >= 5999 ? "00:00" :
                string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(rankScore[i] / 60), Mathf.FloorToInt(rankScore[i] % 60));
            rankName[i] = PlayerPrefs.GetString(i.ToString() + "BestName");
            RankNameText[i].text = string.Format(rankName[i]);            

            //강조 표시
            if (Mathf.FloorToInt(PlayerPrefs.GetFloat("CurrentPlayerScore")) == Mathf.FloorToInt(rankScore[i])
                && PlayerPrefs.GetString("CurrentPlayerName") != "내 기록")
            {
                Color Rank = new Color(255, 172, 0);
                RankText[i].color = Rank;
                RankNameText[i].color = Rank;
                RankScoreText[i].color = Rank;
            }

        }
    }

    public void MainScore()
    {
        meText.gameObject.SetActive(false);
        RankNameCurrent.gameObject.SetActive(false);
        RankScoreCurrent.gameObject.SetActive(false);

        for (int i = 0; i < 5; i++)
        {        
            rankScore[i] = PlayerPrefs.GetFloat(i + "BestScore");
            RankScoreText[i].text = rankScore[i] >= 5999 ? "00:00" :
                string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(rankScore[i] / 60), Mathf.FloorToInt(rankScore[i] % 60));
            rankName[i] = PlayerPrefs.GetString(i.ToString() + "BestName");
            RankNameText[i].text = string.Format(rankName[i]);
        }
    }

    public void RankOff()
    {
        nameField.text = null;
        meText.gameObject.SetActive(true);
        RankNameCurrent.gameObject.SetActive(true);
        RankScoreCurrent.gameObject.SetActive(true);
    }
}