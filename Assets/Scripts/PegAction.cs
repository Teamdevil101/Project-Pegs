using UnityEngine;

public class PegAction : MonoBehaviour
{
    [Tooltip("Only objects with this tag will trigger the peg.")]
    public string ballTag = "";

    public Sprite baseSprite;
    public Sprite hitSprite;

    public SpriteRenderer pegSpriteRenderer;
    private Collider2D myCollider;

    public PegType MyPegType { get; private set; }
    private bool triggered = false;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        pegSpriteRenderer.sprite = baseSprite;
    }

    private void Start()
    {
        UpdatePegColor();
    }

    public void UpdatePegColor()
    {
        pegSpriteRenderer.color = GameManager.instance.pegData[(int)MyPegType].baseColor;
    }

    public void SetPegType(PegType type) => MyPegType = type;

    // Shared handler for both trigger and collision events
    void HandleHit(GameObject other)
    {
        if (triggered) return;

        // Optional tag filter
        if (!string.IsNullOrEmpty(ballTag) && !other.CompareTag(ballTag)) return;

        // Ensure the other object is a physics object (has a Rigidbody2D)
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        triggered = true;

        if (pegSpriteRenderer != null)
            pegSpriteRenderer.sprite = hitSprite;

        PlayImpactSound(rb.linearVelocity.magnitude);

        if (myCollider != null)
            GameManager.instance.StoreForDestruction(gameObject);
    }

    void PlayImpactSound(float velocity)
    {
        AudioClip impactClip = GameManager.instance.pegData[(int)MyPegType].impactSound;
        
        if (impactClip != null)
        {
            SoundManager.instance.PlayPegImpactSound(impactClip, velocity);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    // Useful for trigger objects
    /*void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }*/

    public enum PegType
    {
        Regular,
        Mandatory,
        PowerUp,
        Special
    }
}
