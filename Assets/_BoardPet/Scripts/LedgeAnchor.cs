using UnityEngine;
using Vuforia;

// Puts everything that lives on the marker ledge (cave, rocks, pet) in the right place.
//
// The card is taped UPRIGHT on the whiteboard, so for the image target:
//   local Y points out of the board toward the viewer
//   local Z points to the top edge of the card (real-world up)
// This object is rotated 90 degrees around X, so its own up (Y) is real-world up
// and its own -Z points out of the board toward the viewer.
public class LedgeAnchor : MonoBehaviour
{
    [Header("Tune these after measuring the real whiteboard (meters)")]
    [SerializeField] private float ledgeBelowCardBottomMeters = 0.05f;
    [SerializeField] private float ledgeOutFromBoardMeters = 0.03f;

    private const float DefaultCardHeightMeters = 0.21f;

    private void Awake()
    {
        PlaceOnLedge();
    }

    private void OnValidate()
    {
        PlaceOnLedge();
    }

    private void PlaceOnLedge()
    {
        float cardHeightMeters = GetCardHeightMeters();

        // Card center is the target origin, so the bottom edge is half the height below it (-Z).
        float distanceBelowCenter = cardHeightMeters / 2f + ledgeBelowCardBottomMeters;

        transform.localPosition = new Vector3(0f, ledgeOutFromBoardMeters, -distanceBelowCenter);
        transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private float GetCardHeightMeters()
    {
        ImageTargetBehaviour imageTarget = GetComponentInParent<ImageTargetBehaviour>();
        if (imageTarget == null)
        {
            return DefaultCardHeightMeters;
        }
        return imageTarget.GetSize().y;
    }
}
