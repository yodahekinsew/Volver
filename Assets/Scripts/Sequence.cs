using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    public List<Wall> walls = new List<Wall>();
    private int numDestroyed = 0;
    private float length = 0;

    public void Initialize(Vector3 wallDirection)
    {
        if (walls.Count > 1) length = Vector3.Distance(walls[0].transform.localPosition, walls[walls.Count - 1].transform.localPosition);
        // General wall initializations
        foreach (Wall wall in walls) wall.Initialize(wallDirection);
    }

    public void UpdateColors()
    {
        Component[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sprite in sprites) sprite.color = ThemeManager.colors[0];
    }

    public float GetLength()
    {
        return length;
    }

    public void DestroyWall()
    {
        // Destroy(walls[numDestroyed].gameObject);
        numDestroyed++;
        if (numDestroyed == walls.Count)
        {
            transform.parent.gameObject.GetComponent<WallSpawner>().DestroySequence(this);
        }
    }
}
