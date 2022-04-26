using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public SpriteRenderer sprite;
    public HardLight2D light;
    public TrailRenderer trail;
    public SpriteRenderer background;

    private Player player;
    private Vector3 goalPosition;
    private bool moving = false;
    private bool rotating = false;
    private float ballSpeed = 7.5f;
    private int tickSide = 0;

    void Start()
    {
        player = transform.parent.gameObject.GetComponent<Player>();
        // player = GameObject.Find("/VOLVER/Player").GetComponent<Player>();
    }

    void Update()
    {
        if (moving)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, goalPosition, ballSpeed * Time.deltaTime);
            if (transform.localPosition == goalPosition) moving = false;
            light.transform.localPosition = transform.localPosition;
        }
    }

    public void SetColor(Color newColor)
    {
        sprite.color = newColor;
    }

    public void SetLightIntensity(float newIntensity)
    {
        light.Intensity = newIntensity;
    }

    public void SetLightColor(Color newColor)
    {
        light.Color = newColor;
    }

    public void SetTrailColor(Color newColor)
    {
        trail.startColor = newColor;
        trail.endColor = newColor;
    }

    public void SetBackgroundColor(Color newColor)
    {
        background.color = newColor;
    }

    public void SetPosition(Vector3 newPosition)
    {
        moving = true;
        goalPosition = newPosition;
    }

    public void UpdateTrail(GameObject newTrail)
    {
        Destroy(trail.gameObject);
        trail = Instantiate(newTrail, transform).GetComponent<TrailRenderer>();
    }

    public void DisableTrail()
    {
        trail.enabled = false;
    }

    public void EnableTrail()
    {
        trail.enabled = true;
    }

    public void EnableRotation()
    {
        rotating = true;
    }

    public void DisableRotation()
    {
        rotating = false;
        transform.eulerAngles = Vector3.zero;
    }

    public void SetSprite(Sprite newSprite)
    {
        sprite.sprite = newSprite;
        background.sprite = newSprite;
    }

    public void Rotate(int dir)
    {
        if (dir == -1) transform.eulerAngles -= new Vector3(0, 0, 180);
        else transform.eulerAngles += new Vector3(0, 0, 180);
    }

    public void SetBackgroundSprite(Sprite newSprite)
    {
        background.sprite = newSprite;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "WallBlock":
                player.HitWall();
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Tick":
                player.HitTick();
                break;
        }
    }
}
