using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class DeckManager : MonoBehaviour
{
    public int playerChips = 1000; 
    public int[] botChips = {1000, 1000, 1000, 1000}; 
    public int potSize= 0; 
    public int minimumBet = 50;  // example min bet
    public int currentBet = 0;   // current high bet

    private bool[] botActive = { true, true, true, true };

    // ui reference to pota and chips 
    public TextMeshProUGUI potText;
    public TextMeshProUGUI playerChipsText;
    public TextMeshProUGUI[] botChipsTexts;
    public TextMeshProUGUI turnIndicatorText; 

    public int smallBlind = 50;  // Default small blind amount
    public int bigBlind = 100;  // Default big blind amount (double the small blind)
    private int dealerIndex = 0; // Tracks the dealer position
    private int smallBlindIndex; // Tracks the player with the small blind
    private int bigBlindIndex;   // Tracks the player with the big blind

    private int smallBlindPlayer = 0; 
    private int bigBlindPlayer = 1;
    public TextMeshProUGUI smallBlindText;
    public TextMeshProUGUI bigBlindText;

    public TextMeshProUGUI CurrentSmall;
    public TextMeshProUGUI CurrentBigBlind;

    public TextMeshProUGUI moveHistoryText;



    // Card class representing individual cards
    public class Card
    {
        public string Suit { get; private set; }
        public string Rank { get; private set; }

        public Card(string suit, string rank)
        {
            Suit = suit;  // e.g., "club", "spade", etc.
            Rank = rank;  // e.g., "7", "A", "K", etc.
        }
    }

    // List to hold the deck of cards
    public List<Card> deck = new List<Card>();

    // Reference to UI elements for player cards and community cards
    public Image playerCard1;
    public Image playerCard2;
    public Image[] communityCards;  // For Flop, Turn, and River (5 community cards)
    
    [SerializeField] private Image botOneCard1;
    [SerializeField] private Image botOneCard2;
    [SerializeField] private Image botTwoCard1;
    [SerializeField] private Image botTwoCard2;
    [SerializeField] private Image botThreeCard1;
    [SerializeField] private Image botThreeCard2;
    [SerializeField] private Image botFourCard1;
    [SerializeField] private Image botFourCard2;

    // Example card sprites (you'll need to set these in the Inspector)
    public Sprite cardBack;  // Back of card sprite
    public Sprite[] cardSprites;  // Array of sprites for all 52 cards

    void Start()
    {
        CreateDeck();
        ShuffleDeck();
        DealInitialCards();
        SetBlinds();
    }

    // Method to create the deck
    void CreateDeck()
    {
        string[] suits = { "Heart", "Diamond", "Club", "Spade" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "1" };

        // Loop through suits and ranks to create 52 cards
        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                deck.Add(new Card(suit, rank));
            }
        }
    }

    // Method to shuffle the deck
    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // Method to deal initial cards to the player and bots
    void DealInitialCards()
    {
        // Deal 2 cards to the player
        playerCard1.sprite = GetCardSprite(deck[0]);
        playerCard2.sprite = GetCardSprite(deck[1]);

        // Deal 2 cards to each bot
            botOneCard1.sprite = cardBack;
            botOneCard2.sprite = cardBack;

            botTwoCard1.sprite = cardBack;
            botTwoCard2.sprite = cardBack;

            botThreeCard1.sprite = cardBack;
            botThreeCard2.sprite = cardBack;

            botFourCard1.sprite = cardBack;
            botFourCard2.sprite = cardBack;



        

        // Remove dealt cards from the deck (2 cards for each player and bot = 10 cards)
        for (int i = 0; i < 10; i++)
        {
            deck.RemoveAt(0);
        }

        // Deal 5 community cards (Flop, Turn, River)
        for (int i = 0; i < 5; i++)
        {
            communityCards[i].sprite = cardBack;
        }

        // Remove community cards from the deck
        for (int i = 0; i < 5; i++)
        {
            deck.RemoveAt(0);  // Removing the top card after it's dealt
        }
    }

    // Method to get the card sprite
    Sprite GetCardSprite(Card card)
    {
        string cardName = card.Rank + "_" + card.Suit.ToLower();  // No need to convert rank to lowercase

        Debug.Log("Looking for card sprite: " + cardName);  // Debug message

        foreach (var sprite in cardSprites)
        {
            if (sprite.name == cardName)
            {
                Debug.Log("Found sprite: " + sprite.name);  // Debug message for a successful match
                return sprite;
            }
        }

        Debug.LogWarning("No sprite found for: " + cardName);  // Warning if no sprite matches
        return cardBack;  // If no matching sprite is found, return the card back as a fallback
    }

    void SetBlinds()
    {
    smallBlindIndex = (dealerIndex + 1) % 5; // Small blind is the next player
    bigBlindIndex = (dealerIndex + 2) % 5;   // Big blind is the one after small blind

    // Deduct blinds from chips and add to pot
    if (smallBlindIndex == 0) // Player is small blind
    {
        playerChips -= smallBlind;
        potSize += smallBlind;
    }
    else // Bot is small blind
    {
        botChips[smallBlindIndex - 1] -= smallBlind;
        potSize += smallBlind;
    }

    if (bigBlindIndex == 0) // Player is big blind
    {
        playerChips -= bigBlind;
        potSize += bigBlind;
        currentBet = bigBlind; // Set current bet to big blind amount
    }
    else // Bot is big blind
    {
        botChips[bigBlindIndex - 1] -= bigBlind;
        potSize += bigBlind;
        currentBet = bigBlind; // Set current bet to big blind amount
    }

    UpdateUI();
    }

