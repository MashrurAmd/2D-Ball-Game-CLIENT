using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public BallSpawner spawner;

    public Transform topRight;    // RED
    public Transform topLeft;     // BLUE
    public Transform bottomLeft;  // GREEN
    public Transform bottomRight; // YELLOW

    public Transform centerPoint;
    public float centerRadius = 2f;
    public float minSwipeDistance = 0.2f;

    [Header("Ball & Corner Colors (set with HEX)")]
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    public Color blueColor = Color.blue;
    public Color yellowColor = Color.yellow;

    [HideInInspector] public BallController currentBall;
    private Vector2 swipeStartWorld;
    private bool swipeStarted = false;
    private int score = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Apply colors to corner objects so they match
        if (topRight) topRight.GetComponent<SpriteRenderer>().color = redColor;
        if (topLeft) topLeft.GetComponent<SpriteRenderer>().color = blueColor;
        if (bottomLeft) bottomLeft.GetComponent<SpriteRenderer>().color = greenColor;
        if (bottomRight) bottomRight.GetComponent<SpriteRenderer>().color = yellowColor;
    }

    private void Update()
    {
        if (currentBall == null) return;

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
            Debug.Log($"Matched! Score: {score}");
        }
        else
        {
            Debug.Log($"Wrong! Ball {ball.ballColor} → {corner}. Score: {score}");
        }

        spawner.SpawnBall();
    }

    // BallController calls this to get the correct hex-defined color
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
}
