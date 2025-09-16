using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{
    public float fallSpeed = 3f;
    public float moveToCornerSpeed = 10f;

    [HideInInspector] public BallColor ballColor;

    private SpriteRenderer sr;
    private bool isMovingToCorner = false;
    private Vector3 targetPosition;
    private Corner targetCorner;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Called by spawner to randomize color
    public void InitRandom()
    {
        int r = Random.Range(0, 4);
        ballColor = (BallColor)r;
        sr.color = GameManager.Instance.GetColor(ballColor);
    }

    // Optional explicit init
    public void Init(BallColor color)
    {
        ballColor = color;
        sr.color = GameManager.Instance.GetColor(ballColor);
    }

    private void Update()
    {
        if (!isMovingToCorner)
        {
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
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
