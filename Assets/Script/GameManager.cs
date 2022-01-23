using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GameState { Ready, Start, End, Stop }
public enum GameEventState { StudentCheck }
public class GameManager : MonoBehaviour
{

    //싱글톤
    public static GameManager instance;


    //현재 게임 상태
    [Header("State")]
    public GameState state;
    //현재 이벤트 상태
    public GameEventState eventState;

    //게임 종료 타이머
    [Header("Timer")]
    [SerializeField]
    private float maxTimer;
    [SerializeField]
    private float curTimer;


    //밥먹는지 체크하는 타이머
    [Space(10f)]
    [SerializeField]
    private float checkTimer;
    [SerializeField]
    private float maxCheckTimer;
    [SerializeField]
    private float minCheckTimer;
    [SerializeField]
    private float curCheckTimer;

    //플레이어에게 경고하는 타이머★
    [Space(10f)]
    [SerializeField]
    public float checkReadyTimer;

    //전체 타이머★
    [HideInInspector]
    public float allTimer;
    [SerializeField]
    private Text allTimeText;

    //레이캐스트
    private RaycastHit hit;
    private float maxDistance = 15f;

    //UI
    [Header("UI"), Space(10f)]
    public Image timerImage;


    public CanvasGroup failPanel;
    public CanvasGroup successPanel;
    public CanvasGroup RankPanel;
    //게임 클리어 확인
    [HideInInspector] public bool isClear = false;

    //먹는 중 확인
    [HideInInspector]
    public bool isEating = false; //능지 이슈로 public으로 전환★
    [SerializeField]
    private bool isChecking = false;

    [Header("Objects"), Space(10f)]
    public GameObject computer;
    public GameObject[] objs;

    //연출 관련★
    [Header("Effect"), Space(10f)]
    [SerializeField]
    private CutScene cutScene; //컷씬관련★
    [SerializeField]
    private GameObject Glitch;
    public GameObject eatParticle;
    bool isRanking = false; //능지이슈로 랭킹 입력 창 처리 일케함★

    //음식 오브젝트
    [Header("Food")]
    public GameObject[] foods;
    [SerializeField]
    private List<GameObject> curFoods = new List<GameObject>();

    [Range(1, 3)]
    public int foodCount = 1;
    public Transform[] foodSpawnPoint;


    //중복 없는 랜덤 
    private List<int> nSameList = new List<int>();


