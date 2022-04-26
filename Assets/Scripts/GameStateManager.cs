using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Opening,
    Starting,
    Playing,
    Pausing,
    Continuing,
    Finishing
}

public class GameStateManager : MonoBehaviour
{
    public static GameState state = GameState.Opening;
    public static bool vibrating = true;
    public static float difficulty = 0f;
    public static float maxDifficulty = 10f;

    [Header("Game Components")]
    public Player player;
    public WallSpawner wallSpawner;
    public GemSpawner gemSpawner;
    public AudioManager audio;
    public ScoreCounter score;
    public LivesCounter lives;
    public GameObject tick;

    [Header("Animators")]
    public Animator playerAnim;
    public Animator uiAnim;

    [Header("UI Components")]
    public GameObject startingUI;
    public GameObject playingUI;
    public GameObject finishedUI;
    public GameObject settingsUI;
    public GameObject upgradeUI;
    public GameObject tutorialUI;
    public GameObject settingsPause;

    [Header("Unlockables")]
    public ThemeManager themeManager;
    public DesignManager designManager;
    public GameObject themesHolder;
    public GameObject designsHolder;

    [Header("Buttons")]
    public GameObject playButton;
    public GameObject upgradeToggle;

    // [Header("In-App Purchasing")]
    // public Purchaser purchaser;

    public delegate void Delayed();

    private bool showingThemes = false;
    private bool showingDesigns = false;
    private bool showingSettings = false;
    private bool showingTutorial = false;

