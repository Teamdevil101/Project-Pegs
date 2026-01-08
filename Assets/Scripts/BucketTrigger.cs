using UnityEngine;

public class BucketTrigger : MonoBehaviour
{
    public string ballTag = "Ball";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ballTag))
        {
            Debug.Log("Free ball collected!");
            Destroy(other.gameObject);
            
        }
    }
}
