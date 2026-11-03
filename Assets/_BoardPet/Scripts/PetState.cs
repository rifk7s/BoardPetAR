// Everything the pet can be doing. Only PetStateMachine changes the current state.
public enum PetState
{
    Intro,      // before and while hopping out of the cave
    Wandering,  // walking back and forth on the ledge
    Hungry,     // stopped, gesture-negative, alert and fish bubble above the head
    Eating,     // eat clip, fish model, heart emote, hunger reset
    Playing,    // dance clip, star particles, star and happy face emotes
    Sleeping,   // lying down, Zzz emote
    WakingUp    // standing up again after Wake, then back to wandering
}
