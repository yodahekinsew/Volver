using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public float speed;

    private int dir = 1;

    void Update()
    {
        transform.position += dir * Vector3.right * speed * Time.deltaTime;
        if (dir == 1 && transform.position.x > Camera.main.orthographicSize / 2f) dir = -1;
        if (dir == -1 && transform.position.x < -Camera.main.orthographicSize / 2f) dir = 1;
    }
}
