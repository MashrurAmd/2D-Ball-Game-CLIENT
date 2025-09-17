using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public GameManager gameManager;

    private float spawnDelay = 1f; // base delay
    private float minSpawnDelay = 0.3f; // cap minimum delay
    private float lastSpawnTime;

    private void Update()
    {
        if (gameManager.currentBall == null && Time.time - lastSpawnTime >= spawnDelay)
        {
            SpawnBall();
        }
    }

    public BallController SpawnBall()
    {
        if (ballPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Spawner: missing prefab or spawnPoint");
            return null;
        }

        GameObject go = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        BallController bc = go.GetComponent<BallController>();
        if (bc == null) bc = go.AddComponent<BallController>();

        bc.InitRandom();
        if (gameManager != null) gameManager.currentBall = bc;

        lastSpawnTime = Time.time;

        // Adjust spawn rate with score
        float difficultyFactor = Mathf.Clamp01(gameManager.score / 100f);
        spawnDelay = Mathf.Lerp(1f, minSpawnDelay, difficultyFactor);

        return bc;
    }
}