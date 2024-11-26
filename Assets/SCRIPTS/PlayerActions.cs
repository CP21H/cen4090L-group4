using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerActions : MonoBehaviour
{
    public Button foldButton;   // Button for Fold
    public Button callButton;   // Button for Call
    public Button raiseButton;  // Button for Raise
    public TMP_InputField raiseInputField; 
    

    private DeckManager deckManager;  // Reference to DeckManager for game flow control

    void Start()
    {
        // Assuming DeckManager is attached to a GameObject called GameManager
        deckManager = GameObject.Find("GameManager").GetComponent<DeckManager>();

        // Attach the button listeners
        foldButton.onClick.AddListener(Fold);
        callButton.onClick.AddListener(Call);
        raiseButton.onClick.AddListener(Raise);
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
    int raiseAmount;
    if (int.TryParse(raiseInputField.text, out raiseAmount))
    {
        // Check if the raise amount is within a reasonable range (e.g., not exceeding player's chips)
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
    else
    {
        Debug.Log("Invalid raise amount.");
    }
}






}
