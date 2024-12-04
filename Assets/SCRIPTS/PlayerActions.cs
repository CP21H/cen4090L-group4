using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActions : MonoBehaviour
{
    public Button foldButton;   // Button for Fold
    public Button callButton;   // Button for Call
    public Button raiseButton;  // Button for Raise
    public TMP_InputField raiseInputField; // Optional input field for raise
    public Slider raiseSlider;  // Slider for raise amount
    public TextMeshProUGUI sliderValueText; // Text to display the slider value

    private DeckManager deckManager;  // Reference to DeckManager for game flow control

    void Start()
    {
        // Assuming DeckManager is attached to a GameObject called GameManager
        deckManager = GameObject.Find("GameManager").GetComponent<DeckManager>();

        // Attach the button listeners
        foldButton.onClick.AddListener(Fold);
        callButton.onClick.AddListener(Call);
        raiseButton.onClick.AddListener(Raise);

        // Attach slider listener
        raiseSlider.onValueChanged.AddListener(UpdateSliderText);

        // Initialize slider values
        raiseSlider.minValue = deckManager.minimumBet;
        raiseSlider.maxValue = deckManager.playerChips;
        raiseSlider.value = deckManager.minimumBet;
        UpdateSliderText(raiseSlider.value);
    }

    void Fold()
    {
        Debug.Log("Player folds");
        // Handle folding logic, perhaps notify DeckManager to skip player's turn
        deckManager.PlayerFolded();
        deckManager.ExecuteWithDelay(1.0f, deckManager.AdvanceGameFlow);
    }

    void Call()
    {
        Debug.Log("Player calls");
        // Logic for player calling, matching the current bet
        deckManager.PlayerCalled();
        deckManager.ExecuteWithDelay(1.0f, deckManager.AdvanceGameFlow);
    }

   void Raise()
    {
    int raiseAmount = Mathf.RoundToInt(raiseSlider.value); // Use slider value for the raise
    if (raiseAmount > 0 && raiseAmount <= deckManager.playerChips)
    {
        deckManager.PlayerRaise(raiseAmount);
        deckManager.ExecuteWithDelay(5.0f, deckManager.AdvanceGameFlow);
    }
    else
    {
        Debug.Log("Raise amount is invalid or exceeds available chips.");
    }
}


    void UpdateSliderText(float value)
    {
        sliderValueText.text = "$" + Mathf.RoundToInt(value); // Update the slider value text
    }

public void UpdateSliderRange()
{
    raiseSlider.minValue = deckManager.currentBet + deckManager.minimumBet;
    raiseSlider.maxValue = deckManager.playerChips;

    if (raiseSlider.value < raiseSlider.minValue)
    {
        raiseSlider.value = raiseSlider.minValue;
    }

    UpdateSliderText(raiseSlider.value);
}
}

