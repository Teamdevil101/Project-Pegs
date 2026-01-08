using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float shootForce = 10f;
    public LineRenderer trajectoryLine;
    public int resolution = 40;
    public Vector2 aimOffset = Vector2.zero;

    void Update()
    {
        RotateCannon();
        DrawTrajectory();

        if (Input.GetMouseButtonDown(0) && GameManager.instance.GetTotalBallCount() > 0)
        {
            Shoot();
        }
    }

    void RotateCannon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (Vector2)mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += 90f;
        angle = Mathf.Clamp(angle, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (ballPrefab != null && spawnPoint != null && GameManager.instance.GetTotalBallCount() > 0)
        {
            GameManager.instance.AdjustActiveBallToCount(1);

            GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);

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
            float t = i * 0.05f;
            Vector2 pos = startPos + startVel * t + 0.5f * gravity * t * t;
            trajectoryLine.SetPosition(i, pos);
        }
    }
}
