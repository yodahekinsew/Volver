using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    public Vector3 originScale = new Vector3(1, 1, 1);
    public float pulseStrength = 1.25f;
    private float pulseRate = 120f; // in BPM
    private Vector3 goalScale;
    private float pulseDistance = 0;
    // Start is called before the first frame update
    void Start()
    {
        // originScale = transform.localScale;
        goalScale = originScale * pulseStrength;
        pulseDistance = Vector3.Distance(originScale, goalScale);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.state == GameState.Pausing) return;
        transform.localScale = Vector3.MoveTowards(transform.localScale, goalScale, 4 * 120f / pulseRate * pulseDistance * Time.deltaTime);
        if (Vector3.Distance(transform.localScale, goalScale) < .001f)
        {
            if (goalScale == originScale) goalScale = originScale * pulseStrength;
            else goalScale = originScale;
        }
    }
}
