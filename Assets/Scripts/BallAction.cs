using UnityEngine;

public class BallAction : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is tagged as "Bound"
        if (collision.gameObject.CompareTag("Bound"))
        {
            PlayBoundCollisionSound();
        }
    }

    private void PlayBoundCollisionSound()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.boundCollisionSound);
    }
}
