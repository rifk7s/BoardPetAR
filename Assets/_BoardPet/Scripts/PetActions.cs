using System.Collections;
using UnityEngine;

// Plays what the pet does for Feed, Play and Sleep.
// PetStateMachine calls these methods when it enters (or leaves) the matching state.
// Eating and Playing finish by themselves and then request Wandering again.
public class PetActions : MonoBehaviour
{
    [SerializeField] private PetStateMachine petStateMachine;
    [SerializeField] private PetHunger petHunger;
    [SerializeField] private PetWander petWander;
    [SerializeField] private Animator animator;
    [SerializeField] private PetEmote petEmote;
    [SerializeField] private PetEyes petEyes;
    [SerializeField] private AudioSource audioSource;

    [Header("Feed")]
    [Tooltip("The food on the ledge (a salmon slice). Saved inactive in the scene.")]
    [SerializeField] private Transform food;
    [SerializeField] private Sprite heartEmote;
    [SerializeField] private float eatSeconds = 2.0f;
    [Tooltip("The food gets smaller in this many bites until it is gone.")]
    [SerializeField] private int biteCount = 4;
    [SerializeField] private float heartSeconds = 1.5f;
    [Tooltip("How far in front of the pet the food is placed (meters, toward the viewer).")]
    [SerializeField] private float foodDistanceInFrontMeters = 0.035f;

    [Header("Play")]
    [SerializeField] private ParticleSystem starParticles;
    [SerializeField] private Sprite starEmote;
    [SerializeField] private Sprite happyEmote;
    [SerializeField] private AudioClip playSoundFirst;
    [SerializeField] private AudioClip playSoundSecond;
    [SerializeField] private float playSeconds = 2.5f;
    [Tooltip("The happy hop at the start of Play: how high (meters) and how long (seconds).")]
    [SerializeField] private float hopHeightMeters = 0.02f;
    [SerializeField] private float hopSeconds = 0.4f;

    [Header("Sleep")]
    [SerializeField] private Sprite sleepEmote;
    [SerializeField] private float lieDownSeconds = 0.6f;
    [SerializeField] private float standUpSeconds = 0.6f;
    [Tooltip("Lifts the pet a little while lying on its side so it does not sink into the ledge.")]
    [SerializeField] private float lyingLiftMeters = 0.02f;

    // Facing the viewer (180) and rolled 90 degrees onto its side.
    private const float FacingViewerYaw = 180f;
    private const float LyingRollDegrees = 90f;

    private Coroutine actionRoutine;
    private Vector3 foodFullScale;

    private void Awake()
    {
        foodFullScale = food.localScale;
        food.gameObject.SetActive(false);
    }

    // ---------- Feed ----------

    public void StartEating()
    {
        StopRunningAction();
        actionRoutine = StartCoroutine(EatRoutine());
    }

    private IEnumerator EatRoutine()
    {
        petHunger.ResetHunger();
        petWander.FaceViewer();

        // Put the food on the ledge right in front of the pet, at full size.
        Vector3 petPosition = transform.localPosition;
        food.localPosition = new Vector3(petPosition.x, 0f, petPosition.z - foodDistanceInFrontMeters);
        food.localScale = foodFullScale;
        food.gameObject.SetActive(true);

        // Start the eat clip from its first frame. Passing 0 as the start time makes Unity
        // restart the clip even if the Animator is still in "Eat" from the meal before.
        animator.CrossFade("Eat", 0.1f, 0, 0f);

        // Eat it bite by bite: wait a moment, then the food shrinks a step.
        // One bite lasts as long as one loop of the eat clip, so each chew matches a bite.
        // After the last bite it is gone, right when the eating time is over.
        float secondsPerBite = eatSeconds / biteCount;
        for (int bite = 1; bite <= biteCount; bite++)
        {
            yield return new WaitForSeconds(secondsPerBite * 0.6f);

            float sizeLeft = 1f - (float)bite / biteCount;
            Vector3 smallerScale = foodFullScale * sizeLeft;
            yield return ScaleFoodTo(smallerScale, secondsPerBite * 0.4f);
        }

        // The food is gone, so stop chewing straight away.
        food.gameObject.SetActive(false);
        animator.CrossFade("Idle", 0.1f);
        petEmote.ShowForSeconds(heartEmote, heartSeconds);

        actionRoutine = null;
        petStateMachine.RequestState(PetState.Wandering);
    }

