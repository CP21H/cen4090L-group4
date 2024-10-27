using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    public Button foldButton;   // Button for Fold
    public Button callButton;   // Button for Call
    public Button raiseButton;  // Button for Raise

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
        deckManager.AdvanceGameFlow();
    }

    void Call()
    {
        Debug.Log("Player calls");
        // Logic for player calling, matching the current bet
        deckManager.PlayerCalled();
        deckManager.AdvanceGameFlow();
    
    }

    void Raise()
    {
        Debug.Log("Player raises");
        deckManager.PlayerRaised(); 
        deckManager.AdvanceGameFlow(); 
    }






}
