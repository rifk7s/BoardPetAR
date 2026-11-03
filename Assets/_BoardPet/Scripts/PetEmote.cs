using System.Collections;
using UnityEngine;

// Shows one small speech bubble above the pet's head.
// - Show(emote): a ready-made Kenney emote that already has its bubble (heart, star, music, Zzz).
// - ShowAlertWithItem(alert, item): one empty bubble with an alert sign on top and an item
//   below it, both at the same time. Used when hungry: the alert catches the eye, the fish says why.
// The bubble always turns to face the camera so it stays readable from any angle.
//
// Parts are shown and hidden by turning their GameObjects on and off, not just their renderers.
// Vuforia turns every renderer under the image target back on when the marker is found,
// but it never activates GameObjects that are switched off.
public class PetEmote : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bubbleRenderer;
    [SerializeField] private SpriteRenderer alertRenderer;
    [SerializeField] private SpriteRenderer itemRenderer;
    [SerializeField] private Sprite emptyBubble;

    [Header("Position")]
    [Tooltip("The cube pet's head and body are one block ('body'). The bubble follows its center.")]
    [SerializeField] private Renderer headRenderer;
    [Tooltip("LedgeAnchor: its up direction is real-world up, even when the pet lies down.")]
    [SerializeField] private Transform ledgeAnchor;
    [SerializeField] private float gapAboveHeadMeters = 0.06f;

    private Coroutine hideLaterRoutine;

    private void Awake()
    {
        Hide();
    }

    // Shows a full emote (bubble included) until Hide() is called.
    public void Show(Sprite emote)
    {
        StopHideLater();
        bubbleRenderer.sprite = emote;
        SetPartsVisible(true, false, false);
    }

    // Shows one empty bubble with the alert on top and the item below, until Hide() is called.
    public void ShowAlertWithItem(Sprite alertIcon, Sprite item)
    {
        StopHideLater();
        bubbleRenderer.sprite = emptyBubble;
        alertRenderer.sprite = alertIcon;
        itemRenderer.sprite = item;
        SetPartsVisible(true, true, true);
    }

    // Shows a full emote, then hides it by itself after a few seconds.
    public void ShowForSeconds(Sprite emote, float seconds)
    {
        Show(emote);
        hideLaterRoutine = StartCoroutine(HideAfterSeconds(seconds));
    }

    public void Hide()
    {
        StopHideLater();
        SetPartsVisible(false, false, false);
    }

    private void SetPartsVisible(bool showBubble, bool showAlert, bool showItem)
    {
        bubbleRenderer.gameObject.SetActive(showBubble);
        alertRenderer.gameObject.SetActive(showAlert);
        itemRenderer.gameObject.SetActive(showItem);
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hideLaterRoutine = null;
        Hide();
    }

    private void StopHideLater()
    {
        if (hideLaterRoutine != null)
        {
            StopCoroutine(hideLaterRoutine);
            hideLaterRoutine = null;
        }
    }

    private void LateUpdate()
    {
        if (!bubbleRenderer.gameObject.activeSelf)
        {
            return;
        }

        // Float straight above the head in real-world up. When the pet lies on its side to sleep,
        // the head block moves sideways and lower, and the bubble follows it.
        Vector3 headCenter = headRenderer.bounds.center;
        transform.position = headCenter + ledgeAnchor.up * gapAboveHeadMeters;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Copy the camera rotation so the bubble is always flat to the screen.
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
