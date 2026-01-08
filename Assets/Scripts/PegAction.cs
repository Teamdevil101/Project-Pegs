using UnityEngine;

public class PegAction : MonoBehaviour
{
    public string ballTag = "";
    public AudioClip pegPopSound = null;

    [Space]
    public Sprite baseSprite;
    public Sprite hitSprite;

    public SpriteRenderer pegSpriteRenderer;
    public Collider2D myCollider;

    public PegType MyPegType { get; private set; }
    private bool triggered = false;

    private void Awake()
    {
        if (pegSpriteRenderer != null)
            pegSpriteRenderer.sprite = baseSprite;

        if(myCollider == null)
            myCollider = GetComponent<Collider2D>();
        
        pegSpriteRenderer.sprite = baseSprite;
    }

    private void Start()
    {
        UpdatePegColor();
    }

    public void UpdatePegColor()
    {
        if (GameManager.instance != null && pegSpriteRenderer != null)
            pegSpriteRenderer.color = GameManager.instance.pegData[(int)MyPegType].baseColor;
    }

    public void SetPegType(PegType type) => MyPegType = type;

    // Shared handler for both trigger and collision events
    public void HandleHit(GameObject other)
    {
        if (triggered) return;
        if (!string.IsNullOrEmpty(ballTag) && !other.CompareTag(ballTag)) return;

        // Ensure the other object is a physics object (has a Rigidbody2D)
        if (!other.TryGetComponent(out Rigidbody2D rb)) return;

        triggered = true;

        if (pegSpriteRenderer != null)
            pegSpriteRenderer.sprite = hitSprite;

        PlayImpactSound();

        if (myCollider != null)
            GameManager.instance.StoreForDestruction(gameObject);

        if (GameManager.instance.GetAllOrangePegsCount() <= 0 && (GameManager.instance.GetCurrentState() != GameManager.GameState.Win || GameManager.instance.GetCurrentState() != GameManager.GameState.Lose))
            GameManager.instance.WinGame();
    }

    void PlayImpactSound()
    {
        AudioClip impactClip = GameManager.instance.pegData[(int)MyPegType].impactSound;
        
        if (impactClip != null)
        {
            SoundManager.instance.PlayPegImpactSound(impactClip, MyPegType);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    public bool IsTriggered() => triggered;
    public enum PegType
    {
        Regular,
        Mandatory,
        PowerUp,
        Special
    }
}
