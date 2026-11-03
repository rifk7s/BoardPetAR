using System;
using UnityEngine;

// The only script that decides what the pet is doing.
// Other scripts call RequestState(...). This script checks whether the change is allowed,
// stops the old behaviour, starts the new one, and tells listeners (like the buttons).
public class PetStateMachine : MonoBehaviour
{
    [SerializeField] private PetWander petWander;

    public PetState CurrentState { get; private set; } = PetState.Intro;

    // Raised after every successful state change. The buttons will listen to this in stage C.
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

            // Hungry, Eating, Playing and Sleeping are added in stages B and C.
        }
    }

    private void ExitState(PetState state)
    {
        switch (state)
        {
            case PetState.Wandering:
                petWander.StopWandering();
                break;
        }
    }
}
