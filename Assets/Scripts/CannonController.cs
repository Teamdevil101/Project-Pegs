using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public float shootForce = 10f;
    public LineRenderer trajectoryLine;
    public int resolution = 40;
    public Vector2 aimOffset = Vector2.zero;

    [Header("Aim Adjustment")]
    public Vector2 aimOffset = Vector2.zero; // Optional offset to fine-tune mouse alignment

    private float lastValidAngle = 0f;

    void Start()
    {
        // SoundManager handles all audio now
    }

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

        float launchAngle = CalculateLaunchAngle(spawnPoint.position, mousePos, shootForce);

        // Calculate angle in degrees
        if (!float.IsNaN(launchAngle))
        {
            float angle = launchAngle * Mathf.Rad2Deg + 90f;
            angle = Mathf.Clamp(angle, -90f, 90f);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    float CalculateLaunchAngle(Vector2 startPos, Vector2 targetPos, float speed)
    {
        Vector2 displacement = targetPos - startPos;
        float x = displacement.x;
        float y = displacement.y;
        float distance = displacement.magnitude;

        if (distance < 1.2f)
        {
            return lastValidAngle;
        }

        Vector2 gravity = Physics2D.gravity;
        if (ballPrefab != null)
        {
            Rigidbody2D rb = ballPrefab.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                gravity *= rb.gravityScale;
            }
        }
        float g = -gravity.y;

        if (Mathf.Abs(g) < 0.01f)
        {
            return lastValidAngle;
        }

        float v2 = speed * speed;
        float v4 = v2 * v2;
        float gx2 = g * x * x;

        float discriminant = v4 - g * (gx2 + 2 * y * v2);

        if (discriminant < 0)
        {
            return lastValidAngle;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float numerator1 = v2 + sqrtDiscriminant;
        float numerator2 = v2 - sqrtDiscriminant;
        float denominator = g * x;

        float angle1 = Mathf.Atan2(numerator1, denominator);
        float angle2 = Mathf.Atan2(numerator2, denominator);

        float angleDeg1 = angle1 * Mathf.Rad2Deg + 90f;
        float angleDeg2 = angle2 * Mathf.Rad2Deg + 90f;

        float minAngle = -85f;
        float maxAngle = 85f;

        bool angle1Valid = angleDeg1 >= minAngle && angleDeg1 <= maxAngle;
        bool angle2Valid = angleDeg2 >= minAngle && angleDeg2 <= maxAngle;

        if (angle2Valid && angle1Valid)
        {
            float angle1Margin = Mathf.Min(Mathf.Abs(angleDeg1 - minAngle), Mathf.Abs(angleDeg1 - maxAngle));
            float angle2Margin = Mathf.Min(Mathf.Abs(angleDeg2 - minAngle), Mathf.Abs(angleDeg2 - maxAngle));

            if (angle2Margin > angle1Margin)
            {
                lastValidAngle = angle2;
                return angle2;
            }
            else
            {
                lastValidAngle = angle1;
                return angle1;
            }
        }
        else if (angle1Valid)
        {
            lastValidAngle = angle1;
            return angle1;
        }
        else if (angle2Valid)
        {
            lastValidAngle = angle2;
            return angle2;
        }
        else
        {
            return lastValidAngle;
        }
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
            SoundManager.instance.PlaySound(SoundManager.instance.shootSound);
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