    private void Awake()
    {
        instance = this;

        state = GameState.Ready;
        checkTimer = Random.Range(minCheckTimer, maxCheckTimer);
        SoundManager.Instance.PlayBGMSound();
    }



    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
    }


    void Update()
    {
        switch (state)
        {

            case GameState.Ready:
                curTimer = maxTimer;


                curCheckTimer = 0;
                checkReadyTimer = 0;
                allTimer = 0;

                timerImage.fillAmount = 1;

                eatParticle.SetActive(false);
                isClear = false;
                isRanking = false;
                break;
            case GameState.Start:
                allTimer += Time.deltaTime;
                //바라보는 이벤트
                LookEvent();

                switch (eventState)
                {
                    case GameEventState.StudentCheck:
                        CheckStudent();
                        break;
                }


                if (curTimer - Time.deltaTime >= 0)
                {
                    curTimer -= Time.deltaTime;
                }
                else
                {
                    curTimer = 0;
                    state = GameState.End;
                }

                timerImage.fillAmount = curTimer / maxTimer;

                break;

            case GameState.Stop:
                Glitch.SetActive(false);
                break;

            case GameState.End:
                eatParticle.SetActive(false);
                Glitch.SetActive(false);

                if (cutScene.state == SceneState.End)
                {
                    if (isClear)
                    {
                        if (!isRanking)
                        {
                            //랭크 입력 창 활성화★
                            allTimeText.text = allTimer >= 5999 ? "00:00" :
                                string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(allTimer / 60), Mathf.FloorToInt(allTimer % 60));
                            RankPanel.alpha = 1;
                            RankPanel.blocksRaycasts = true;
                            RankPanel.interactable = true;

                            isRanking = true;
                        }
                        successPanel.alpha = 1;
                        successPanel.blocksRaycasts = true;
                        successPanel.interactable = true;
                    }
                    else
                    {
                        failPanel.alpha = 1;
                        failPanel.blocksRaycasts = true;
                        failPanel.interactable = true;
                    }
                }
                break;
        }

    }

    //바라보는 이벤트 함수
    private void LookEvent()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * maxDistance, Color.red);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
        {
            //아웃라인 초기화
            foreach (var f in curFoods)
            {
                if (hit.transform.gameObject != f)
                {
                    f.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 0);
                }

            }

            foreach (var o in objs)
            {
                if (hit.transform.gameObject != o)
                {
                    o.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 0);
                }

            }

            if (hit.transform.CompareTag("Food"))
            {


                hit.transform.GetComponent<FoodsInfo>().foodTimerImage.enabled = true;

                //아웃라인 
                if (hit.transform.gameObject.name.Contains("onigiri") || hit.transform.gameObject.name.Contains("rice")
                    || hit.transform.gameObject.name.Contains("grape") || hit.transform.gameObject.name.Contains("meat"))
                {
                    hit.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 2f);
                }
                else if (hit.transform.gameObject.name.Contains("ramen"))
                {
                    hit.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 1.5f);
                }
                else
                {
                    hit.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 0.012f);
                }


                if (Input.GetMouseButton(0))
                {
                    if (hit.transform.GetComponent<FoodsInfo>().curFoodTimer >= hit.transform.GetComponent<FoodsInfo>().maxFoodTimer)
                    {
                        //현재 음식 다 먹음

                        hit.transform.GetComponent<FoodsInfo>().isEmpty = true;


                        //모든 음식을 다먹었는지 확인
                        isClear = true;

                        for (int i = 0; i < curFoods.Count; i++)
                        {
                            if (!curFoods[i].transform.GetComponent<FoodsInfo>().isEmpty)
                            {
                                isClear = false;

                            }
                        }

                        if (isClear)
                        {
                            state = GameState.End;                            
                        }

                    }
                    else
                    {
                        hit.transform.GetComponent<FoodsInfo>().curFoodTimer += Time.deltaTime;
                        hit.transform.GetComponent<FoodsInfo>().foodTimerImage.GetComponent<Image>().fillAmount = hit.transform.GetComponent<FoodsInfo>().curFoodTimer / hit.transform.GetComponent<FoodsInfo>().maxFoodTimer;
                    }
                    if (!hit.transform.GetComponent<FoodsInfo>().isEmpty) //현재 음식이 있는지 확인
                    {
                        eatParticle.SetActive(true);
                        isEating = true;
                    }
                    isChecking = false;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    eatParticle.SetActive(false);
                }
            }
            else if (hit.transform.CompareTag("Computer"))
            {
                foreach (var o in objs)
                {
                    if ("monitor" == o.name)
                    {
                        o.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 0.02f);
                    }

                }
                eatParticle.SetActive(false);

                foreach (GameObject f in curFoods)
                {
                    if (!f.GetComponent<FoodsInfo>().isEmpty)
                    {
                        f.GetComponent<FoodsInfo>().foodTimerImage.enabled = false;
                    }

                }

                isChecking = true;
                isEating = false;
            }
            else
            {
                eatParticle.SetActive(false);

                foreach (GameObject f in curFoods)
                {
                    if (!f.GetComponent<FoodsInfo>().isEmpty)
                    {
                        f.GetComponent<FoodsInfo>().foodTimerImage.enabled = false;
                    }

                }

                isChecking = false;
                isEating = false;
            }
        }
        else
        {
            eatParticle.SetActive(false);

            foreach (GameObject f in curFoods)
            {
                if (!f.GetComponent<FoodsInfo>().isEmpty)
                {
                    f.GetComponent<FoodsInfo>().foodTimerImage.enabled = false;
                }

            }

            //아웃라인 초기화
            foreach (var f in curFoods)
            {
                f.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 0);
            }
            foreach (var o in objs)
            {
                o.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("_Scale", 0);
            }

            isChecking = false;
            isEating = false;
        }
    }


    //교수님이 학생들 체크
    private void CheckStudent()
    {

        if (curCheckTimer >= checkTimer)
        {
            Check();
        }
        else
        {
            curCheckTimer += Time.deltaTime;
        }

    }

    private void Check()
    {
        if (checkReadyTimer >= 3f)
        {
            StartCoroutine(LookStudent());

            if (isChecking)
            {
                checkTimer = Random.Range(minCheckTimer, maxCheckTimer);
                curCheckTimer = 0;
                checkReadyTimer = 0;

            }
            else
            {
                state = GameState.End;
                checkTimer = Random.Range(minCheckTimer, maxCheckTimer);
                curCheckTimer = 0;
                checkReadyTimer = 0;
            }


        }
        else
        {
            checkReadyTimer += Time.deltaTime;
            Glitch.SetActive(true);
        }

    }


    private IEnumerator LookStudent()
    {
        Glitch.SetActive(false);
        computer.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(1f);
        computer.GetComponent<Renderer>().material.color = Color.black;
    }

    //음식 초기화
    public void FoodsReset()
    {
        for (int i = curFoods.Count - 1; i >= 0; i--)
        {
            Destroy(curFoods[i]);
        }
        curFoods.Clear();

        nSameList.Clear();

        CreateUnDuplicateRandom(0, foods.Length, foodCount);
        for (int i = 0; i < foodCount; i++)
        {
            GameObject food = Instantiate(foods[nSameList[i]], new Vector3(foodSpawnPoint[i].position.x, foods[nSameList[i]].GetComponent<FoodsInfo>().posY, foodSpawnPoint[i].position.z), foods[nSameList[i]].transform.rotation);
            curFoods.Add(food);
        }

    }




    // 랜덤 생성 (중복 배제)
    private void CreateUnDuplicateRandom(int min, int max, int count)
    {
        int currentNumber = Random.Range(min, max);

        for (int i = 0; i < count;)
        {
            if (nSameList.Contains(currentNumber))
            {
                currentNumber = Random.Range(min, max);
            }
            else
            {
                nSameList.Add(currentNumber);
                i++;
            }
        }
    }
}
