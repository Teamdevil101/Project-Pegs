using UnityEngine;

public class BallDeleter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
            GameManager.instance.AdjustActiveBallToCount(-1);
            SoundManager.instance.PlaySound(SoundManager.instance.outSound);
        }
    }
}
