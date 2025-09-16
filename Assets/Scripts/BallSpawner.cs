using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public GameManager gameManager;

    private void Start()
    {
        SpawnBall();
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

        return bc;
    }
}
