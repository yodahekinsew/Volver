using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateScrollRect : MonoBehaviour
{
    public ScrollRect scroll;

    private bool resettingHorizontal = false;
    private float horizontalPos = 0;

    void Update()
    {
        if (resettingHorizontal)
        {
            scroll.horizontalNormalizedPosition -= horizontalPos * 2 * Time.deltaTime;
            if (scroll.horizontalNormalizedPosition <= 0)
            {
                scroll.horizontalNormalizedPosition = 0;
                horizontalPos = 0;
                resettingHorizontal = false;
            }
        }
    }

    public void ResetHorizontal()
    {
        resettingHorizontal = true;
        horizontalPos = scroll.horizontalNormalizedPosition;
    }
}
