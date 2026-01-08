using UnityEngine;

public class PegAction : MonoBehaviour
{
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
        if (pegSpriteRenderer != null)
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

    void HandleHit(GameObject other)
    {
        if (triggered) return;
        if (!string.IsNullOrEmpty(ballTag) && !other.CompareTag(ballTag)) return;
        if (other.GetComponent<Rigidbody2D>() == null) return;

        triggered = true;

        if (pegSpriteRenderer != null)
            pegSpriteRenderer.sprite = hitSprite;

        if (GameManager.instance != null)
            GameManager.instance.StoreForDestruction(gameObject);

        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    public enum PegType
    {
        Regular,
        Mandatory,
        PowerUp,
        Special
    }
}
