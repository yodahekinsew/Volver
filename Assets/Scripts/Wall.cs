using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    Starter,
    LeftLong,
    RightLong,
    Gap,
    DoubleGap,
    RightSkewGap,
    LeftSkewGap,
    Rotating,
    RightSkewRotate,
    LeftSkewRotate,
    Moving
}

public class Wall : MonoBehaviour
{
    public WallType type;
    public bool usesDistance = false;

    private bool initialized = false;
    private float speed = 5;
    private float rotSpeed = 180;
    private Vector3 wallDir = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private int side = 1;
    private float lateralMovement = 0;
    private Vector3 destroyPosition = Vector3.zero;

    private bool isTrick = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!initialized) return;
        if (GameStateManager.state != GameState.Playing &&
            GameStateManager.state != GameState.Starting) return;

        speed = 4f + (GameStateManager.difficulty / GameStateManager.maxDifficulty) * 4f;
        rotSpeed = 180f + (GameStateManager.difficulty / GameStateManager.maxDifficulty) * 180f;
        if (type == WallType.LeftSkewRotate || type == WallType.RightSkewRotate) rotSpeed *= .5f;

        transform.position += wallDir * speed * Time.deltaTime;
        switch (type)
        {
            case WallType.LeftLong:
            case WallType.RightLong:
                break;
            case WallType.LeftSkewRotate:
                rotation.z += side * .5f * rotSpeed * Time.deltaTime;
                transform.eulerAngles = rotation;
                break;
            case WallType.RightSkewRotate:
                rotation.z += side * .5f * rotSpeed * Time.deltaTime;
                transform.eulerAngles = rotation;
                break;
            case WallType.Rotating:
                rotation.z += side * rotSpeed * Time.deltaTime;
                transform.eulerAngles = rotation;
                break;
            case WallType.Moving:
                lateralMovement += side * .9f * speed * Time.deltaTime;
                transform.Find("WallBlock").localPosition += side * Vector3.right * .9f * speed * Time.deltaTime;
                if (side == 1 && lateralMovement > Camera.main.orthographicSize / 2f) side = -1;
                if (side == -1 && lateralMovement < -Camera.main.orthographicSize / 2f) side = 1;
                break;
        }

        // This "trick" is reserved for Long blocks that slide out
        if (isTrick && transform.position.magnitude < 8)
        {
            isTrick = false;
            switch (type)
            {
                case WallType.LeftLong:
                case WallType.RightLong:
                    StartCoroutine(TrickLong());
                    break;
                case WallType.LeftSkewGap:
                case WallType.RightSkewRotate:
                    StartCoroutine(TrickSkewGap());
                    break;
            }
        }

        if ((destroyPosition.y < 0 && transform.position.y < destroyPosition.y)
            || (destroyPosition.y > 0 && transform.position.y > destroyPosition.y))
        {
            initialized = false;
            transform.parent.gameObject.GetComponent<Sequence>().DestroyWall();
        }
    }

    IEnumerator TrickLong()
    {
        Transform block = transform.Find("WallBlock");
        Vector3 targetPos = -1f * block.localPosition;
        while (Vector3.Distance(block.localPosition, targetPos) >= .1f)
        {
            block.localPosition = Vector3.Lerp(block.localPosition, targetPos, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        block.localPosition = targetPos;
    }

    IEnumerator TrickSkewGap()
    {
        Transform leftBlock = transform.Find("LeftWall");
        Transform rightBlock = transform.Find("RightWall");
        Vector3 leftTargetPos = leftBlock.localPosition;
        Vector3 rightTargetPos = rightBlock.localPosition;
        if (type == WallType.LeftSkewGap)
        {
            leftTargetPos += new Vector3(4, 0, 0);
            rightTargetPos += new Vector3(4, 0, 0);
        }
        else
        {
            leftTargetPos += new Vector3(-4, 0, 0);
            rightTargetPos += new Vector3(-4, 0, 0);
        }

        while (Vector3.Distance(leftBlock.localPosition, leftTargetPos) >= .1f)
        {
            leftBlock.localPosition = Vector3.Lerp(leftBlock.localPosition, leftTargetPos, speed * Time.deltaTime);
            rightBlock.localPosition = Vector3.Lerp(rightBlock.localPosition, rightTargetPos, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        leftBlock.localPosition = leftTargetPos;
        rightBlock.localPosition = rightTargetPos;
    }

    public void Initialize(Vector3 wallDirection)
    {
        // General Wall intializations
        float angle = Vector3.SignedAngle(transform.up, -wallDirection, transform.forward);
        transform.RotateAround(Vector3.zero, Vector3.forward, angle);
        side = Random.Range(0, 2) == 1 ? 1 : -1;
        wallDir = wallDirection;
        rotation = transform.eulerAngles;
        transform.localPosition += -1.5f * Camera.main.orthographicSize * wallDir;
        destroyPosition = wallDir * 1.5f * Camera.main.orthographicSize;

        // Wall type specific intializations
        float difficultyCoeff = GameStateManager.difficulty / GameStateManager.maxDifficulty;
        if (type == WallType.RightSkewRotate) side = 1;
        if (type == WallType.LeftSkewRotate) side = -1;
        switch (type)
        {
            case WallType.RightSkewRotate:
            case WallType.LeftSkewRotate:
            case WallType.Rotating:
                transform.eulerAngles += new Vector3(0, 0, side * 45);
                rotation = transform.eulerAngles;
                break;
            case WallType.LeftSkewGap:
            case WallType.RightSkewGap:
                isTrick = Mathf.Floor(Random.Range(0, 4 + (1 - difficultyCoeff) * 16)) == 0;
                break;
            case WallType.LeftLong:
                isTrick = Mathf.Floor(Random.Range(0, 4 + (1 - difficultyCoeff) * 16)) == 0;
                break;
            case WallType.RightLong:
                isTrick = Mathf.Floor(Random.Range(0, 4 + (1 - difficultyCoeff) * 16)) == 0;
                break;
        }

        initialized = true;
    }
}
