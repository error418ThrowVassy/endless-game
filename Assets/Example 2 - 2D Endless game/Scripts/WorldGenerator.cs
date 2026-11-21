using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] GameObject[] chunkPrefabs;

    [Header("Spawning")]
    [SerializeField] float chunkHeight = 12f;
    [SerializeField] float spawnAheadDistance = 24f;
    [SerializeField] float despawnDistance = 24f;

    [Header("Starting Chunks")]
    [Tooltip("Pre-placed chunks at the level start; despawned like any other chunk.")]
    [SerializeField] GameObject[] startingChunks;
    [Tooltip("Offset so the first spawned chunk doesn't overlap the starting chunks.")]
    [SerializeField] float firstChunkOffset = 12f;

    [Header("Kill Zone")]
    [Tooltip("Width of the kill zone that trails below the lowest loaded chunk.")]
    [SerializeField] float killZoneWidth = 50f;

    [Header("Coins")]
    [SerializeField] GameObject coinPrefab;
    [Range(0f, 1f)]
    [SerializeField] float coinChance = 0.7f;
    [Tooltip("Coins spawn between these x positions (inside the shaft).")]
    [SerializeField] float coinMinX = -2f;
    [SerializeField] float coinMaxX = 2f;

    float nextSpawnY;
    Transform killZone;
    readonly List<GameObject> spawnedChunks = new List<GameObject>();

    void Start()
    {
        nextSpawnY = transform.position.y + firstChunkOffset;
        spawnedChunks.AddRange(startingChunks);
        killZone = CreateKillZone();
    }

    Transform CreateKillZone()
    {
        GameObject killZoneObject = new GameObject("KillZone");
        killZoneObject.transform.SetParent(transform);
        killZoneObject.transform.position = new Vector3(0f, transform.position.y - despawnDistance, 0f);

        BoxCollider2D collider = killZoneObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(killZoneWidth, 1f);

        killZoneObject.AddComponent<KillZone>();
        return killZoneObject.transform;
    }

    void Update()
    {
        if (nextSpawnY - player.position.y < spawnAheadDistance)
        {
            Spawn();
        }

        DespawnChunksBelowPlayer();
    }

    void Spawn()
    {
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
        GameObject chunk = Instantiate(prefab, new Vector3(0f, nextSpawnY, 0f), Quaternion.identity);
        chunk.transform.SetParent(transform);
        spawnedChunks.Add(chunk);

        TrySpawnCoin(chunk);

        nextSpawnY += chunkHeight;
    }

    void TrySpawnCoin(GameObject chunk)
    {
        if (coinPrefab == null || Random.value > coinChance) return;

        Physics2D.SyncTransforms();

        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 position = new Vector2(
                Random.Range(coinMinX, coinMaxX),
                chunk.transform.position.y + Random.Range(-chunkHeight / 2f, chunkHeight / 2f));

            if (Physics2D.OverlapCircle(position, 0.5f) == null)
            {
                Instantiate(coinPrefab, position, Quaternion.identity, chunk.transform);
                return;
            }
        }
    }

    void DespawnChunksBelowPlayer()
    {
        for (int i = spawnedChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = spawnedChunks[i];

            if (player.position.y - chunk.transform.position.y > despawnDistance)
            {
                spawnedChunks.RemoveAt(i);

                if (chunk.transform.position.y > killZone.position.y)
                {
                    killZone.position = new Vector3(killZone.position.x, chunk.transform.position.y, killZone.position.z);
                }

                Destroy(chunk);
            }
        }
    }
}