    // ---------- Play ----------

    public void StartPlaying()
    {
        StopRunningAction();
        actionRoutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        petWander.FaceViewer();
        animator.CrossFade("Dance", 0.1f, 0, 0f);

        // Stars burst around the pet.
        Vector3 petPosition = transform.localPosition;
        starParticles.transform.localPosition = new Vector3(petPosition.x, 0.04f, petPosition.z);
        starParticles.Play();

        float halfTime = playSeconds / 2f;

        PlaySound(playSoundFirst);
        petEmote.Show(starEmote);

        // One happy hop at the start of the dance, then keep dancing for the rest of the first half.
        yield return HappyHop();
        yield return new WaitForSeconds(halfTime - hopSeconds);

        PlaySound(playSoundSecond);
        petEmote.Show(happyEmote);
        yield return new WaitForSeconds(halfTime);

        animator.CrossFade("Idle", 0.1f);
        petEmote.Hide();

        actionRoutine = null;
        petStateMachine.RequestState(PetState.Wandering);
    }

    // A short jump straight up and back down onto the ledge.
    private IEnumerator HappyHop()
    {
        Vector3 groundPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < hopSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / hopSeconds);
            float height = Mathf.Sin(progress * Mathf.PI) * hopHeightMeters;
            transform.localPosition = groundPosition + Vector3.up * height;
            yield return null;
        }

        transform.localPosition = groundPosition;
    }

    // ---------- Sleep ----------

    public void StartSleeping()
    {
        StopRunningAction();
        actionRoutine = StartCoroutine(LieDownRoutine());
    }

    private IEnumerator LieDownRoutine()
    {
        animator.CrossFade("Idle", 0.15f);
        petEmote.Show(sleepEmote);
        petEyes.FallAsleep();

        // Always lie down toward the middle of the ledge, so the body never falls onto the rocks
        // at the ends. A positive roll tips the body to the right (+X), a negative roll to the left.
        bool isOnRightHalf = transform.localPosition.x > 0f;
        float rollDegrees = isOnRightHalf ? -LyingRollDegrees : LyingRollDegrees;

        Quaternion lyingRotation = Quaternion.Euler(0f, FacingViewerYaw, rollDegrees);
        Vector3 position = transform.localPosition;
        Vector3 lyingPosition = new Vector3(position.x, lyingLiftMeters, position.z);

        yield return MoveToPose(lyingRotation, lyingPosition, lieDownSeconds);
        actionRoutine = null;
    }

    // ---------- Wake up ----------

    // Called when entering the WakingUp state. The pet stands up smoothly,
    // then asks for Wandering (PetHunger moves it to Hungry if it got hungry while asleep).
    public void StartWakingUp()
    {
        StopRunningAction();
        actionRoutine = StartCoroutine(StandUpRoutine());
    }

    private IEnumerator StandUpRoutine()
    {
        petEmote.Hide();
        petEyes.WakeUp();

        Quaternion standingRotation = Quaternion.Euler(0f, FacingViewerYaw, 0f);
        Vector3 position = transform.localPosition;
        Vector3 standingPosition = new Vector3(position.x, 0f, position.z);

        yield return MoveToPose(standingRotation, standingPosition, standUpSeconds);

        actionRoutine = null;
        petStateMachine.RequestState(PetState.Wandering);
    }

    // ---------- Helpers ----------

    private IEnumerator ScaleFoodTo(Vector3 targetScale, float seconds)
    {
        Vector3 startScale = food.localScale;
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / seconds);
            food.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        food.localScale = targetScale;
    }

    // Smoothly turns and moves the pet to a pose over a number of seconds.
    private IEnumerator MoveToPose(Quaternion targetRotation, Vector3 targetPosition, float seconds)
    {
        Quaternion startRotation = transform.localRotation;
        Vector3 startPosition = transform.localPosition;

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / seconds));
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        transform.localRotation = targetRotation;
        transform.localPosition = targetPosition;
    }

    private void StopRunningAction()
    {
        if (actionRoutine != null)
        {
            StopCoroutine(actionRoutine);
            actionRoutine = null;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
