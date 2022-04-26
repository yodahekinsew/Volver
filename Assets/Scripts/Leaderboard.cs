using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    private const string LEADERBOARD_ID = "Volver.HighScore";

    private bool loggedIn;

    private void Start()
    {
        AuthenticateUser();
    }

    private void AuthenticateUser()
    {
        Social.localUser.Authenticate((bool successful) =>
        {
            loggedIn = successful;
        });
    }

    public void UpdateLeaderboard(int playerScore)
    {
        if (!loggedIn) return;

        Social.ReportScore(playerScore, LEADERBOARD_ID, (bool successful) => { });
    }

    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
}
