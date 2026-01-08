using UnityEngine;

public class BallDeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AdjustActiveBallToCount(-1);
                GameManager.instance.AdjustBallCount(-1);
            }

            Destroy(other.gameObject);
        }
    }
}
