using System.Collections;
using UnityEngine;

// Closes the pet's eyes while it sleeps.
// The Kenney cat has its eyes painted on the body mesh, so the eye triangles were split off
// into their own objects (EyeLeft, EyeRight) with the pivot in the middle of each eye.
// Closing an eye squashes it flat on Y until it is a thin line, like a cartoon eye shut tight.
public class PetEyes : MonoBehaviour
{
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    [SerializeField] private float closeSeconds = 0.4f;
    [Tooltip("Eye height when fully closed, compared to open (1). A small value leaves a thin line.")]
    [SerializeField] private float closedEyeHeight = 0.08f;
    [Tooltip("Color multiplied onto the eyes when fully closed. It turns the yellow eye into a dark line, so it reads as shut and not as squinting.")]
    [SerializeField] private Color closedEyeTint = new Color(0.18f, 0.23f, 0.66f, 1f);

    [Header("Sleepy flutter while asleep")]
    [SerializeField] private float minSecondsBetweenFlutters = 2.5f;
    [SerializeField] private float maxSecondsBetweenFlutters = 5f;
    [Tooltip("How far the eyes open during a flutter: 0 stays closed, 1 opens fully.")]
    [SerializeField] private float flutterOpenAmount = 0.45f;
    [SerializeField] private float flutterMoveSeconds = 0.35f;

    // How closed the eyes are: 0 is fully open, 1 is fully closed.
    private const float Open = 0f;
    private const float Closed = 1f;

    // The eye color lives in the URP material's "_BaseColor". A MaterialPropertyBlock changes it
    // for the two eye renderers only, without touching the material the rest of the cat uses.
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private Coroutine eyesRoutine;
    private float currentClosedAmount = Open;
    private Renderer leftEyeRenderer;
    private Renderer rightEyeRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Color openEyeColor;

    private void Awake()
    {
        leftEyeRenderer = leftEye.GetComponent<Renderer>();
        rightEyeRenderer = rightEye.GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        openEyeColor = leftEyeRenderer.sharedMaterial.GetColor(BaseColorId);

        SetEyes(Open);
    }

    public void FallAsleep()
    {
        StopEyesRoutine();
        eyesRoutine = StartCoroutine(SleepRoutine());
    }

    public void WakeUp()
    {
        StopEyesRoutine();
        eyesRoutine = StartCoroutine(MoveEyes(Open, closeSeconds));
    }

    private IEnumerator SleepRoutine()
    {
        yield return MoveEyes(Closed, closeSeconds);

        while (true)
        {
            float waitSeconds = Random.Range(minSecondsBetweenFlutters, maxSecondsBetweenFlutters);
            yield return new WaitForSeconds(waitSeconds);

            // Peek a little, hold for a moment, then close again.
            yield return MoveEyes(Closed - flutterOpenAmount, flutterMoveSeconds);
            yield return new WaitForSeconds(0.3f);
            yield return MoveEyes(Closed, flutterMoveSeconds);
        }
    }

    private IEnumerator MoveEyes(float targetClosedAmount, float seconds)
    {
        float startClosedAmount = currentClosedAmount;
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / seconds));
            SetEyes(Mathf.Lerp(startClosedAmount, targetClosedAmount, progress));
            yield return null;
        }
        SetEyes(targetClosedAmount);
    }

    private void SetEyes(float closedAmount)
    {
        currentClosedAmount = closedAmount;
        float eyeHeight = Mathf.Lerp(1f, closedEyeHeight, closedAmount);
        leftEye.localScale = new Vector3(1f, eyeHeight, 1f);
        rightEye.localScale = new Vector3(1f, eyeHeight, 1f);

        // Fade the eyes from their normal color to dark as they close.
        Color closedEyeColor = openEyeColor * closedEyeTint;
        Color eyeColor = Color.Lerp(openEyeColor, closedEyeColor, closedAmount);
        SetEyeColor(leftEyeRenderer, eyeColor);
        SetEyeColor(rightEyeRenderer, eyeColor);
    }

    private void SetEyeColor(Renderer eyeRenderer, Color color)
    {
        eyeRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorId, color);
        eyeRenderer.SetPropertyBlock(propertyBlock);
    }

    private void StopEyesRoutine()
    {
        if (eyesRoutine != null)
        {
            StopCoroutine(eyesRoutine);
            eyesRoutine = null;
        }
    }
}
