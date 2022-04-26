using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public Player player;
    public List<GameObject> obstacleSequences = new List<GameObject>();
    public List<float> obstacleDifficulty = new List<float>();

    private HashSet<Sequence> spawnedSequences = new HashSet<Sequence>();
    private float nextTimeToSpawn = 0;
    private float spawnRate = .5f;
    private int availableDir = 6;
    private Vector3 previousDir = Vector3.zero;
    private int sameDirCounter = 3; // Counts the number of times the same direction is used
    private int previousSequence = -1;
    private int sameSequenceCounter = 3;
    private int seed = 0;

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.state == GameState.Pausing) nextTimeToSpawn += Time.deltaTime;
        if (GameStateManager.state != GameState.Starting
            && GameStateManager.state != GameState.Playing) return;
        if (Time.time >= nextTimeToSpawn)
        {
            // Get a random direction
            int randomDirSeed = Random.Range(0, availableDir);
            Vector3 randomDir = GetRandomDirection(randomDirSeed);
            if (randomDir == previousDir)
            {
                sameDirCounter--;
                if (sameDirCounter == 0) randomDir = GetRandomDirection(randomDirSeed + 1);
            }
            if (randomDir != previousDir)
            {
                previousDir = randomDir;
                sameDirCounter = 3;
            }
            randomDir = randomDir.normalized;

            // Get a random sequence
            Sequence randomSequence;
            int randomSequenceSeed = 0;
            if (GameStateManager.state == GameState.Playing)
            {
                randomSequenceSeed = Random.Range(1, obstacleSequences.Count);
                if (randomSequenceSeed == previousSequence)
                {
                    sameSequenceCounter--;
                    if (sameSequenceCounter <= 0) randomSequenceSeed += 1;
                    if (randomSequenceSeed >= obstacleSequences.Count) randomSequenceSeed = 0;
                }
                if (randomSequenceSeed != previousSequence)
                {
                    previousSequence = randomSequenceSeed;
                    sameSequenceCounter = 3;
                }

                // If the sequence is too difficult, choose from the first two beginner sequences
                float sequenceDifficulty = obstacleDifficulty[randomSequenceSeed];
                if (sequenceDifficulty > GameStateManager.difficulty)
                {
                    randomSequenceSeed = Random.Range(1, 3);
                    sameSequenceCounter = 0;
                }
            }

            // Instantiate and initialize the new random sequence
            randomSequence = Instantiate(obstacleSequences[randomSequenceSeed], transform).GetComponent<Sequence>();
            randomSequence.Initialize(randomDir);
            randomSequence.UpdateColors();
            spawnedSequences.Add(randomSequence);

            // Calculate next time to spawn
            // difficultySpawnCoefficient will range from .9 to .5 as game gets harder
            // so walls will go from being spawned when last wall in the sequence is 9/10 the way 
            // through it's path to when the last wall in the sequence is 1/2 through it's path
            float wallSpeed = 4f + (GameStateManager.difficulty / GameStateManager.maxDifficulty) * 4f;
            // sequenceDistance is the total distance that the sequence travels
            float sequenceDistance = 3 * Camera.main.orthographicSize + randomSequence.GetLength();
            float difficultySpawnCoefficient = .9f - (GameStateManager.difficulty / GameStateManager.maxDifficulty) * .5f;
            nextTimeToSpawn = Time.time + difficultySpawnCoefficient * (sequenceDistance / wallSpeed);
        }
    }

    public void DestroyWalls()
    {
        print("Destorying all walls");
        foreach (Sequence sequence in spawnedSequences) Destroy(sequence.gameObject);
        nextTimeToSpawn = Time.time + .5f / spawnRate;
        spawnedSequences.Clear();
    }

    public void DestroySequence(Sequence toRemove)
    {
        spawnedSequences.Remove(toRemove);
        Destroy(toRemove.gameObject);
    }

    public void UpdateColors()
    {
        foreach (Sequence sequence in spawnedSequences) sequence.UpdateColors();
    }

    Vector3 GetRandomDirection(int seed)
    {
        if (seed > availableDir) seed = 0;
        switch (seed)
        {
            case 0: return Vector3.up;
            case 1: return -Vector3.up;
            // case 2: return Vector3.right;
            // case 3: return -Vector3.right;
            case 2: return new Vector3(1, 2, 0);
            case 3: return new Vector3(-1, 2, 0);
            case 4: return new Vector3(1, -2, 0);
            case 5: return new Vector3(-1, -2, 0);
            default: return Vector3.up;
        }
    }
}
