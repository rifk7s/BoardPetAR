using System.Collections;
using UnityEngine;

// Makes the pet walk back and forth along the ledge on its own.
// The pet walks along the ledge's X axis (LedgeAnchor local space), stops for a moment
// at each end, turns around, and walks back.
public class PetWander : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Walking (meters, local to LedgeAnchor)")]
    [SerializeField] private float walkSpeedMetersPerSecond = 0.05f;
    [SerializeField] private float leftLimitMeters = -0.10f;
    [SerializeField] private float rightLimitMeters = 0.10f;

    [Header("Timing")]
    [SerializeField] private float pauseAtEndSeconds = 1.0f;
    [SerializeField] private float turnSeconds = 0.8f;

    // The pet model faces +Z, so facing +X (right) is a 90 degree turn around Y.
    // Facing left is 270 (not -90) so the pet always turns through 180, which is
    // facing the viewer, and never shows its back to the camera.
    private const float FaceRightYaw = 90f;
    private const float FaceLeftYaw = 270f;

    private Coroutine wanderRoutine;
    private bool isFacingRight = true;

    public void StartWandering()
    {
        if (wanderRoutine != null)
        {
            return;
        }

        wanderRoutine = StartCoroutine(WanderForever());
    }

    public void StopWandering()
    {
        if (wanderRoutine != null)
        {
            StopCoroutine(wanderRoutine);
            wanderRoutine = null;
        }

        PlayAnimation("Idle");
    }

    private IEnumerator WanderForever()
    {
        while (true)
        {
            yield return TurnToFaceWalkDirection();

            PlayAnimation("Walk");
            yield return WalkToEndOfLedge();

            PlayAnimation("Idle");
            yield return new WaitForSeconds(pauseAtEndSeconds);

            isFacingRight = !isFacingRight;
        }
    }

    private IEnumerator TurnToFaceWalkDirection()
    {
        float startYaw = transform.localEulerAngles.y;
        float targetYaw = isFacingRight ? FaceRightYaw : FaceLeftYaw;

        float elapsed = 0f;
        while (elapsed < turnSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / turnSeconds);
            float yaw = Mathf.Lerp(startYaw, targetYaw, progress);
            transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(0f, targetYaw, 0f);
    }

    private IEnumerator WalkToEndOfLedge()
    {
        float targetX = isFacingRight ? rightLimitMeters : leftLimitMeters;

        while (!Mathf.Approximately(transform.localPosition.x, targetX))
        {
            Vector3 position = transform.localPosition;
            position.x = Mathf.MoveTowards(position.x, targetX, walkSpeedMetersPerSecond * Time.deltaTime);
            transform.localPosition = position;
            yield return null;
        }
    }

    private void PlayAnimation(string stateName)
    {
        if (animator != null)
        {
            animator.CrossFade(stateName, 0.15f);
        }
    }
}
