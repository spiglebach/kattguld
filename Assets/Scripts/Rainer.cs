using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rainer : MonoBehaviour {
    [Header("Spawnable objects")]
    [SerializeField] private GameObject[] treasurePrefabs;
    [SerializeField] private GameObject[] hazardPrefabs;

    [Header("Spawning parameters")]
    [SerializeField] private int minSpawnCooldownInSteps = 2;
    [SerializeField] private int maxSpawnCooldownInSteps = 4;
    [SerializeField] private int minSpawnHeight = 4;
    [SerializeField] private int maxSpawnHeight = 6;
    
    private int stepsUntilNextSpawn = 2;
    private HashSet<PlatformBlock> platformBlocks;
    private List<PlatformBlock> platformsWithFallingObject = new List<PlatformBlock>();
    
    void Start() {
        platformBlocks = new HashSet<PlatformBlock>(FindObjectsOfType<PlatformBlock>());
    }

    private void Spawn() {
        // todo randomize hazard and treasure spawning
        if (treasurePrefabs == null || treasurePrefabs.Length <= 0) {
            return;
        }

        var platformBlock = platformBlocks.ElementAt(Random.Range(0, platformBlocks.Count));
        int spawnHeight = Random.Range(minSpawnHeight, maxSpawnHeight + 1);
        var spawnedObject = Instantiate(
            treasurePrefabs[Random.Range(0, treasurePrefabs.Length)],
            platformBlock.transform.position + new Vector3(0, spawnHeight, 0),
            Random.rotation);
        var fallingObject = spawnedObject.GetComponent<FallingObject>();
        if (fallingObject) {
            platformBlock.SetFallingObject(fallingObject);
            platformsWithFallingObject.Add(platformBlock);
        }
    }

    public void Fall() {
        var finished = new List<PlatformBlock>();
        foreach (var platformBlock in platformsWithFallingObject) {
            platformBlock.Fall();
            if (!platformBlock.IsObjectFallingAbove()) {
                finished.Add(platformBlock);
            }
        }
        platformsWithFallingObject.RemoveAll(block => finished.Contains(block));
        stepsUntilNextSpawn--;
        if (stepsUntilNextSpawn <= 0) {
            Spawn();
            stepsUntilNextSpawn = Random.Range(minSpawnCooldownInSteps, maxSpawnCooldownInSteps + 1);
        }
    }
}