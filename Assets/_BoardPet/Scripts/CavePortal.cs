using System.Collections;
using UnityEngine;

// A glowing portal that fills the opening of the cave arch.
// The visible part ("visual") is three flat layers (Unity Quads with Kenney particle textures):
// a soft glow that fills the opening, a bright rim, and a faint swirl inside, plus rising sparkles.
// Open() grows it from a dot into an oval, Close() shrinks it away again.
// While open it turns slowly and gently pulses, so it reads as energy and not as a loading spinner.
//
// This script sits on an object that is always active. Only "visual" is switched on and off,
// because Vuforia turns renderers back on when the marker is found but never activates objects.
public class CavePortal : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform visual;
    [SerializeField] private Transform rim;
    [SerializeField] private Transform swirl;
    [SerializeField] private ParticleSystem sparkles;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("Timing (seconds)")]
    [SerializeField] private float openSeconds = 0.5f;
    [SerializeField] private float closeSeconds = 0.4f;

    [Header("Motion while open")]
    [Tooltip("Degrees per second. Slow, so it does not look like a loading spinner.")]
    [SerializeField] private float rimTurnSpeed = 30f;
    [SerializeField] private float swirlTurnSpeed = -45f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.06f;

    private Vector3 fullScale;
    private float openAmount = 0f;

    private void Awake()
    {
        fullScale = visual.localScale;
        visual.gameObject.SetActive(false);
    }

    public IEnumerator Open()
    {
        openAmount = 0f;
        visual.localScale = Vector3.zero;
        visual.gameObject.SetActive(true);
        sparkles.Play();
        PlaySound(openSound);

        float elapsed = 0f;
        while (elapsed < openSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / openSeconds);
            openAmount = EaseOutBack(progress);
            yield return null;
        }
        openAmount = 1f;
    }

    public IEnumerator Close()
    {
        sparkles.Stop();
        PlaySound(closeSound);

        float startAmount = openAmount;
        float elapsed = 0f;
        while (elapsed < closeSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / closeSeconds);
            openAmount = Mathf.Lerp(startAmount, 0f, progress * progress);
            yield return null;
        }

        openAmount = 0f;
        visual.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!visual.gameObject.activeSelf)
        {
            return;
        }

        // A gentle breathing pulse on top of the open amount.
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        visual.localScale = fullScale * openAmount * pulse;

        rim.Rotate(0f, 0f, rimTurnSpeed * Time.deltaTime, Space.Self);
        swirl.Rotate(0f, 0f, swirlTurnSpeed * Time.deltaTime, Space.Self);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Grows a little past full size, then settles back, like a portal snapping open.
    private float EaseOutBack(float t)
    {
        const float overshoot = 1.6f;
        float shifted = t - 1f;
        return 1f + (overshoot + 1f) * shifted * shifted * shifted + overshoot * shifted * shifted;
    }
}
