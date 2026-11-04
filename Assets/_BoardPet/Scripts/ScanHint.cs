using UnityEngine;

// The "point your camera at the card" hint.
// It shows a small picture of the marker card with a cyan scan line sweeping over it.
// It is visible at the start, fades out when the marker is found, and fades back in
// when the marker has been lost for a moment. The card also floats up and down a little.
// Hooked to the image target's OnTargetFound and OnTargetLost events.
public class ScanHint : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform card;
    [SerializeField] private RectTransform scanLine;
    [SerializeField] private RectTransform scanArea;

    [Header("Timing (seconds)")]
    [SerializeField] private float fadeSeconds = 0.3f;
    [Tooltip("Wait this long after the marker is lost before showing the hint again, so it does not flicker.")]
    [SerializeField] private float showAgainDelaySeconds = 1f;
    [SerializeField] private float scanLineSeconds = 1.6f;

    [Header("Floating")]
    [SerializeField] private float floatPixels = 8f;
    [SerializeField] private float floatSpeed = 2f;

    private bool wantsToBeVisible = true;
    private float lostTime = 0f;
    private Vector2 cardStartPosition;

    private void Awake()
    {
        cardStartPosition = card.anchoredPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    // Called from OnTargetFound.
    public void HideHint()
    {
        wantsToBeVisible = false;
    }

    // Called from OnTargetLost.
    public void ShowHintSoon()
    {
        wantsToBeVisible = true;
        lostTime = Time.time;
    }

    private void Update()
    {
        UpdateFade();
        UpdateFloat();
        UpdateScanLine();
    }

    private void UpdateFade()
    {
        bool delayIsOver = Time.time - lostTime >= showAgainDelaySeconds;
        float targetAlpha = (wantsToBeVisible && delayIsOver) ? 1f : 0f;
        float step = Time.deltaTime / fadeSeconds;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, step);
    }

    private void UpdateFloat()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatPixels;
        card.anchoredPosition = cardStartPosition + new Vector2(0f, offset);
    }

    private void UpdateScanLine()
    {
        // Sweep from the bottom of the picture to the top and back again.
        float halfHeight = scanArea.rect.height / 2f;
        float progress = Mathf.PingPong(Time.time / scanLineSeconds, 1f);
        float y = Mathf.Lerp(-halfHeight, halfHeight, progress);
        scanLine.anchoredPosition = new Vector2(0f, y);
    }
}
