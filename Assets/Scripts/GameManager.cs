using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // for restarting the scene

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public BallSpawner spawner;
    public Transform topRight, topLeft, bottomLeft, bottomRight;
    public Transform centerPoint;

    [Header("Gameplay Settings")]
    public float centerRadius = 2f;
    public float minSwipeDistance = 0.2f;
    public int maxMisses = 3;

    [Header("Ball & Corner Colors (set with HEX)")]
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    public Color blueColor = Color.blue;
    public Color yellowColor = Color.yellow;

    [Header("UI")]
    public Text scoreText;
    public Text livesText;
    public GameObject gameOverPanel; // assign in inspector
    public Button restartButton;     // assign in inspector
    public Button exitButton;        // assign in inspector

    [HideInInspector] public BallController currentBall;

    private Vector2 swipeStartWorld;
    private bool swipeStarted = false;
    public int score = 0;
    private int missCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gameOverPanel) gameOverPanel.SetActive(false);

        // set corner colors
        if (topRight) topRight.GetComponent<SpriteRenderer>().color = redColor;
        if (topLeft) topLeft.GetComponent<SpriteRenderer>().color = blueColor;
        if (bottomLeft) bottomLeft.GetComponent<SpriteRenderer>().color = greenColor;
        if (bottomRight) bottomRight.GetComponent<SpriteRenderer>().color = yellowColor;

        // Button listeners
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
        if (exitButton) exitButton.onClick.AddListener(ExitGame);

        UpdateUI();
    }

    private void Update()
    {
        if (currentBall == null || missCount >= maxMisses) return;

        Vector2 center = centerPoint != null ? (Vector2)centerPoint.position : Vector2.zero;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0f;

            if (Vector2.Distance(world, center) <= centerRadius &&
                Vector2.Distance(currentBall.transform.position, center) <= centerRadius)
            {
                swipeStartWorld = world;
                swipeStarted = true;
            }
        }

        if (swipeStarted && Input.GetMouseButtonUp(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0f;
            Vector2 swipe = (Vector2)world - swipeStartWorld;
            swipeStarted = false;

            if (swipe.magnitude < minSwipeDistance) return;

            Corner corner = GetCornerFromDirection(swipe);
            Transform cornerTransform = GetCornerTransform(corner);

            if (cornerTransform != null && currentBall != null)
            {
                currentBall.MoveToCorner(cornerTransform, corner, 12f);
            }
        }
    }

    public void OnBallArrived(BallController ball, Corner corner)
    {
        BallColor expected = BallColor.Red;
        switch (corner)
        {
            case Corner.TopRight: expected = BallColor.Red; break;
            case Corner.TopLeft: expected = BallColor.Blue; break;
            case Corner.BottomLeft: expected = BallColor.Green; break;
            case Corner.BottomRight: expected = BallColor.Yellow; break;
        }

        if (ball.ballColor == expected)
        {
            score++;
        }
        else
        {
            missCount++;
            if (missCount >= maxMisses)
            {
                GameOver();
                return;
            }
        }

        UpdateUI();
        spawner.SpawnBall();
    }

    public void OnBallMissed(BallController ball)
    {
        missCount++;
        if (missCount >= maxMisses)
        {
            GameOver();
            return;
        }
        UpdateUI();
        spawner.SpawnBall();
    }

    public Color GetColor(BallColor c)
    {
        switch (c)
        {
            case BallColor.Red: return redColor;
            case BallColor.Green: return greenColor;
            case BallColor.Blue: return blueColor;
            case BallColor.Yellow: return yellowColor;
            default: return Color.white;
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // hide score & lives when game over
        if (scoreText) scoreText.gameObject.SetActive(false);
        if (livesText) livesText.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (livesText) livesText.text = $"Lives: {maxMisses - missCount}";
    }

    private void RestartGame()
    {
        // reset variables
        score = 0;
        missCount = 0;

        // hide GameOver panel
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // show UI
        if (scoreText) scoreText.gameObject.SetActive(true);
        if (livesText) livesText.gameObject.SetActive(true);

        // destroy current ball if exists
        if (currentBall != null)
        {
            Destroy(currentBall.gameObject);
        }

        // spawn a new ball
        spawner.SpawnBall();
        UpdateUI();
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stop play mode in editor
#else
        Application.Quit(); // quit build
#endif
    }

    private Corner GetCornerFromDirection(Vector2 dir)
    {
        if (dir.x >= 0f && dir.y >= 0f) return Corner.TopRight;
        if (dir.x < 0f && dir.y >= 0f) return Corner.TopLeft;
        if (dir.x < 0f && dir.y < 0f) return Corner.BottomLeft;
        return Corner.BottomRight;
    }

    private Transform GetCornerTransform(Corner c)
    {
        switch (c)
        {
            case Corner.TopRight: return topRight;
            case Corner.TopLeft: return topLeft;
            case Corner.BottomLeft: return bottomLeft;
            case Corner.BottomRight: return bottomRight;
            default: return null;
        }
    }
}
