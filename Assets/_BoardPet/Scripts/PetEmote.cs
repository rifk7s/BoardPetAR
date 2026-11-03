using System.Collections;
using UnityEngine;

// Shows one small speech bubble above the pet's head.
// - Show(emote): a ready-made Kenney emote that already has its bubble (heart, star, music, Zzz).
// - ShowAlertWithItem(alert, item): one empty bubble with an alert sign on top and an item
//   below it, both at the same time. Used when hungry: the alert catches the eye, the fish says why.
// The bubble always turns to face the camera so it stays readable from any angle.
public class PetEmote : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bubbleRenderer;
    [SerializeField] private SpriteRenderer alertRenderer;
    [SerializeField] private SpriteRenderer itemRenderer;
    [SerializeField] private Sprite emptyBubble;

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
        bubbleRenderer.enabled = true;
        alertRenderer.enabled = false;
        itemRenderer.enabled = false;
    }

    // Shows one empty bubble with the alert on top and the item below, until Hide() is called.
    public void ShowAlertWithItem(Sprite alertIcon, Sprite item)
    {
        StopHideLater();
        bubbleRenderer.sprite = emptyBubble;
        bubbleRenderer.enabled = true;
        alertRenderer.sprite = alertIcon;
        alertRenderer.enabled = true;
        itemRenderer.sprite = item;
        itemRenderer.enabled = true;
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
        bubbleRenderer.enabled = false;
        alertRenderer.enabled = false;
        itemRenderer.enabled = false;
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
        Camera mainCamera = Camera.main;
        if (mainCamera != null && bubbleRenderer.enabled)
        {
            // Copy the camera rotation so the bubble is always flat to the screen.
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
