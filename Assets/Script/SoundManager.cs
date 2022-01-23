using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {

            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }

            return instance;
        }
    }

    private AudioSource bgmPlayer;
    private AudioSource sfxPlayer;

    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;

    [SerializeField]
    private AudioClip mainBgmAudioClip; //메인 BGM
    [SerializeField]
    private AudioClip gameBgmAudioClip; //게임 BGM

    [SerializeField]
    private AudioClip[] sfxAudioClips; //효과음들 지정

    Dictionary<string, AudioClip> audioClipsDic = new Dictionary<string, AudioClip>(); //효과음 딕셔너리
    // AudioClip을 Key,Value 형태로 관리하기 위해 딕셔너리 사용

    [SerializeField]
    private Slider bgmSlider, sfxSlider;

    private GameState _tempState = GameState.Ready;

    bool _eating = false, _teaching = false;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject); //여러 씬에서 사용할 것.

        bgmPlayer = GameObject.Find("BGMSoundPlayer").GetComponent<AudioSource>();
        sfxPlayer = GameObject.Find("SFXSoundPlayer").GetComponent<AudioSource>();
        bgmSlider.value = masterVolumeSFX;
        sfxSlider.value = masterVolumeBGM;

        foreach (AudioClip audioclip in sfxAudioClips)
        {
            audioClipsDic.Add(audioclip.name, audioclip);
        }
    }

    private void Update()
    {
        if(GameManager.instance.state != _tempState
            && GameManager.instance.state != GameState.End && GameManager.instance.state != GameState.Stop)
        {
            PlayBGMSound();
            _tempState = GameManager.instance.state;
        }

        if(!_teaching)
            Invoke("BlahSound", 0);

        if(!_eating)
            Invoke("EatSound", 0);


    }

    void BlahSound()
    {
        if (GameManager.instance.checkReadyTimer <= 0f
            && GameManager.instance.state == GameState.Start)
        {
            _teaching = true;

            PlaySFXSound("Professer_2");
            Invoke("BlahSound", 3f);
        }
        else
        {
            _teaching = false;
            return;
        }
    }

    void EatSound()
    {
        if (GameManager.instance.isEating == true
            && GameManager.instance.state == GameState.Start)
        {
            _eating = true;

            PlaySFXSound("Eating");
            Invoke("EatSound", 3f);
        }
        else
        {
            _eating = false;
            return;
        }
    }
    void OnDisable()
    {
        CancelInvoke();
    }

    // 효과 사운드 재생 : 이름을 필수 매개변수, 볼륨을 선택적 매개변수로 지정
    public void PlaySFXSound(string name, float volume = 1f)
    {
        if (audioClipsDic.ContainsKey(name) == false)
        {
            Debug.Log(name + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.PlayOneShot(audioClipsDic[name], volume * masterVolumeSFX);
    }

    //BGM 사운드 재생 : 볼륨을 선택적 매개변수로 지정
    public void PlayBGMSound(float volume = 1f)
    {
        bgmPlayer.loop = true; //BGM 사운드이므로 루프설정
        bgmPlayer.volume = volume * masterVolumeBGM;

        if(GameManager.instance.state == GameState.Ready)
        {
            bgmPlayer.clip = mainBgmAudioClip;
        }
        else
        {
            bgmPlayer.clip = gameBgmAudioClip;
        }
        bgmPlayer.Play();
    }

    public void SetVolume(int _type)
    {
        //0 = BGM // 1 = SFX
        switch (_type)
        {
            case 0:
                masterVolumeBGM = bgmSlider.value;
                bgmPlayer.volume = masterVolumeBGM;

                break;
            case 1:
                masterVolumeSFX = sfxSlider.value;
                break;
        }
    }
}

