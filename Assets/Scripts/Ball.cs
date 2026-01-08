using UnityEngine;

public class Ball : MonoBehaviour
{
    [Tooltip("Tag of the ball object.")]
    public string ballTag = "Ball";

    private void Start()
    {
        // Ensure correct tag
        if (!CompareTag(ballTag))
            gameObject.tag = ballTag;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ball fell out of the level
        if (other.CompareTag("DeathZone"))
        {
            // Tell GameManager this ball is gone
            if (GameManager.instance != null)
            {
                GameManager.instance.AdjustActiveBallToCount(-1);
                GameManager.instance.AdjustBallCount(-1);
            }

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ball hit a peg
        if (collision.gameObject.TryGetComponent(out PegAction peg))
        {
            // Store peg for later disable (Peggle-style cleanup)
            GameManager.instance.StoreForDestruction(peg.gameObject);
        }
    }
}
