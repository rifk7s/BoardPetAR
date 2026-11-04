using System;
using System.Globalization;
using UnityEngine;

// Keeps track of when the pet last ate, and asks for the Hungry state when it is time.
// The last feed time is saved on the phone with PlayerPrefs, in UTC, so hunger keeps
// counting even while the app is closed.
public class PetHunger : MonoBehaviour
{
    [SerializeField] private PetStateMachine petStateMachine;

    [Tooltip("Seconds after eating before the pet becomes hungry. Kept short for the demo.")]
    [SerializeField] private float hungerDurationSeconds = 20f;

    private const string LastFeedTimeKey = "BoardPet.LastFeedUtcTicks";

    private DateTime lastFeedTimeUtc;

    public bool IsHungry
    {
        get { return GetSecondsSinceLastFeed() >= hungerDurationSeconds; }
    }

    private void Awake()
    {
        LoadLastFeedTime();
    }

    private void Update()
    {
        // Only ask for Hungry while wandering. If hunger runs out while the pet is eating,
        // playing or sleeping, this check simply passes again once it is back to wandering.
        if (petStateMachine.CurrentState == PetState.Wandering && IsHungry)
        {
            petStateMachine.RequestState(PetState.Hungry);
        }
    }

    // Called when the pet eats (stage C). Hunger becomes full again and the time is saved.
    public void ResetHunger()
    {
        lastFeedTimeUtc = DateTime.UtcNow;
        SaveLastFeedTime();
    }

    private double GetSecondsSinceLastFeed()
    {
        return (DateTime.UtcNow - lastFeedTimeUtc).TotalSeconds;
    }

    private void LoadLastFeedTime()
    {
        // PlayerPrefs has no long type, so the UTC ticks are stored as a string.
        string savedTicks = PlayerPrefs.GetString(LastFeedTimeKey, "");

        long ticks;
        bool isNumber = long.TryParse(savedTicks, NumberStyles.Integer, CultureInfo.InvariantCulture, out ticks);
        bool isValidTime = isNumber && ticks > 0 && ticks <= DateTime.MaxValue.Ticks;

        if (isValidTime)
        {
            lastFeedTimeUtc = new DateTime(ticks, DateTimeKind.Utc);
        }
        else
        {
            // First launch, or the saved value is broken: treat the pet as just fed.
            ResetHunger();
        }
    }

    private void SaveLastFeedTime()
    {
        string ticksText = lastFeedTimeUtc.Ticks.ToString(CultureInfo.InvariantCulture);
        PlayerPrefs.SetString(LastFeedTimeKey, ticksText);
        PlayerPrefs.Save();
    }
}
