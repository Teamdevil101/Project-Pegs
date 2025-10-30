using UnityEngine;

public class mouseRotate : MonoBehaviour
{
    [Tooltip("If true, the sprite's TOP side will point toward the mouse. If false, the RIGHT side will point toward the mouse.")]
    public bool alignTopSide = true;

    [Tooltip("If > 0 rotation will be smoothed. 0 = instant rotation.")]
    public float smoothSpeed = 0f;

    [Tooltip("Prefab that will be instantiated and fired toward the mouse on left click.")]
    public GameObject projectilePrefab;

    [Tooltip("Speed applied to the projectile (units/sec). If the prefab has a Rigidbody2D this sets its velocity.")]
    public float projectileSpeed = 10f;

    [Header("Muzzle / spawn settings")]
    [Tooltip("If true the script will attempt to calculate the muzzle distance from the SpriteRenderer bounds. If false, use Manual Muzzle Distance.")]
    public bool autoComputeMuzzleDistance = true;

    [Tooltip("Used when Auto Compute is off. Distance from the transform.position along the chosen local axis to spawn the projectile.")]
    public float manualMuzzleDistance = 0.5f;

    [Tooltip("Extra offset added to the computed muzzle distance (world units).")]
    public float muzzleOffset = 0f;

    [Tooltip("If true the projectile will spawn at the 'opposite' end (away from the pivot) along the chosen local axis.")]
    public bool muzzleFromOppositeEnd = true;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        // Get mouse world position (use the object's screen Z so projection is correct)
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = cam.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        // Direction from object to mouse
        Vector2 dir = mouseWorld - transform.position;
        if (dir.sqrMagnitude <= Mathf.Epsilon) return; // avoid zero-length direction

        // Angle of that direction in degrees measured from +X axis
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Decide which local axis should point to the mouse:
        // - If aligning the TOP side, rotate so local +Y points to the mouse (subtract 90).
        // - If aligning the RIGHT side, rotate so local +X points to the mouse (no offset).
        float zRotation = alignTopSide ? angle - 90f : angle;

        Quaternion target = Quaternion.Euler(0f, 0f, zRotation);

        // Apply rotation (optionally smooth)
        if (smoothSpeed > 0f)
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Mathf.Clamp01(Time.deltaTime * smoothSpeed));
        else
            transform.rotation = target;

        // Shoot on left mouse button down
        if (Input.GetMouseButtonDown(0) && projectilePrefab != null)
        {
            // Determine spawn direction in world space (local axis after rotation)
            Vector3 axisDirWorld = alignTopSide ? transform.up : transform.right;

            // Compute muzzle distance (world units)
            float muzzleDistance;
            if (autoComputeMuzzleDistance && spriteRenderer != null)
            {
                // Use renderer.bounds.extents which is in world space and measured from transform.position
                float extent = alignTopSide ? spriteRenderer.bounds.extents.y : spriteRenderer.bounds.extents.x;
                muzzleDistance = extent + muzzleOffset;
            }
            else
            {
                muzzleDistance = manualMuzzleDistance + muzzleOffset;
            }

            // Choose side: spawn at opposite end (true) or the inverse (false)
            float side = muzzleFromOppositeEnd ? 1f : -1f;

            // Small safety padding so projectile doesn't overlap the shooter
            const float safetyPadding = 0.01f;
            Vector3 spawnPos = transform.position + axisDirWorld * (muzzleDistance + safetyPadding) * side;

            // Instantiate projectile at computed position and oriented toward the mouse
            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));

            // Apply velocity if Rigidbody2D exists
            Rigidbody2D rb2d = proj.GetComponent<Rigidbody2D>();
            Vector2 velocity = dir.normalized * projectileSpeed;
            if (rb2d != null)
            {
                rb2d.linearVelocity = velocity;
            }
            else
            {
                // If no Rigidbody2D, move it one frame-step to avoid overlap; continuous motion should use Rigidbody2D.
                proj.transform.position = proj.transform.position + (Vector3)(velocity * Time.fixedDeltaTime);
            }
        }
    }
}
