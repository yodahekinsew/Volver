// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Advertisements;

// public class InterstitialAdsPlayer : MonoBehaviour
// {

// #if UNITY_IOS
//         private string gameId = "3874652";
// #else
//     private string gameId = "3874653";
// #endif

//     private bool testMode = true;
//     private int playCount = 0;
//     private int numCounts = 1;

//     void Start()
//     {
//         // Initialize the Ads service:
//         Advertisement.Initialize(gameId, testMode);
//     }

//     public void TryPlay()
//     {
//         playCount++;
//         if (playCount % numCounts == 0 && Advertisement.IsReady())
//         {
//             Advertisement.Show();
//             playCount = 0;
//         }
//     }

// }