    private bool usedSecondChance = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Vibration.Init();
        ChangeAppIcon.Init();
        Opening();
    }

    void Start()
    {
        startingUI.SetActive(true);
        playingUI.SetActive(true);
        finishedUI.SetActive(false);
        tutorialUI.SetActive(false);
        upgradeUI.SetActive(false);
        tick.SetActive(false);
        settingsPause.SetActive(false);

        // Set up music
        if (!PlayerPrefs.HasKey("music")) PlayerPrefs.SetInt("music", 1);
        if (PlayerPrefs.GetInt("music") == 0)
        {
            audio.Stop();
            settingsUI.transform.Find("Music/Disabled").gameObject.SetActive(true);
        }
        else audio.Open();

        // Set up vibrating
        if (!PlayerPrefs.HasKey("vibration")) PlayerPrefs.SetInt("vibration", 1);
        else if (PlayerPrefs.GetInt("vibration") == 0)
        {
            vibrating = false;
            settingsUI.transform.Find("Vibration/Disabled").gameObject.SetActive(true);
        }

        // Set up upgrade
        if (!PlayerPrefs.HasKey("upgrade")) PlayerPrefs.SetInt("upgrade", 0);
        else if (PlayerPrefs.GetInt("upgrade") == 1) upgradeToggle.SetActive(false);
        // {
        //     upgradeToggle.SetActive(false);
        //     developerToggle.SetActive(true);
        // }

        // First time playing
        if (!PlayerPrefs.HasKey("played")) showingTutorial = true;
    }

    public void SetDifficulty(float newDifficulty)
    {
        difficulty = newDifficulty;
    }

    public void IncreaseDifficulty()
    {
        difficulty += 1;
        difficulty = Mathf.Clamp(0f, difficulty, maxDifficulty);
        print("Increased the difficulty to " + difficulty);
    }

    public void DecreaseDifficulty()
    {
        difficulty -= 1;
        difficulty = Mathf.Clamp(0f, difficulty, maxDifficulty);
        print("Decreased the difficulty to " + difficulty);
    }

    public void StartPlaying()
    {
        if (showingSettings) uiAnim.SetTrigger("HideSettings");
        if (showingThemes)
        {
            uiAnim.SetTrigger("HideThemes");
            themesHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
        }
        if (showingDesigns)
        {
            uiAnim.SetTrigger("HideDesigns");
            designsHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
        }
        showingSettings = false;
        showingThemes = false;
        showingDesigns = false;
        difficulty = 0;

        tick.SetActive(true);
        score.CountDown();

        if (showingTutorial)
        {
            tutorialUI.SetActive(true);
            uiAnim.SetTrigger("PlayTutorial");
            playerAnim.SetTrigger("PlayTutorial");
            StartCoroutine(WaitToCall(() =>
            {
                tutorialUI.SetActive(false);
                playerAnim.enabled = false;
                state = GameState.Playing;
                PlayerPrefs.SetInt("played", 1);
                showingTutorial = false;
            }, 16f));
        }
        else
        {
            playerAnim.enabled = false;
            uiAnim.SetTrigger("StartPlaying");
            StartCoroutine(WaitToCall(() =>
            {
                state = GameState.Playing;
            }, 1f));
        }
    }


    public void GoToStart()
    {
        usedSecondChance = false;
        lives.Reset();
        playerAnim.enabled = true;
        playerAnim.SetTrigger("GoToStart");
        uiAnim.SetTrigger("GoToStart");
        finishedUI.GetComponent<AnimateBlur>().FadeOut();
        audio.DisableLowPass();
        score.ShowHighscore();
        StartCoroutine(WaitToCall(() =>
        {
            finishedUI.SetActive(false);
            state = GameState.Starting;
        }, 1.5f));
    }

    public void Opening()
    {
        playerAnim.SetTrigger("Opening");
        uiAnim.SetTrigger("Opening");
        StartCoroutine(WaitToCall(() =>
        {
            if (state == GameState.Opening) state = GameState.Starting;
        }, 5f));
    }

    public void ContinueGame()
    {
        // Continuing the same game
        lives.AddLife();
        DecreaseDifficulty();
        usedSecondChance = true;

        // Moving from finished to play
        tick.SetActive(true);
        uiAnim.SetTrigger("PlayAgain");
        finishedUI.GetComponent<AnimateBlur>().FadeOut();
        audio.DisableLowPass();
        StartCoroutine(WaitToCall(() =>
        {
            finishedUI.transform.Find("SECOND CHANCE").gameObject.SetActive(false);
            finishedUI.SetActive(false);
            state = GameState.Playing;
        }, 1.5f));
    }

    public void PlayAgain()
    {
        // Resetting the game
        difficulty = 0;
        score.ResetScore();
        lives.Reset();
        usedSecondChance = false;

        // Moving from finished to play
        tick.SetActive(true);
        uiAnim.SetTrigger("PlayAgain");
        finishedUI.GetComponent<AnimateBlur>().FadeOut();
        audio.DisableLowPass();
        state = GameState.Playing;
        StartCoroutine(WaitToCall(() =>
        {
            finishedUI.SetActive(false);
        }, 1.5f));
    }

    public void FinishGame()
    {
        state = GameState.Finishing;

        // Reset game now that it is finished
        player.ResetRotation();
        player.Reset();
        score.Finalize();

        // Open up the finished ui
        finishedUI.SetActive(true);
        finishedUI.GetComponent<AnimateBlur>().FadeIn();
        audio.EnableLowPass();
        uiAnim.SetTrigger("Finish");
        if (PlayerPrefs.GetInt("upgrade") == 0 && !usedSecondChance) finishedUI.transform.Find("SECOND CHANCE").gameObject.SetActive(true);
        StartCoroutine(WaitToCall(() =>
        {
            tick.SetActive(false);
        }, 1.5f));
    }

    public void ShowUpgrade()
    {
        if (showingSettings) uiAnim.SetTrigger("HideSettings");
        if (showingThemes)
        {
            uiAnim.SetTrigger("HideThemes");
            themesHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
        }
        if (showingDesigns)
        {
            uiAnim.SetTrigger("HideDesigns");
            designsHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
        }
        showingSettings = false;
        showingThemes = false;
        showingDesigns = false;
        upgradeUI.SetActive(true);
        upgradeUI.GetComponent<AnimateBlur>().FadeIn();
        uiAnim.SetTrigger("ShowUpgrade");
    }

    // Upgrade
    // public void BuyUpgrade()
    // {
    //     purchaser.BuyUpgrade();
    // }

    // public void RestoreUpgrade()
    // {
    //     purchaser.RestorePurchases();
    // }

    public void HideUpgrade()
    {
        uiAnim.SetTrigger("HideUpgrade");
        upgradeUI.GetComponent<AnimateBlur>().FadeOut();
        StartCoroutine(WaitToCall(() =>
        {
            upgradeUI.SetActive(false);
        }, 1f));
    }

    public void UpgradePlayer()
    {
        upgradeUI.transform.Find("UPGRADED").gameObject.SetActive(true);
        StartCoroutine(WaitToCall(() =>
        {
            upgradeUI.transform.Find("NOT UPGRADED").gameObject.SetActive(false);
        }, 1f));
        uiAnim.SetTrigger("BuyUpgrade");
        PlayerPrefs.SetInt("upgrade", 1);
        PlayerPrefs.Save();
        themeManager.UnlockAllThemes();
        designManager.UnlockAllDesigns();
        lives.SetNumLives(2);
        upgradeToggle.SetActive(false);
    }

    // Settings
    public void ToggleVibration()
    {
        bool vibrationEnabled = PlayerPrefs.GetInt("vibration") == 1;
        if (vibrationEnabled)
        {
            settingsUI.transform.Find("Vibration/Disabled").gameObject.SetActive(true);
            vibrating = false;
            PlayerPrefs.SetInt("vibration", 0);
        }
        else
        {
            settingsUI.transform.Find("Vibration/Disabled").gameObject.SetActive(false);
            vibrating = true;
            PlayerPrefs.SetInt("vibration", 1);
        }
    }

    public void ToggleMusic()
    {
        bool musicEnabled = PlayerPrefs.GetInt("music") == 1;
        if (musicEnabled)
        {
            settingsUI.transform.Find("Music/Disabled").gameObject.SetActive(true);
            audio.Stop();
            PlayerPrefs.SetInt("music", 0);
        }
        else
        {
            settingsUI.transform.Find("Music/Disabled").gameObject.SetActive(false);
            audio.Play();
            PlayerPrefs.SetInt("music", 1);
        }
    }

    public void ToggleSettings()
    {

        if (showingSettings)
        {
            uiAnim.SetTrigger("HideSettings");
            //If paused then play the game
            if (state == GameState.Pausing)
            {
                settingsPause.GetComponent<Button>().interactable = false;
                settingsPause.GetComponent<AnimateBlur>().FadeOut();
                StartCoroutine(WaitToCall(() =>
                {
                    settingsPause.SetActive(false);
                }, 1f));
                audio.DisableLowPass();
                state = GameState.Playing;
            }
        }
        else
        {
            uiAnim.SetTrigger("ShowSettings");
            //If playing then pause the game
            if (state == GameState.Playing)
            {
                settingsPause.SetActive(true);
                settingsPause.GetComponent<Button>().interactable = true;
                settingsPause.GetComponent<AnimateBlur>().FadeIn();
                audio.EnableLowPass();
                state = GameState.Pausing;
            }
        }
        showingSettings = !showingSettings;
    }

    public void ToggleThemes()
    {
        if (showingDesigns)
        {
            uiAnim.SetTrigger("HideDesigns");
            designsHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
            showingDesigns = false;
        }

        if (showingThemes)
        {
            uiAnim.SetTrigger("HideThemes");
            themesHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
        }
        else uiAnim.SetTrigger("ShowThemes");
        showingThemes = !showingThemes;
    }

    public void ToggleDesigns()
    {
        if (showingThemes)
        {
            uiAnim.SetTrigger("HideThemes");
            themesHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
            showingThemes = false;
        }

        if (showingDesigns)
        {
            uiAnim.SetTrigger("HideDesigns");
            designsHolder.GetComponent<AnimateScrollRect>().ResetHorizontal();
        }
        else uiAnim.SetTrigger("ShowDesigns");
        showingDesigns = !showingDesigns;
    }

    IEnumerator WaitToCall(Delayed functionToCall, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        functionToCall();
    }

    private void Onse(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Time.timeScale = 0;
        }
        else Time.timeScale = 1;
    }
}
