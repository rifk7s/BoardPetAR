using System.Collections;
using UnityEngine;
using Vuforia;

// Plays the intro, one step after another so each step can be seen:
//   1. the cave grows out of the ledge (rock thud)
//   2. a glowing portal opens inside the cave arch and glows for a moment
//   3. the pet hops out through the portal in a puff of smoke
//      (it starts behind the invisible CaveMask, so it looks like it comes out from behind the cave wall)
//   4. the portal closes behind it
// It is hooked to the image target's OnTargetFound event, and it only plays
// once per app session (losing and finding the marker again does nothing once it has finished).
//
// The intro only plays while the marker is really in view (status TRACKED).
// Vuforia also reports the marker as found while it is only EXTENDED_TRACKED or LIMITED
// (the phone is pointed away and the position is just estimated), and then the player would
// miss the whole intro. So if the marker stays out of view for a moment before the intro is
// finished, the intro stops and starts again from the beginning when the marker is in view again.
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
    [Tooltip("Where the pet starts: just behind the invisible CaveMask, so it is hidden until it moves forward through the cave opening.")]
    [SerializeField] private Vector3 petStartPosition = new Vector3(0f, 0f, 0.036f);
    [SerializeField] private Vector3 petRestPosition = new Vector3(0f, 0f, -0.04f);
    [Tooltip("The pet starts at this fraction of its full size and grows while it comes out.")]
    [SerializeField] private float petStartScaleFraction = 0.4f;

    [Header("Marker visibility")]
    [Tooltip("The marker may be out of view this long (tracking can flicker for a moment) before the intro restarts.")]
    [SerializeField] private float markerLostGraceSeconds = 0.4f;

    private bool introIsFinished = false;
    private bool introIsRunning = false;
    private Coroutine introRoutine;
    private float secondsMarkerNotInView = 0f;
    private ObserverBehaviour imageTarget;
    private Vector3 caveFullScale;
    private Vector3 petFullScale;

    // True only while the marker is really in the camera view, not just estimated.
    private bool MarkerIsInView
    {
        get { return imageTarget != null && imageTarget.TargetStatus.Status == Status.TRACKED; }
    }

    private void Awake()
    {
        caveFullScale = cave.localScale;
        petFullScale = pet.localScale;
        imageTarget = GetComponentInParent<ObserverBehaviour>();

        cave.gameObject.SetActive(false);
        pet.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (imageTarget != null)
        {
            imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDisable()
    {
        if (imageTarget != null)
        {
            imageTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    // Called from the OnTargetFound event of the image target.
    public void PlayIfFirstTime()
    {
        StartIntroIfMarkerInView();
    }

    // Starts the intro again when the marker comes back into view after an unfinished intro.
    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (targetStatus.Status == Status.TRACKED)
        {
            StartIntroIfMarkerInView();
        }
    }

    private void StartIntroIfMarkerInView()
    {
        if (introIsFinished || introIsRunning || !MarkerIsInView)
        {
            return;
        }

        introIsRunning = true;
        secondsMarkerNotInView = 0f;
        introRoutine = StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        if (!introIsRunning)
        {
            return;
        }

        if (MarkerIsInView)
        {
            secondsMarkerNotInView = 0f;
            return;
        }

        secondsMarkerNotInView += Time.deltaTime;
        if (secondsMarkerNotInView >= markerLostGraceSeconds)
        {
            StopIntroAndReset();
        }
    }

    // Stops the unfinished intro and puts everything back to how it was before the intro,
    // so the next time the marker is in view it plays from the beginning.
    private void StopIntroAndReset()
    {
        StopCoroutine(introRoutine);
        introRoutine = null;
        introIsRunning = false;
        secondsMarkerNotInView = 0f;

        portal.ResetToClosed();
        smokePuff.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        cave.gameObject.SetActive(false);
        pet.gameObject.SetActive(false);
        pet.localPosition = petStartPosition;
        pet.localScale = petFullScale;
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
        // From now on losing the marker does nothing to it.
        introIsFinished = true;
        introIsRunning = false;
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
            pet.localScale = Vector3.Lerp(petFullScale * petStartScaleFraction, petFullScale, progress);
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
