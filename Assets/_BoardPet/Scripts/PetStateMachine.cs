using System;
using UnityEngine;

// The only script that decides what the pet is doing.
// Other scripts call RequestState(...). This script checks whether the change is allowed,
// stops the old behaviour, starts the new one, and tells listeners (like the buttons).
public class PetStateMachine : MonoBehaviour
{
    [SerializeField] private PetWander petWander;
    [SerializeField] private Animator animator;
    [SerializeField] private PetEmote petEmote;
    [SerializeField] private PetActions petActions;

    [Header("Icons")]
    [SerializeField] private Sprite hungryAlertIcon;
    [SerializeField] private Sprite hungryIcon;

    public PetState CurrentState { get; private set; } = PetState.Intro;

    // Raised after every successful state change. PetButtons listens to this.
    public event Action<PetState> StateChanged;

    public bool RequestState(PetState newState)
    {
        if (!IsChangeAllowed(CurrentState, newState))
        {
            Debug.Log("[PetStateMachine] Ignored request " + CurrentState + " -> " + newState);
            return false;
        }

        ExitState(CurrentState);
        CurrentState = newState;
        EnterState(newState);

        if (StateChanged != null)
        {
            StateChanged.Invoke(newState);
        }
        return true;
    }

    // The transition table from the plan. Anything not listed here is refused.
    private bool IsChangeAllowed(PetState from, PetState to)
    {
        switch (from)
        {
            case PetState.Intro:
                return to == PetState.Wandering;

            case PetState.Wandering:
                return to == PetState.Hungry
                    || to == PetState.Eating
                    || to == PetState.Playing
                    || to == PetState.Sleeping;

            case PetState.Hungry:
                return to == PetState.Eating;

            case PetState.Eating:
                return to == PetState.Wandering;

            case PetState.Playing:
                return to == PetState.Wandering;

            case PetState.Sleeping:
                return to == PetState.WakingUp;

            case PetState.WakingUp:
                return to == PetState.Wandering;

            default:
                return false;
        }
    }

    private void EnterState(PetState state)
    {
        switch (state)
        {
            case PetState.Wandering:
                petWander.StartWandering();
                break;

            case PetState.Hungry:
                // Stop, turn to the camera, shake the head now and then,
                // and show one bubble with an alert on top and a fish below.
                petWander.FaceViewer();
                animator.CrossFade("Hungry", 0.15f);
                petEmote.ShowAlertWithItem(hungryAlertIcon, hungryIcon);
                break;

            case PetState.Eating:
                // Ends by itself and requests Wandering when the meal is finished.
                petActions.StartEating();
                break;

            case PetState.Playing:
                // Ends by itself and requests Wandering when the dance is finished.
                petActions.StartPlaying();
                break;

            case PetState.Sleeping:
                // Stays asleep until the Wake button requests WakingUp.
                petActions.StartSleeping();
                break;

            case PetState.WakingUp:
                // Stands up smoothly, then requests Wandering by itself.
                petActions.StartWakingUp();
                break;
        }
    }

    private void ExitState(PetState state)
    {
        switch (state)
        {
            case PetState.Wandering:
                petWander.StopWandering();
                break;

            case PetState.Hungry:
                petEmote.Hide();
                break;
        }
    }
}
