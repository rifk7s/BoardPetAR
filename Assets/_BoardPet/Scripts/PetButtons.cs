using UnityEngine;

// Connects the Feed, Play and Sleep buttons to the pet.
// The buttons only ask PetStateMachine for a new state. They also follow the current state:
// hidden during the intro, greyed out when they cannot be used, and Sleep turns into Wake.
public class PetButtons : MonoBehaviour
{
    [SerializeField] private PetStateMachine petStateMachine;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonBar;
    [SerializeField] private UnityEngine.UI.Button feedButton;
    [SerializeField] private UnityEngine.UI.Button playButton;
    [SerializeField] private UnityEngine.UI.Button sleepButton;
    [SerializeField] private TMPro.TMP_Text sleepButtonLabel;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private static readonly Color AvailableColor = Color.white;
    private static readonly Color LockedColor = new Color(0.45f, 0.45f, 0.45f, 0.85f);

    private void Awake()
    {
        feedButton.onClick.AddListener(OnFeedPressed);
        playButton.onClick.AddListener(OnPlayPressed);
        sleepButton.onClick.AddListener(OnSleepPressed);
    }

    private void OnEnable()
    {
        petStateMachine.StateChanged += UpdateButtons;
        UpdateButtons(petStateMachine.CurrentState);
    }

    private void OnDisable()
    {
        petStateMachine.StateChanged -= UpdateButtons;
    }

    private void OnFeedPressed()
    {
        PlayClick();
        petStateMachine.RequestState(PetState.Eating);
    }

    private void OnPlayPressed()
    {
        PlayClick();
        petStateMachine.RequestState(PetState.Playing);
    }

    private void OnSleepPressed()
    {
        PlayClick();

        if (petStateMachine.CurrentState == PetState.Sleeping)
        {
            // Wake: the pet stands up, then wanders again. If it got hungry while asleep,
            // PetHunger moves it to Hungry right after.
            petStateMachine.RequestState(PetState.WakingUp);
        }
        else
        {
            petStateMachine.RequestState(PetState.Sleeping);
        }
    }

    // Matches the button table in the plan.
    private void UpdateButtons(PetState state)
    {
        bool showButtons = state != PetState.Intro;
        buttonBar.SetActive(showButtons);

        bool canFeed = state == PetState.Wandering || state == PetState.Hungry;
        bool canPlay = state == PetState.Wandering;
        bool canSleepOrWake = state == PetState.Wandering || state == PetState.Sleeping;

        SetButtonAvailable(feedButton, canFeed);
        SetButtonAvailable(playButton, canPlay);
        SetButtonAvailable(sleepButton, canSleepOrWake);

        bool isAsleepOrWaking = state == PetState.Sleeping || state == PetState.WakingUp;
        sleepButtonLabel.text = isAsleepOrWaking ? "Wake" : "Sleep";
    }

    // A locked button cannot be pressed and is drawn grey, so it is clear why nothing happens.
    private void SetButtonAvailable(UnityEngine.UI.Button button, bool isAvailable)
    {
        button.interactable = isAvailable;
        button.image.color = isAvailable ? AvailableColor : LockedColor;
    }

    private void PlayClick()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
