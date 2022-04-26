using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Peripherals")]
    public GameStateManager gameManager;
    public AudioManager audio;
    public ScoreCounter score;
    public LivesCounter lives;
    public CameraShake shakeCam;
    public WallSpawner wallSpawner;

    [Header("Balls")]
    public GameObject ball;
    public Ball leftBall;
    public Ball rightBall;

    [Header("Tick")]
    public RectTransform tick;

    [Header("Rotation")]
    public float maxRotSpeed = 300;
    public float rotAcceleration = 600;

    private int numBalls = 2;
    private HashSet<Ball> balls = new HashSet<Ball>();
    private float rotSpeed = 0;
    private float angle = 0;
    private float rotation = 0;
    private int rotDir = 1;
    private bool rotating = false;
    private bool noScore = true;

    // Touch Detection
    private float touchTime = 0;
    private Vector3 touchDown = Vector3.zero;
    private Vector3 touchUp = Vector3.zero;
    private float doubleTapThreshold = .15f;
    private bool tapped = false;

    // Hit detection
    private float hitTime = 0;

    // Ball Track
    private int track = 1;

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.state != GameState.Playing) return;

        maxRotSpeed = 180f + (GameStateManager.difficulty / GameStateManager.maxDifficulty) * 180f;

        if (rotating)
        {
            if (Mathf.Sign(rotSpeed) == rotDir) rotSpeed += rotDir * rotAcceleration * Time.deltaTime;
            else rotSpeed += rotDir * rotAcceleration * 2 * Time.deltaTime;

            if (rotDir == 1) rotSpeed = Mathf.Min(maxRotSpeed, rotSpeed);
            else rotSpeed = Mathf.Max(-maxRotSpeed, rotSpeed);
        }
        else
        {
            rotSpeed = Mathf.Max(0, rotSpeed - rotAcceleration * 2 * Time.deltaTime);
            if (rotDir == 1) rotSpeed = Mathf.Max(0, rotSpeed);
            else rotSpeed = Mathf.Min(0, rotSpeed);
        }

        rotation += Mathf.Abs(rotSpeed * Time.deltaTime);
        tick.anchorMax = new Vector2(1, rotation / 360f);
        if (rotation >= 360f)
        {
            rotation -= 360f;
            score.Increment();
        }
        angle += rotSpeed * Time.deltaTime;
        // print(rotating + " " + angle + " " + rotation + " " + rotSpeed + " " + maxRotSpeed);
        if (Mathf.Abs(angle) >= 360.0f) angle = rotDir * (Mathf.Abs(angle) - 360.0f);

        transform.eulerAngles = new Vector3(0, 0, angle);

        if (Input.GetMouseButtonDown(0))
        {
            print("Mouse down");
            rotating = true;
            touchTime = Time.time;
            touchDown = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            print("Mouse Up");
            rotating = false;
            touchUp = Input.mousePosition;
        }

        if (rotDir == 1 && Input.mousePosition.x >= Screen.width / 2f)
        {
            rotDir = -1;
            noScore = !noScore;
        }
        else if (rotDir == -1 && Input.mousePosition.x < Screen.width / 2f)
        {
            rotDir = 1;
            noScore = !noScore;
        }
    }

    private Vector3 GetBallPosition(int ballNum)
    {
        return Quaternion.Euler(0, 0, (float)(ballNum + 1) / (float)numBalls * 360.0f) * Vector3.right * 2;
    }

    public void HitWall()
    {
        if (Time.time < hitTime) return;
        hitTime = Time.time + .5f;
        wallSpawner.DestroyWalls();
        lives.RemoveLife();
        if (GameStateManager.vibrating) Vibration.Vibrate();
        shakeCam.Shake(.5f, .25f);
        // If numLives == 0 then the game is over...
        if (lives.NumLives() == 0)
        {
            gameManager.FinishGame();
            return;
        }
        audio.Scratch();
        int index = 0;
        foreach (Ball ballx in balls)
        {
            ballx.SetPosition(GetBallPosition(index));
            index++;
        }
    }

    public void CompletedRevolution()
    {
        if (noScore)
        {
            noScore = false;
            return;
        }
        score.Increment();
    }

    public void HitTick()
    {
        // if (Mathf.Abs(rotation) >= 180f)
        // {
        //     rotation = 0f;
        //     score.Increment();
        // }
    }

    public void ResetRotation(bool noRotate = false)
    {
        rotating = false;
        rotSpeed = 0;
        rotDir = 1;
        rotation = 0;
        if (noRotate) return;
        StartCoroutine(ResetRotationSmooth());
    }

    IEnumerator ResetRotationSmooth()
    {
        while (Mathf.Abs(angle) > 0.5f)
        {
            if (GameStateManager.state == GameState.Playing) break;
            if (angle > 0) angle -= maxRotSpeed * Time.deltaTime;
            if (angle < 0) angle += maxRotSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, 0, angle);
            yield return new WaitForSeconds(0.001f);
        }
        angle = 0;
        transform.eulerAngles = Vector3.zero;
    }

    public void Reset()
    {
        rotSpeed = 0;
        rotation = 0;
        rotDir = 1;
        rotating = false;
        noScore = true;
        numBalls = 2;
        track = 1;
        leftBall.SetPosition(new Vector3(-2, 0, 0));
        rightBall.SetPosition(new Vector3(2, 0, 0));

    }

    public void UpdateColors(Color ballColor, Color lightColor)
    {
        leftBall.SetColor(ballColor);
        leftBall.SetTrailColor(ballColor);
        leftBall.SetLightColor(lightColor);
        leftBall.SetBackgroundColor(lightColor);
        rightBall.SetColor(ballColor);
        rightBall.SetTrailColor(ballColor);
        rightBall.SetLightColor(lightColor);
        rightBall.SetBackgroundColor(lightColor);
    }
}
