using System.Collections;
using UnityEngine;

// Plays the intro, one step after another so each step can be seen:
//   1. the cave grows out of the ledge (rock thud)
//   2. a glowing portal opens inside the cave arch and glows for a moment
//   3. the pet hops out through the portal in a puff of smoke
//   4. the portal closes behind it
// It is hooked to the image target's OnTargetFound event, and it only plays
// once per app session (losing and finding the marker again does nothing).
public class CaveIntro : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Transform cave;
    [SerializeField] private Transform pet;
    [SerializeField] private PetStateMachine petStateMachine;

    [Header("Portal effects")]
    [Tooltip("Glowing portal inside the cave arch.")]
    [SerializeField] private CavePortal portal;
    [Tooltip("Small puff of smoke, started when the pet hops out.")]
    [SerializeField] private ParticleSystem smokePuff;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip caveAppearSound;
    [SerializeField] private AudioClip petHopOutSound;

    [Header("Timing and movement (meters, local to LedgeAnchor)")]
    [SerializeField] private float caveGrowSeconds = 0.6f;
    [SerializeField] private float pauseBeforePortalSeconds = 0.2f;
    [Tooltip("How long the open portal glows before the pet comes out.")]
    [SerializeField] private float portalGlowSeconds = 0.5f;
    [SerializeField] private float petHopOutSeconds = 0.6f;
    [SerializeField] private float petHopHeightMeters = 0.03f;
    [SerializeField] private Vector3 petStartPosition = new Vector3(0f, 0f, 0.015f);
    [SerializeField] private Vector3 petRestPosition = new Vector3(0f, 0f, -0.04f);

    private bool hasPlayedThisSession = false;
    private Vector3 caveFullScale;
    private Vector3 petFullScale;

    private void Awake()
    {
        caveFullScale = cave.localScale;
        petFullScale = pet.localScale;

        cave.gameObject.SetActive(false);
        pet.gameObject.SetActive(false);
    }

    // Called from the OnTargetFound event of the image target.
    public void PlayIfFirstTime()
    {
        if (hasPlayedThisSession)
        {
            return;
        }

        hasPlayedThisSession = true;
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // 1. The cave grows out of the ledge.
        cave.gameObject.SetActive(true);
        cave.localScale = Vector3.zero;
        PlaySound(caveAppearSound);
        yield return ScaleOverTime(cave, Vector3.zero, caveFullScale, caveGrowSeconds);

        // 2. The portal opens inside the arch and glows for a moment.
        yield return new WaitForSeconds(pauseBeforePortalSeconds);
        yield return portal.Open();
        yield return new WaitForSeconds(portalGlowSeconds);

        // 3. The pet hops out through the portal.
        pet.gameObject.SetActive(true);
        smokePuff.Play();
        PlaySound(petHopOutSound);
        yield return HopPetOutOfCave();

        // 4. The portal closes behind the pet.
        yield return portal.Close();

        // The intro is over: let the state machine take over the pet.
        petStateMachine.RequestState(PetState.Wandering);
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 from, Vector3 to, float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / seconds);
            target.localScale = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, progress));
            yield return null;
        }
        target.localScale = to;
    }

    private IEnumerator HopPetOutOfCave()
    {
        Vector3 startPosition = petStartPosition;
        float elapsed = 0f;

        while (elapsed < petHopOutSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / petHopOutSeconds);

            Vector3 groundPosition = Vector3.Lerp(startPosition, petRestPosition, progress);
            float hopHeight = Mathf.Sin(progress * Mathf.PI) * petHopHeightMeters;

            pet.localPosition = groundPosition + Vector3.up * hopHeight;
            pet.localScale = Vector3.Lerp(Vector3.zero, petFullScale, progress);
            yield return null;
        }

        pet.localPosition = petRestPosition;
        pet.localScale = petFullScale;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
