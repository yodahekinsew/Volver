using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void Shake(float shakeStrength, float shakeTime)
    {
        StartCoroutine(Shaking(shakeStrength, shakeTime));
    }

    IEnumerator Shaking(float shakeStrength, float shakeTime)
    {
        Vector3 originPosition = transform.position;
        float endTime = Time.time + shakeTime;
        while (Time.time < endTime)
        {
            Vector3 randomPosition = Random.insideUnitCircle * shakeStrength;
            randomPosition.z = transform.position.z;
            transform.position = randomPosition;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = originPosition;
    }
}
