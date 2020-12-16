using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField]
    private AudioClip BGM_Mypage;

    [SerializeField]
    private AudioClip BGM_Game_Main;

    [SerializeField]
    private AudioClip SE_Stone_Tap;

    [SerializeField]
    private AudioClip SE_Stone_roll;

    [SerializeField]
    private AudioClip SE_ButtonTap;

    [SerializeField]
    private AudioSource _audioSource;


    public void Awake()
    {

        if (this != Instance) {

            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

    }


    // Start is called before the first frame update
    void Start()
    {
        Instance._audioSource = this.GetComponent<AudioSource>();
        Instance.BGM_Mypage_Play();
    }


    public void BGM_Mypage_Play() {
   
        Instance._audioSource.clip = BGM_Mypage;
        Instance._audioSource.Play();
    }


    public void BGM_Game_Main_Play() {

        Instance._audioSource.clip = BGM_Game_Main;
        Instance._audioSource.Play();
    }


    public void SE_Stone_Tap_Play() {

        Instance._audioSource.PlayOneShot(SE_Stone_Tap);
        
    }

    public void SE_Stone_Turn() {


        Instance._audioSource.PlayOneShot(SE_Stone_roll);

    }








   






}
