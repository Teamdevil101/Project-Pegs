using UnityEngine;

public class BallDeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.outSound);

            if (GameManager.instance != null)
                GameManager.instance.AdjustActiveBallToCount(-1);

            Destroy(other.gameObject);
        }
    }
}
