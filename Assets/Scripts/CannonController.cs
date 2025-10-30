using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;      // Drag your Ball prefab here
    public Transform spawnPoint;       // Drag the BallSpawnPoint here
    public float shootForce = 10f;     // How fast the ball shoots

    [Header("Aim Adjustment")]
    public Vector2 aimOffset = Vector2.zero; // Optional offset to fine-tune mouse alignment

    void Update()
    {
        RotateCannon();

        if (Input.GetMouseButtonDown(0))
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
}
