using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Cannon Sounds")]
    public AudioClip shootSound;
    public AudioClip reloadedSound;
    public AudioClip outSound;

    [Header("Collision Sounds")]
    public AudioClip boundCollisionSound;
    [Tooltip("Peg impact sounds are configured in GameManager.pegData based on PegType (Regular, Mandatory, PowerUp, Special)")]
    public string pegCollisionSoundInfo = "Peg impact sounds are configured in GameManager.pegData based on PegType";

    [Header("Velocity-Based Volume Settings")]
    [Tooltip("Velocity at which peg impact sound starts playing")]
    public float pegMinVelocity = 1f;
    [Tooltip("Velocity at which peg impact sound reaches max volume")]
    public float pegMaxVelocity = 20f;
    [Tooltip("Minimum volume for peg impacts (0-1)")]
    public float pegMinVolume = 0.2f;
    [Tooltip("Maximum volume for peg impacts (0-1)")]
    public float pegMaxVolume = 1f;

    [Header("Audio Settings")]
    public float masterVolume = 1f;
    [Range(0.1f, 1f)] public float sfxVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError($"Multiple SoundManagers detected! Deleting the extra manager in {gameObject.name}");
            Destroy(gameObject);
        }

        // Get or create AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Play a sound effect with optional delay
    /// </summary>
    public void PlaySound(AudioClip clip, float delay = 0f)
    {
        if (audioSource == null || clip == null)
            return;

        if (delay > 0f)
        {
            Invoke(nameof(PlaySoundImmediate), delay);
            // Store the clip to play in a temporary variable
            audioSource.clip = clip;
        }
        else
        {
            PlaySoundImmediate(clip);
        }
    }

    private void PlaySoundImmediate(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.volume = masterVolume * sfxVolume;
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Play a sound with velocity-based volume (for peg collisions)
    /// </summary>
    public void PlayPegImpactSound(AudioClip clip, float velocity)
    {
        if (audioSource == null || clip == null)
            return;

        float volume = Mathf.Clamp01(Mathf.InverseLerp(pegMinVelocity, pegMaxVelocity, velocity));
        volume = Mathf.Lerp(pegMinVolume, pegMaxVolume, volume);
        
        audioSource.volume = masterVolume * sfxVolume * volume;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play a sound with velocity-based volume (generic version)
    /// </summary>
    public void PlaySoundWithVelocity(AudioClip clip, float velocity, float minVelocity = 1f, float maxVelocity = 20f, float minVolume = 0.2f, float maxVolume = 1f)
    {
        if (audioSource == null || clip == null)
            return;

        float volume = Mathf.Clamp01(Mathf.InverseLerp(minVelocity, maxVelocity, velocity));
        volume = Mathf.Lerp(minVolume, maxVolume, volume);
        
        audioSource.volume = masterVolume * sfxVolume * volume;
        audioSource.PlayOneShot(clip);
    }
}
