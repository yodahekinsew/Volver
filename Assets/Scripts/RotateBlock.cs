using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBlock : MonoBehaviour
{
    public float rotSpeed = 360;

    private Vector3 rotation = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        rotation.z += rotSpeed * Time.deltaTime;
        transform.eulerAngles = rotation;

    }
}
