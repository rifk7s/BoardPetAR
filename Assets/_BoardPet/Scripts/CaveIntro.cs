using System.Collections;
using UnityEngine;

// Plays the intro: the cave grows out of the ledge, then the pet hops out of it.
// It is hooked to the image target's OnTargetFound event, and it only plays
// once per app session (losing and finding the marker again does nothing).
public class CaveIntro : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Transform cave;
    [SerializeField] private Transform pet;
    [SerializeField] private PetStateMachine petStateMachine;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip caveAppearSound;
    [SerializeField] private AudioClip petHopOutSound;

    [Header("Timing and movement (meters, local to LedgeAnchor)")]
    [SerializeField] private float caveGrowSeconds = 0.6f;
    [SerializeField] private float pauseBeforePetSeconds = 0.4f;
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
        cave.gameObject.SetActive(true);
        cave.localScale = Vector3.zero;
        PlaySound(caveAppearSound);
        yield return ScaleOverTime(cave, Vector3.zero, caveFullScale, caveGrowSeconds);

        yield return new WaitForSeconds(pauseBeforePetSeconds);

        pet.gameObject.SetActive(true);
        PlaySound(petHopOutSound);
        yield return HopPetOutOfCave();

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
