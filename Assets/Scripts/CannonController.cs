using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;      // Drag your Ball prefab here
    public Transform spawnPoint;       // Drag the BallSpawnPoint here
    public float shootForce = 10f;     // How fast the ball shoots
    public LineRenderer trajectoryLine;
    public int resolution = 40;

    [Header("Aim Adjustment")]
    public Vector2 aimOffset = Vector2.zero; // Optional offset to fine-tune mouse alignment

    void Update()
    {
        RotateCannon();
        DrawTrajectory();

        if (Input.GetMouseButtonDown(0) && GameManager.instance.GetCurrentState() == GameManager.GameState.Aim)
        {
            Shoot();
        }
    }

    void RotateCannon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (Vector2)mousePos - (Vector2)transform.position;

        // Calculate angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Add rotation offset (if your barrel points up, use +90)
        angle += 90f;

        // Clamp rotation: right to left
        angle = Mathf.Clamp(angle, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (ballPrefab != null && spawnPoint != null && GameManager.instance.GetTotalBallCount() > 0)
        {
            GameManager.instance.AdjustBallCount(-1);
            GameManager.instance.AdjustActiveBallToCount(1);
            GameManager.instance.ChangeState(GameManager.GameState.Shot);

            // Instantiate ball at spawn point
            GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);

            // Add force to the ball
            Rigidbody2D rb = newBall.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(-spawnPoint.up * shootForce, ForceMode2D.Impulse);
            }
        }
    }

    void DrawTrajectory()
    {
        if (trajectoryLine == null) return;

        Vector2 startPos = spawnPoint.position;
        Vector2 startVel = (-spawnPoint.up) * shootForce;

        Vector2 gravity = Physics2D.gravity * ballPrefab.GetComponent<Rigidbody2D>().gravityScale;

        trajectoryLine.positionCount = resolution;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * 0.05f; // The spacing between points

            Vector2 pos =
                startPos +
                startVel * t +
                0.5f * gravity * t * t;

            trajectoryLine.SetPosition(i, pos);
        }
    }

}
