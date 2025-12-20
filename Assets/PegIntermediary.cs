using UnityEngine;

public class PegIntermediary : MonoBehaviour
{
    public PegAction myPeg;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(myPeg != null)
            myPeg.HandleHit(collision.gameObject);
    }
}
