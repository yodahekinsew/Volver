using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSrc;
    public Animator anim;

    [Header("Low Pass Filter")]
    public AudioLowPassFilter lowPassFilter;
    public float maxFrequency;
    public float targetFrequency;

    // Start is called before the first frame update
    void Start()
    {
        lowPassFilter.cutoffFrequency = maxFrequency;
    }

    public void Stop()
    {
        // audioSrc.Stop();
        anim.enabled = false;
        audioSrc.volume = 0;
    }

    public void Play()
    {
        // audioSrc.Play();
        audioSrc.volume = .75f;
        anim.enabled = true;
    }

    public void Open()
    {
        anim.SetTrigger("Open");
    }

    public void Scratch()
    {
        anim.SetTrigger("Scratch");
    }

    public void EnableLowPass()
    {
        StartCoroutine(EnablingLowPass());
    }

    IEnumerator EnablingLowPass()
    {
        // lowPassFilter.enabled = true;
        float frequency = maxFrequency;
        while (frequency > targetFrequency)
        {
            frequency -= (maxFrequency - targetFrequency) * Time.unscaledDeltaTime;
            lowPassFilter.cutoffFrequency = frequency;
            yield return new WaitForSeconds(0f);
        }
        lowPassFilter.cutoffFrequency = targetFrequency;
    }

    public void DisableLowPass()
    {
        StartCoroutine(DisablingLowPass());
    }

    IEnumerator DisablingLowPass()
    {
        float frequency = targetFrequency;
        while (frequency < maxFrequency)
        {
            frequency += (maxFrequency - targetFrequency) * Time.unscaledDeltaTime;
            lowPassFilter.cutoffFrequency = frequency;
            yield return new WaitForSeconds(0f);
        }
        lowPassFilter.cutoffFrequency = maxFrequency;
        // lowPassFilter.enabled = false;
    }
}
