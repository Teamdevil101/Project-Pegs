using UnityEngine;

public class pegAction : MonoBehaviour
{
    [Tooltip("Optional: if set, only objects with this tag will trigger the peg. Leave empty to accept any physics object.")]
    public string requiredTag = "";

    [Tooltip("Color to apply when the peg is hit.")]
    public Color hitColor = Color.yellow;

    [Tooltip("Seconds to wait before destroying the peg after being hit.")]
    public float destroyDelay = 3f;

    SpriteRenderer spriteRenderer;
    Collider2D myCollider;
    bool triggered = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
    }

    // Shared handler for both trigger and collision events
    void HandleHit(GameObject other)
    {
        if (triggered) return;

        // Optional tag filter
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

        // Ensure the other object is a physics object (has a Rigidbody2D)
        if (other.GetComponent<Rigidbody2D>() == null) return;

        triggered = true;

        if (spriteRenderer != null)
            spriteRenderer.color = hitColor;

        // Disable collider so it won't be hit again while waiting to destroy
        if (myCollider != null)
            //myCollider.enabled = false;

        Destroy(gameObject, destroyDelay);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }
}
