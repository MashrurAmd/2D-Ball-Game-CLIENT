using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public BallSpawner spawner;
    public Transform topRight, topLeft, bottomLeft, bottomRight;
    public Transform middleRight, middleLeft;
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
    public Color orangeColor = new Color(1f, 0.5f, 0f);
    public Color cyanColor = Color.cyan;

    [Header("UI")]
    public Text scoreText;
    public Text livesText;
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button exitButton;

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

        // Set corner colors
        if (topRight) topRight.GetComponent<SpriteRenderer>().color = redColor;
        if (topLeft) topLeft.GetComponent<SpriteRenderer>().color = blueColor;
        if (bottomLeft) bottomLeft.GetComponent<SpriteRenderer>().color = greenColor;
        if (bottomRight) bottomRight.GetComponent<SpriteRenderer>().color = yellowColor;
        if (middleRight) middleRight.GetComponent<SpriteRenderer>().color = orangeColor;
        if (middleLeft) middleLeft.GetComponent<SpriteRenderer>().color = cyanColor;

        // Button listeners
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
        if (exitButton) exitButton.onClick.AddListener(ExitGame);

        UpdateUI();
    }
    private void Update()
    {
        if (currentBall == null || missCount >= maxMisses) return;

        Vector2 center = centerPoint != null ? (Vector2)centerPoint.position : Vector2.zero;

        // 🟢 Start swipe (anywhere on screen)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0f;

            swipeStartWorld = world;
            swipeStarted = true;

            /*Rigidbody2D currentBallRb = currentBall.GetComponent<Rigidbody2D>();

            currentBallRb.velocity = Vector2.zero;

            currentBallRb.gravityScale = 0f;

            currentBall.transform.position = world;*/
        }

        // 🔵 End swipe (anywhere on screen)
        if (swipeStarted && Input.GetMouseButtonUp(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0f;
            Vector2 swipe = (Vector2)world - swipeStartWorld;
            swipeStarted = false;

            if (swipe.magnitude < minSwipeDistance) return;

            // ✅ Ball must be inside radius
            if (Vector2.Distance(currentBall.transform.position, center) <= centerRadius)
            {
                Corner corner = GetCornerFromDirection(swipe);
                Transform cornerTransform = GetCornerTransform(corner);

                if (cornerTransform != null && currentBall != null)
                {
                    currentBall.MoveToCorner(cornerTransform, corner, 12f);
                }
            }
        }

        // Show/hide orange & cyan after score
        if (score >= 40)
        {
            if (middleRight) middleRight.gameObject.SetActive(true);
            if (middleLeft) middleLeft.gameObject.SetActive(true);
        }
        else
        {
            if (middleRight) middleRight.gameObject.SetActive(false);
            if (middleLeft) middleLeft.gameObject.SetActive(false);
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
            case Corner.BottomRight: expected = BallColor.Purple; break;
            case Corner.MiddleRight: expected = BallColor.Orange; break;
            case Corner.MiddleLeft: expected = BallColor.Cyan; break;
        }

        if (ball.ballColor == expected)
        {
            score++;
            audiomanager.Instance.PlayCorrect();   // ✅ play pop sound
        }
        else
        {
            missCount++;
            audiomanager.Instance.PlayWrong();     // ❌ play wrong sound

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
        audiomanager.Instance.PlayWrong();  // ❌ missed ball = wrong sound

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
            case BallColor.Purple: return yellowColor;
            case BallColor.Orange: return orangeColor;
            case BallColor.Cyan: return cyanColor;
            default: return Color.white;
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (gameOverPanel) gameOverPanel.SetActive(true);

        if (scoreText) scoreText.gameObject.SetActive(false);
        if (livesText) livesText.gameObject.SetActive(false);

        Time.timeScale = 0f; // pause game

        // ⏹️ Stop background music
        if (audiomanager.Instance != null)
            audiomanager.Instance.StopMusic();
    }


    private void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (livesText) livesText.text = $"Lives: {maxMisses - missCount}";
    }

    private void RestartGame()
    {
        score = 0;
        missCount = 0;

        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (scoreText) scoreText.gameObject.SetActive(true);
        if (livesText) livesText.gameObject.SetActive(true);

        if (currentBall != null)
            Destroy(currentBall.gameObject);

        Time.timeScale = 1f; // ▶️ resume game

        // 🎵 resume background music
        if (audiomanager.Instance != null)
            audiomanager.Instance.ResumeMusic();

        spawner.SpawnBall();
        UpdateUI();
    }


    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private Corner GetCornerFromDirection(Vector2 dir)
    {
        if (GameManager.Instance.score < 40)
        {
            // Before 30 points: only 4 diagonal corners
            if (dir.x >= 0f && dir.y >= 0f) return Corner.TopRight;
            if (dir.x < 0f && dir.y >= 0f) return Corner.TopLeft;
            if (dir.x < 0f && dir.y < 0f) return Corner.BottomLeft;
            return Corner.BottomRight;
        }
        else
        {
            // After 30 points: 6 directions
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) // horizontal dominant
                return dir.x > 0f ? Corner.MiddleRight : Corner.MiddleLeft;

            if (dir.x >= 0f && dir.y >= 0f) return Corner.TopRight;
            if (dir.x < 0f && dir.y >= 0f) return Corner.TopLeft;
            if (dir.x < 0f && dir.y < 0f) return Corner.BottomLeft;
            return Corner.BottomRight;
        }
    }


    private Transform GetCornerTransform(Corner c)
    {
        switch (c)
        {
            case Corner.TopRight: return topRight;
            case Corner.TopLeft: return topLeft;
            case Corner.BottomLeft: return bottomLeft;
            case Corner.BottomRight: return bottomRight;
            case Corner.MiddleRight: return middleRight;
            case Corner.MiddleLeft: return middleLeft;
            default: return null;
        }
    }
}
