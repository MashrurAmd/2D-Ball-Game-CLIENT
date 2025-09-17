using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{
    public float baseFallSpeed = 3f; // starting speed
    public float moveToCornerSpeed = 10f;

    [HideInInspector] public BallColor ballColor;
    private float fallSpeed;

    private SpriteRenderer sr;
    private bool isMovingToCorner = false;
    private Vector3 targetPosition;
    private Corner targetCorner;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void InitRandom()
    {
        int colorCount = GameManager.Instance.score >= 30 ? 6 : 4; // unlock Orange & Cyan after 30 points
        int r = Random.Range(0, colorCount);
        ballColor = (BallColor)r;
        sr.color = GameManager.Instance.GetColor(ballColor);

        UpdateSpeed();
    }

    public void Init(BallColor color)
    {
        ballColor = color;
        sr.color = GameManager.Instance.GetColor(ballColor);

        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        fallSpeed = baseFallSpeed + GameManager.Instance.score * 0.05f;
    }

    private void Update()
    {
        if (!isMovingToCorner)
        {
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

            float bottomLimit = Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;
            if (transform.position.y < bottomLimit)
            {
                GameManager.Instance.OnBallMissed(this);
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveToCornerSpeed * Time.deltaTime);
            if (Vector3.SqrMagnitude(transform.position - targetPosition) < 0.0004f)
            {
                GameManager.Instance.OnBallArrived(this, targetCorner);
                Destroy(gameObject);
            }
        }
    }

    public void MoveToCorner(Transform cornerTransform, Corner corner, float speed = 10f)
    {
        targetPosition = cornerTransform.position;
        targetCorner = corner;
        moveToCornerSpeed = speed;
        isMovingToCorner = true;
    }
}