private void UpdateBlinds()
{
    // Update the blind player indices
    smallBlindPlayer = (smallBlindPlayer + 1) % (botChips.Length + 1); // Move to the next player
    bigBlindPlayer = (smallBlindPlayer + 1) % (botChips.Length + 1);

    // Update the UI
    smallBlindText.text = $"Small Blind: {(smallBlindPlayer == 0 ? "Player" : $"Bot {smallBlindPlayer}")}";
    bigBlindText.text = $"Big Blind: {(bigBlindPlayer == 0 ? "Player" : $"Bot {bigBlindPlayer}")}";

    Debug.Log($"Small Blind: {(smallBlindPlayer == 0 ? "Player" : $"Bot {smallBlindPlayer}")}, " +
              $"Big Blind: {(bigBlindPlayer == 0 ? "Player" : $"Bot {bigBlindPlayer}")}");

    // Deduct the blinds from the respective players
    DeductBlinds();
}









private void DeductBlinds()
{
    int smallBlindAmount = minimumBet;
    int bigBlindAmount = minimumBet * 2;

    // Deduct chips from the small blind player
    if (smallBlindPlayer == 0) // Player
        playerChips -= smallBlindAmount;
    else // Bot
        botChips[smallBlindPlayer - 1] -= smallBlindAmount;

    // Deduct chips from the big blind player
    if (bigBlindPlayer == 0) // Player
        playerChips -= bigBlindAmount;
    else // Bot
        botChips[bigBlindPlayer - 1] -= bigBlindAmount;

    // Add blinds to the pot
    potSize += smallBlindAmount + bigBlindAmount;

    // Update the UI to reflect new chip counts
    UpdateUI();
}









    private void ProcessBotActions()
    { 
        for (int i = 0; i< botActive.Length; i++)
        { 
            if(botActive[i])
            { 
                BotAction(i + 1); 
            }
        }
    }

    private bool AllBotsFolded()
    {
        foreach (bool isActive in botActive)
        {
            if (isActive)
            return false; // If any bot is active, return false
        }
        return true; // All bots have folded
    }






    // Player action methods
    public void PlayerFolded()
    {
        Debug.Log("Player folded. Moving to next player/bot.");
        // Logic to handle folding
    }

    public void PlayerCalled()
    {
        Debug.Log("Player called. Proceeding with next action.");
        // Logic to handle calling
    }

    public void PlayerRaised()
    {
        Debug.Log("Player raised the bet.");
        // Logic to handle raising
    }
    
    private int currentRound = 0; 

    public void AdvanceGameFlow()
    {
        if(currentRound == 0) // Pre-Flop stage
    {
        UpdateBlinds(); // Set blinds at the start of the game
    }

        if(AllBotsFolded()){ 
        Debug.Log("All bots have folded. Skipping to the next round.");
        currentRound++;
    }

        switch (currentRound)
        {
            case 0: // Pre-Flop complete, move to Flop
                turnIndicatorText.text = "Bot 1's Turn"; 
                ProcessBotActions();  // Call bot actions for the Pre-Flop stage
                RevealFlop();
                currentRound++;
                break;
            case 1: // Flop complete, move to Turn
                turnIndicatorText.text = "Bot 1's Turn"; 
                ProcessBotActions();  // Call bot actions for the Flop stage
                RevealTurn();
                currentRound++;
                break;
            case 2: // Turn complete, move to River
                turnIndicatorText.text = "Bot 1's Turn"; 
                ProcessBotActions();  // Call bot actions for the Turn stage
                RevealRiver();
                currentRound++;
                break;
            case 3: // River complete, move to Showdown
                turnIndicatorText.text = "Bot 1's Turn"; 
                ProcessBotActions();  // Call bot actions for the River stage
                PerformShowdown();
            break;
    }
    }


    public AudioClip soundFX;
    void RevealFlop()
    {
        AudioSource.PlayClipAtPoint(soundFX, Camera.main.transform.position);
        communityCards[0].sprite = GetCardSprite(deck[0]);
        communityCards[1].sprite = GetCardSprite(deck[1]);
        communityCards[2].sprite = GetCardSprite(deck[2]);
        Debug.Log("Flop revealed");
    }

    void RevealTurn()
    {
        communityCards[3].sprite = GetCardSprite(deck[3]);
        Debug.Log("Turn revealed");
    }

    void RevealRiver()
    {
        communityCards[4].sprite = GetCardSprite(deck[4]);
        Debug.Log("River revealed");
    }
    void RevealBotCards(int botIndex)
        {
            switch (botIndex)
                {
                    case 1:
                        botOneCard1.sprite = GetCardSprite(deck[2]);
                        botOneCard2.sprite = GetCardSprite(deck[3]);
                        break;
                     case 2:
                        botTwoCard1.sprite = GetCardSprite(deck[4]);
                        botTwoCard2.sprite = GetCardSprite(deck[5]);
                        break;
                    case 3:
                        botThreeCard1.sprite = GetCardSprite(deck[6]);
                        botThreeCard2.sprite = GetCardSprite(deck[7]);
                        break;
                    case 4:
                        botFourCard1.sprite = GetCardSprite(deck[8]);
                        botFourCard2.sprite = GetCardSprite(deck[9]);
                        break;
            }
        }

    void PerformShowdown()
    {
        Debug.Log("Showdown! Comparing hands to determine the winner.");

    // Reveal each bot's cards if they are still active
        for (int i = 0; i < botActive.Length; i++)
        {
            if (botActive[i])
            {
                RevealBotCards(i + 1); // Bot IDs are 1-based
            }
        }

    // Assume player and bots are in a list for comparison
        List<int> activePlayers = new List<int> { 0 }; // 0 for the player
        for (int i = 0; i < botActive.Length; i++)
        {
            if (botActive[i])
            activePlayers.Add(i + 1); // Adding active bots to the list
        }

        // For simplicity, assume a method EvaluateHand that returns a hand ranking
        int bestPlayer = -1;
        int bestHandRank = -1;

        foreach (int player in activePlayers)
        {
            int handRank = EvaluateHand(player);
            if (handRank > bestHandRank)
            {
                bestHandRank = handRank;
                bestPlayer = player;
            }
        }   

        if (bestPlayer == 0)
        {
            Debug.Log("The player wins the pot!");
            playerChips += potSize; // Award pot to player
        }
        else
        {
            Debug.Log("Bot " + bestPlayer + " wins the pot!");
            botChips[bestPlayer - 1] += potSize; // Award pot to the winning bot
        }

        potSize = 0; // Reset pot for the next round
        UpdateUI();
        ResetGame();
        }














    
    

    public void BotAction(int botNumber)
    {
        turnIndicatorText.text = "Bot " + botNumber + "'s Turn";

        ExecuteWithDelay(5.0f, () =>
        {
            int randomAction = Random.Range(0, 3);  // 0 = Fold, 1 = Call, 2 = Raise

            switch (randomAction)
            {
                case 0:
                    BotFold(botNumber);
                    break;
                case 1:
                    BotCall(botNumber);
                    break;
                case 2:
                    int raiseAmount = Random.Range(minimumBet, minimumBet * 3);
                    BotRaise(botNumber, raiseAmount); 
                    break;
            }
        UpdateUI(); 

        // After the bot's action, advance the game flow
        ExecuteWithDelay(1.0f, AdvanceGameFlow);
        }); 
    }
    public void UpdateUI()
    {
        potText.text = "Pot: $" + potSize;
        playerChipsText.text = "$" + playerChips;

        for (int i = 0; i < botChips.Length; i++)
        {
        botChipsTexts[i].text = "$" + botChips[i];
        }

        smallBlindText.text = "Small Blind: $" + smallBlind;
        bigBlindText.text = "Big Blind: $" + bigBlind;
}


    public void PlayerCall()
    {
        int callAmount = currentBet;
        if (playerChips >= callAmount)
        {
        playerChips -= callAmount;
        potSize += callAmount;
        UpdateUI();
        Debug.Log("Player calls with " + callAmount + " chips.");
        }
        else
        {
        Debug.Log("Player does not have enough chips to call.");
        // Consider all-in logic here
        }
        UpdateMoveHistory("Player calls with $" + currentBet);

    }

    public void PlayerRaise(int raiseAmount)
    {
    if (raiseAmount >= currentBet + minimumBet && playerChips >= raiseAmount)
    {
        currentBet = raiseAmount;
        playerChips -= raiseAmount;
        potSize += raiseAmount;
        UpdateUI();
        Debug.Log("Player raises to " + currentBet + " chips.");
    }
    else
    {
        Debug.Log("Raise amount must be at least " + (currentBet + minimumBet));
        }
    UpdateMoveHistory("Player raises to $" + currentBet);

    }




    public void PlayerFold()
    {
        Debug.Log("Player folds");
    // Logic to skip the player's turn or remove them from the current hand
        UpdateMoveHistory("Player folds");

    }


    public void BotCall(int botIndex)
        {
    // Ensure the botIndex is within the valid range
        if (botIndex < 0 || botIndex >= botChips.Length)
        {
            Debug.LogError("Bot index is out of bounds: " + botIndex);
            return;
        }

        int callAmount = currentBet;
        if (botChips[botIndex] >= callAmount)
        {
            botChips[botIndex] -= callAmount;
            potSize += callAmount;
            UpdateUI();
            Debug.Log("Bot " + botIndex + " calls with " + callAmount + " chips.");
        }
        else
        {
        Debug.Log("Bot " + botIndex + " does not have enough chips to call.");
        // Consider handling this scenario, e.g., bot goes all-in
        }
        UpdateMoveHistory("Bot " + (botIndex + 1) + " calls with $" + currentBet);
    }

    public void BotRaise(int botIndex, int raiseAmount)
    {
        if (raiseAmount >= currentBet + minimumBet && botChips[botIndex] >= raiseAmount)
    {
        currentBet = raiseAmount;
        botChips[botIndex] -= raiseAmount;
        potSize += raiseAmount;
        UpdateUI();
        Debug.Log("Bot " + (botIndex + 1) + " raises to " + currentBet + " chips.");
    }
        else
    {
        Debug.Log("Bot " + (botIndex + 1) + " attempted an invalid raise.");
    }
        UpdateMoveHistory("Bot " + (botIndex + 1) + " raises to $" + currentBet);

    }

    public void BotFold(int botIndex)
    {
        Debug.Log("Bot " + botIndex + " folds");
        botActive[botIndex - 1] = false; 
        switch (botIndex)
        {
            case 1:
                botOneCard1.gameObject.SetActive(false);
                botOneCard2.gameObject.SetActive(false);
                break;
            case 2:
                botTwoCard1.gameObject.SetActive(false);
                botTwoCard2.gameObject.SetActive(false);
                break;
            case 3:
                botThreeCard1.gameObject.SetActive(false);
                botThreeCard2.gameObject.SetActive(false);
                break;
            case 4:
                botFourCard1.gameObject.SetActive(false);
                botFourCard2.gameObject.SetActive(false);
                break;
            }

    // Remove the bot from further actions in the current round
    // (botActive array already ensures this during action processing)
        UpdateMoveHistory("Bot " + (botIndex + 1) + " folds");

    }

    

        // Method to evaluate hand rank (simplified)
    int EvaluateHand(int player)
    {
        // Placeholder logic for hand evaluation
        // You would need to implement actual poker hand evaluation logic here
        // Return an integer representing the rank of the hand (higher is better)
        return Random.Range(1, 10); // Randomized for testing
    }
    void ResetGame()
    {
        Debug.Log("Resetting the game for the next round.");
        currentRound = 0;
        botActive = new bool[] { true, true, true, true }; // Reset bots as active
        dealerIndex = (dealerIndex + 1) % 5; // Move dealer to the next player
        CreateDeck();
        ShuffleDeck();
        DealInitialCards();
        SetBlinds(); // Set blinds for the next round
        UpdateUI();
    }



    // Coroutine to wait for a certain amount of time
    private IEnumerator WaitForAction(float seconds, System.Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();  // Execute the action after the delay
    }

    // Call this method to start the coroutine
    public void ExecuteWithDelay(float delay, System.Action action)
    {
        StartCoroutine(WaitForAction(delay, action));
    }
    public void UpdateMoveHistory(string action)
    {
    // Append the new action to the existing text
    moveHistoryText.text += action + "\n";

    // Optional: Automatically scroll to the bottom
    Canvas.ForceUpdateCanvases();
    moveHistoryText.rectTransform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
}





}












































