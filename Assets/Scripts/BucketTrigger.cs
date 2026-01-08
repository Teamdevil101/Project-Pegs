using UnityEngine;

public class BucketTrigger : MonoBehaviour
{
    public AudioClip ballSavedSound;
    public string ballTag = "Ball";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ballTag))
        {
            Debug.Log("Free ball collected!");
            GameManager.instance.AdjustBallCount(1);
            GameManager.instance.AdjustActiveBallToCount(-1);

            SoundManager.instance.PlaySound(ballSavedSound);

            Destroy(other.gameObject);
            
        }
    }
}
