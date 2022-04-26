using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateBlur : MonoBehaviour
{
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void FadeIn()
    {
        StartCoroutine(FadingIn());
    }

    IEnumerator FadingIn()
    {
        float kernelSize = 0f;
        image.material.SetFloat("_Size", kernelSize);
        while (kernelSize < 5.5f)
        {
            kernelSize += .1f;
            image.material.SetFloat("_Size", kernelSize);
            yield return new WaitForSeconds(.001f);
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadingOut());
    }

    IEnumerator FadingOut()
    {
        float kernelSize = 5.5f;
        image.material.SetFloat("_Size", kernelSize);
        while (kernelSize > 0)
        {
            kernelSize -= .1f;
            image.material.SetFloat("_Size", kernelSize);
            yield return new WaitForSeconds(.001f);
        }
    }
}
