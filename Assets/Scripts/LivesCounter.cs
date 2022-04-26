using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesCounter : MonoBehaviour
{
    // public Sprite donut;
    // public Sprite cross;

    // [Header("Lives")]
    // public Image firstLife;
    // public Image secondLife;
    // public Image thirdLife;

    // [Header("Shadows")]
    // public Image firstLifeShadow;
    // public Image secondLifeShadow;
    // public Image thirdLifeShadow;

    private int numLives = 1;

    void Start()
    {
        print("Is ugpraded? " + PlayerPrefs.GetInt("upgrade"));
        if (PlayerPrefs.GetInt("upgrade") == 1) numLives = 2;
        // firstLife.sprite = donut;
        // secondLife.sprite = donut;
        // thirdLife.sprite = donut;
        // firstLifeShadow.sprite = donut;
        // secondLifeShadow.sprite = donut;
        // thirdLifeShadow.sprite = donut;
    }

    public void Reset()
    {
        numLives = 1;
        // firstLife.sprite = donut;
        // secondLife.sprite = donut;
        // thirdLife.sprite = donut;
        // firstLifeShadow.sprite = donut;
        // secondLifeShadow.sprite = donut;
        // thirdLifeShadow.sprite = donut;
    }

    public void RemoveLife()
    {
        numLives--;
        // switch (numLives)
        // {
        //     case 2:
        //         firstLife.sprite = cross;
        //         firstLifeShadow.sprite = cross;
        //         break;
        //     case 1:
        //         secondLife.sprite = cross;
        //         secondLifeShadow.sprite = cross;
        //         break;
        //     case 0:
        //         thirdLife.sprite = cross;
        //         thirdLifeShadow.sprite = cross;
        //         break;
        // }
    }

    public int NumLives()
    {
        return numLives;
    }

    public void SetNumLives(int lives)
    {
        numLives = lives;
    }

    public void AddLife()
    {
        // switch (numLives)
        // {
        //     case 2:
        //         firstLife.sprite = donut;
        //         firstLifeShadow.sprite = donut;
        //         break;
        //     case 1:
        //         secondLife.sprite = donut;
        //         secondLifeShadow.sprite = donut;
        //         break;
        //     case 0:
        //         thirdLife.sprite = donut;
        //         thirdLifeShadow.sprite = donut;
        //         break;
        // }
        numLives++;
    }
}
