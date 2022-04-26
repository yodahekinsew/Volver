using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public Leaderboard leaderboard;

    [Header("Score Counting")]
    public TextMeshPro counter;
    public TextMeshProUGUI finalScoreCount;
    public TextMeshProUGUI highScoreCount;
    public GameStateManager gameStateManager;
    private int score = 0;
    private int highscore = 0;

    [Header("Multiplier")]
    public TextMeshPro multiplierIndicator;
    public Animator multiplierAnim;
    private int multiplier = 1;
    private bool multiplying = false;
    private float touchDown = 0;

    [Header("Difficulty")]
    public List<int> difficultySteps;

    [Header("Tick")]
    public Animator tickAnim;

    [Header("Unlockables")]
    public ThemeManager themeManager;
    public DesignManager designManager;
    private HashSet<string> unlockableThemes = new HashSet<string>();
    private HashSet<string> unlockableDesigns = new HashSet<string>();
    private Dictionary<string, int> unlockableCosts = new Dictionary<string, int>();

    void Start()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            highscore = PlayerPrefs.GetInt("highscore");
        }
        else PlayerPrefs.SetInt("highscore", 0);
        counter.text = "" + highscore;

        // Set up unlockables
        foreach (string theme in themeManager.themes)
        {
            if (PlayerPrefs.GetInt(theme) == 0)
            {
                if (highscore >= themeManager.GetCost(theme))
                {
                    PlayerPrefs.SetInt(theme, 1);
                    continue;
                }
                unlockableThemes.Add(theme);
                unlockableCosts.Add(theme, themeManager.GetCost(theme));
            }
        }
        foreach (string design in designManager.designs)
        {
            if (PlayerPrefs.GetInt(design) == 0)
            {
                if (highscore >= designManager.GetCost(design))
                {
                    PlayerPrefs.SetInt(design, 1);
                    continue;
                }
                unlockableDesigns.Add(design);
                unlockableCosts.Add(design, designManager.GetCost(design));
            }
        }
    }

    void Update()
    {
        if (multiplying)
        {
            int newMultiplier = 1 + (int)Mathf.Floor((Time.time - touchDown) / 5);
            if (multiplier != newMultiplier)
            {
                if (multiplier == 1)
                {
                    multiplierIndicator.text = "x" + newMultiplier;
                    multiplierAnim.SetTrigger("Appear");
                }
                else
                {
                    multiplierAnim.SetTrigger("Grow");
                    StartCoroutine(UpdateMultiplier(newMultiplier));
                }
            }
            multiplier = newMultiplier;
        }
        if (Input.GetMouseButtonDown(0))
        {
            multiplying = true;
            touchDown = Time.time;
        }
        if (Input.GetMouseButtonUp(0) || GameStateManager.state != GameState.Playing)
        {
            multiplying = false;
            if (multiplier > 1) multiplierAnim.SetTrigger("Disappear");
            multiplier = 1;
        }
    }

    IEnumerator UpdateMultiplier(int newMultiplier)
    {
        yield return new WaitForSeconds(.25f);
        multiplierIndicator.text = "x" + newMultiplier;
    }

    public void ShowHighscore()
    {
        counter.text = "" + highscore;
        score = 0;
    }

    public void Finalize()
    {
        finalScoreCount.text = "" + score;
        if (score > highscore)
        {
            highScoreCount.text = "" + score + " HIGH";
            highscore = score;
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
        }
        else highScoreCount.text = "" + highscore + " HIGH";
        multiplier = 1;
        multiplierIndicator.text = "";
        leaderboard.UpdateLeaderboard(highscore);
    }

    public void ResetScore()
    {
        score = 0;
        counter.text = "0";
    }

    public void Increment(int amount = 1)
    {
        tickAnim.SetTrigger("Hit");
        if (multiplying) amount = multiplier * amount;
        if (GameStateManager.vibrating) Vibration.VibratePop();
        // if (amount > 1) StartCoroutine(Incrementing(amount));
        // else
        // {
        // }
        score += amount;
        counter.text = "" + score;

        // Handle difficulty adjustments based on score
        if (GameStateManager.difficulty < GameStateManager.maxDifficulty)
        {
            print("Current Difficulty Step: " + Mathf.Floor(GameStateManager.difficulty));
            int currentDifficulty = (int)Mathf.Floor(GameStateManager.difficulty);
            float currentStep = difficultySteps[currentDifficulty];
            float nextStep = difficultySteps[currentDifficulty + 1];
            // float difficultyChange = ((float)score - currentStep) / (nextStep - currentStep);
            float difficultyChange = ((float)amount) / (nextStep - currentStep);
            print("Difficulty Change: " + currentStep + " " + currentDifficulty + " " + score + " " + difficultyChange);
            gameStateManager.SetDifficulty(GameStateManager.difficulty + difficultyChange);
        }

        // Check if an unlockable is unlocked
        List<string> unlocked = new List<string>();
        foreach (string theme in unlockableThemes)
        {
            if (score >= unlockableCosts[theme])
            {
                PlayerPrefs.SetInt(theme, 1);
                themeManager.UnlockTheme(theme);
                unlocked.Add(theme);
            }
        }
        foreach (string theme in unlocked)
        {
            unlockableCosts.Remove(theme);
            unlockableThemes.Remove(theme);
        }

        unlocked = new List<string>();
        foreach (string design in unlockableDesigns)
        {
            if (score >= unlockableCosts[design])
            {
                PlayerPrefs.SetInt(design, 1);
                designManager.UnlockDesign(design);
                unlocked.Add(design);
            }
        }
        foreach (string design in unlocked)
        {
            unlockableDesigns.Remove(design);
            unlockableThemes.Remove(design);
        }

        print("Current Difficulty: " + GameStateManager.difficulty);
    }

    public void CountDown()
    {
        StartCoroutine(CountingDown());
    }

    IEnumerator Incrementing(int amount)
    {
        for (int i = 0; i < amount - 1; i++)
        {
            score++;
            counter.text = "" + score;
            yield return new WaitForSeconds(.5f);
        }
        score++;
        counter.text = "" + score;
    }

    IEnumerator CountingDown()
    {
        int count = highscore;
        float timex = Time.time;
        while (count > 0)
        {
            count -= (int)Mathf.Ceil(highscore * Time.deltaTime);
            counter.text = "" + count;
            timex = Time.time;
            yield return new WaitForSeconds(0f);
        }
        counter.text = "" + 0;
    }
}
