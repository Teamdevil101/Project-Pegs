using UnityEngine;

public class BucketController : MonoBehaviour
{
    public float moveSpeed = 3f;      // How fast the bucket moves
    public float leftLimit = -8f;     // Adjust manually to match your walls
    public float rightLimit = 8f;     // Adjust manually to match your walls
    private int direction = 1;

    void Update()
    {
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        // Reverse when reaching wall limits
        if (transform.position.x >= rightLimit)
            direction = -1;
        else if (transform.position.x <= leftLimit)
            direction = 1;
    }
}
