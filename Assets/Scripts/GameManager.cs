using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int CurrentLevel;

    public GameObject GameOverUI, GameWinUI;
    public bool finished;
    public Text currentText, nextText;
    public GameObject[] Level;

    public AudioSource BackgroundSound, GameoverSound, ClickSound;

    [Header("Reaction")]
    public GameObject[] Emoji;

    [Header("Control")]
    public GameObject Dpad;
    public GameObject Joystick;
    public GameObject Steering;

    ParticleSystem PS1, PS2;

    [HideInInspector]
    public int Cars;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
       CurrentLevel = PlayerPrefs.GetInt("Level", 0);
       Instantiate(Level[CurrentLevel]);
    }

    void Start()
    {
        finished = false;
        currentText.text = "MISSION " + (CurrentLevel+1);
        int controlActive = PlayerPrefs.GetInt("Control", 0);
        if (controlActive == 1)
        {
            Dpad.SetActive(true);
        }
        else if (controlActive == 2)
        {
            Joystick.SetActive(true);
        }
        else {
            Steering.SetActive(true);
        }

        PS1 = GameObject.FindGameObjectWithTag("Blast").GetComponent<ParticleSystem>();
        Adcontrol.instance.HideBanner();
    }

    
    void Update()
    {
        if (Cars == 2 && !finished) {
            GameWin();
        }
    
        
    }

    IEnumerator End() {
        yield return new WaitForSeconds(2f);
        GameOverUI.SetActive(true);

    }

    public void GameEnd() {
        finished = true;
         Adcontrol.instance.ShowInterstitial();
        GameoverSound.Play();
        BackgroundSound.Stop();
        RunEmoji();
        StartCoroutine(End());
    }

    public void GameWin() {
        finished = true;
        Adcontrol.instance.ShowInterstitial();
        PS1.Play();
        PlayerPrefs.SetInt("Level", (CurrentLevel + 1));
        StartCoroutine(Win());
       
    }


    IEnumerator Win() {
        yield return new WaitForSeconds(2f);
        GameWinUI.SetActive(true);

    }



    public void Restart() {
        SceneManager.LoadScene("Game");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    void RunEmoji() {
        int RandNum = Random.Range(0, 3);
        Emoji[RandNum].SetActive(true);
        Dpad.SetActive(false);
        Steering.SetActive(false);
    }

}
