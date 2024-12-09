using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DeckManager : MonoBehaviour
{
    public int playerChips = 1000;
    public int[] botChips = { 1000, 1000, 1000, 1000 };
    public int potSize = 0;
    public int minimumBet = 50;
    public int currentBet = 0;

    private bool[] botActive = { true, true, true, true, true };    //added position for player 

    // UI references
    public TextMeshProUGUI potText;
    public TextMeshProUGUI playerChipsText;
    public TextMeshProUGUI[] botChipsTexts;
    public TextMeshProUGUI turnIndicatorText;

    public int smallBlind = 50;
    public int bigBlind = 100;
    private int dealerIndex = 0;    //initializes player as the dealer
    private int smallBlindIndex = 0;
    private int bigBlindIndex = 1;
    public List<Card> communityCardData = new List<Card>();


    private int smallBlindPlayer = 0;
    private int bigBlindPlayer = 1;
    public TextMeshProUGUI smallBlindText;
    public TextMeshProUGUI bigBlindText;

    public TextMeshProUGUI CurrentSmall;
    public TextMeshProUGUI CurrentBigBlind;

    

    public GameObject playerHighlight;
    public GameObject botHighlight1;
    public GameObject botHighlight2;
    public GameObject botHighlight3;
    public GameObject botHighlight4;


    private int currentPlayerIndex = 0;
    private bool bettingRoundInProgress = false;

    [SerializeField] private TextMeshProUGUI playerBlindIndicator;

    [SerializeField] private TextMeshProUGUI[] botBlindIndicators;



    public class Card
    {
        public string Suit { get; private set; }
        public string Rank { get; private set; }

        public Card(string suit, string rank)
        {
            Suit = suit;
            Rank = rank;
        }
    }

    public List<Card> deck = new List<Card>();

    public Image playerCard1;
    public Image playerCard2;
    public Image[] communityCards;

    [SerializeField] private Image botOneCard1;
    [SerializeField] private Image botOneCard2;
    [SerializeField] private Image botTwoCard1;
    [SerializeField] private Image botTwoCard2;
    [SerializeField] private Image botThreeCard1;
    [SerializeField] private Image botThreeCard2;
    [SerializeField] private Image botFourCard1;
    [SerializeField] private Image botFourCard2;

    public Sprite cardBack;
    public Sprite[] cardSprites;

    private int currentRound = 0;

    void Start()
    {
        CreateDeck();
        ShuffleDeck();
        DealInitialCards();
        SetBlinds();
        StartBettingRound();
    }

    void CreateDeck()
    {
        string[] suits = { "Heart", "Diamond", "Club", "Spade" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                deck.Add(new Card(suit, rank));
            }
        }
    }

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

void DealInitialCards()
{
    // Ensure there are enough cards in the deck
    if (deck.Count < 15)
    {
        Debug.LogError("Not enough cards in the deck to deal!");
        return;
    }

    // Clear any existing data
    communityCardData.Clear();

    // Assign the first two cards to the player
    playerCard1.sprite = GetCardSprite(deck[0]);
    playerCard2.sprite = GetCardSprite(deck[1]);
    Debug.Log($"Dealt to player: {deck[0].Suit} {deck[0].Rank}, {deck[1].Suit} {deck[1].Rank}");

    // Assign two cards to each bot
    botOneCard1.sprite = cardBack;
    botOneCard2.sprite = cardBack;

    botTwoCard1.sprite = cardBack;
    botTwoCard2.sprite = cardBack;

    botThreeCard1.sprite = cardBack;
    botThreeCard2.sprite = cardBack;

    botFourCard1.sprite = cardBack;
    botFourCard2.sprite = cardBack;

    // Remove the dealt cards (2 per player = 10 cards)
    deck.RemoveRange(0, 10);

    // Assign the next 5 cards to the community (Flop, Turn, River)
    for (int i = 0; i < 5; i++)
    {
        communityCards[i].sprite = cardBack; // Initialize with card back
        communityCardData.Add(deck[i + 10]); // Add card data to communityCardData
    }
    //ResetCardVisibility();
    Debug.Log("Community cards dealt and initialized.");
}


void ResetCardVisibility()
{
    // Ensure all player and bot cards are visible
    playerCard1.enabled = true;
    playerCard2.enabled = true;

    botOneCard1.enabled = true;
    botOneCard2.enabled = true;

    botTwoCard1.enabled = true;
    botTwoCard2.enabled = true;

    botThreeCard1.enabled = true;
    botThreeCard2.enabled = true;

    botFourCard1.enabled = true;
    botFourCard2.enabled = true;

    // Ensure all community cards are visible
    foreach (var communityCard in communityCards)
    {
        communityCard.enabled = true;
    }
}

public void HideBotCards(int botIndex)
{
    // Hide specific bot's cards when they fold
    switch (botIndex)
    {
        case 1:
            botOneCard1.enabled = false;
            botOneCard2.enabled = false;
            break;
        case 2:
            botTwoCard1.enabled = false;
            botTwoCard2.enabled = false;
            break;
        case 3:
            botThreeCard1.enabled = false;
            botThreeCard2.enabled = false;
            break;
        case 4:
            botFourCard1.enabled = false;
            botFourCard2.enabled = false;
            break;
    }
    Debug.Log($"Bot {botIndex}'s cards are now hidden.");
}

public void HidePlayerCards()
{
    playerCard1.enabled = false;
    playerCard2.enabled = false;
    Debug.Log("Player's cards are now hidden.");
}





    Sprite GetCardSprite(Card card)
    {
        string cardName = card.Rank + "_" + card.Suit.ToLower();

        foreach (var sprite in cardSprites)
        {
            if (sprite.name == cardName)
            {
                return sprite;
            }
        }

        return cardBack;
    }

    void SetBlinds()
    {
        smallBlindIndex = (dealerIndex + 1) % 5;
        bigBlindIndex = (dealerIndex + 2) % 5;

        if (smallBlindIndex == 0)
        {
            playerChips -= smallBlind;
            potSize += smallBlind;
        }
        else
        {
            botChips[smallBlindIndex - 1] -= smallBlind;
            potSize += smallBlind;
        }

        if (bigBlindIndex == 0)
        {
            playerChips -= bigBlind;
            potSize += bigBlind;
            currentBet = bigBlind;
        }
        else
        {
            botChips[bigBlindIndex - 1] -= bigBlind;
            potSize += bigBlind;
            currentBet = bigBlind;
        }

        UpdateUI();
    }

public void UpdateRaiseSlider(Slider slider)
{
    slider.maxValue = playerChips; // Update slider max value to match player's chips
}

private void UpdateBlinds()
{
    Debug.Log("UpdateBlinds called"); // Keep the debug log for confirmation

    // Clear previous indicators
    playerBlindIndicator.text = "";
    foreach (var indicator in botBlindIndicators)
    {
        indicator.text = "";
    }

    // Update the blind player indices
    smallBlindPlayer = (smallBlindPlayer + 1) % (botChips.Length + 1); // Rotate to the next player
    bigBlindPlayer = (smallBlindPlayer + 1) % (botChips.Length + 1); // Big blind follows small blind

    Debug.Log($"Small Blind Player: {smallBlindPlayer}, Big Blind Player: {bigBlindPlayer}");

    // Update and set indicators for the new blinds
    if (smallBlindPlayer == 0)
    {
        playerBlindIndicator.text = "Small Blind";
        Debug.Log("Player is Small Blind");
    }
    else
    {
        botBlindIndicators[smallBlindPlayer - 1].text = "Small Blind";
        Debug.Log($"Bot {smallBlindPlayer} is Small Blind");
    }

    if (bigBlindPlayer == 0)
    {
        playerBlindIndicator.text = "Big Blind";
        Debug.Log("Player is Big Blind");
    }
    else
    {
        botBlindIndicators[bigBlindPlayer - 1].text = "Big Blind";
        Debug.Log($"Bot {bigBlindPlayer} is Big Blind");
    }

    // Deduct blinds for the new round
    DeductBlinds();
}




// Resets all blind indicators (hides them)
private void ResetBlindIndicators()
{
    playerBlindIndicator.gameObject.SetActive(false);
    foreach (var indicator in botBlindIndicators)
    {
        indicator.gameObject.SetActive(false);
    }
}


private void DeductBlinds()
{
    int smallBlindAmount = minimumBet;
    int bigBlindAmount = minimumBet * 2;

    // Deduct chips from the small blind player
    if (smallBlindPlayer == 0) // Player is the small blind
    {
        playerChips -= smallBlindAmount;
        Debug.Log($"Player pays small blind: {smallBlindAmount}");
    }
    else // A bot is the small blind
    {
        botChips[smallBlindPlayer - 1] -= smallBlindAmount;
        Debug.Log($"Bot {smallBlindPlayer} pays small blind: {smallBlindAmount}");
    }

    // Deduct chips from the big blind player
    if (bigBlindPlayer == 0) // Player is the big blind
    {
        playerChips -= bigBlindAmount;
        Debug.Log($"Player pays big blind: {bigBlindAmount}");
    }
    else // A bot is the big blind
    {
        botChips[bigBlindPlayer - 1] -= bigBlindAmount;
        Debug.Log($"Bot {bigBlindPlayer} pays big blind: {bigBlindAmount}");
    }

    // Add blinds to the pot
    potSize += smallBlindAmount + bigBlindAmount;

    // Update the UI to reflect new chip counts
    UpdateUI();
}




public void BotFold(int botIndex)
{
    //Debug.Log($"Bot {botIndex} folds.");
    botActive[botIndex - 1] = false; // Mark bot as inactive

    // Hide bot's cards based on index
    switch (botIndex)
    {
        case 1:
            botOneCard1.enabled = false; // Use 'enabled' to hide the Image
            botOneCard2.enabled = false;
            break;
        case 2:
            botTwoCard1.enabled = false;
            botTwoCard2.enabled = false;
            break;
        case 3:
            botThreeCard1.enabled = false;
            botThreeCard2.enabled = false;
            break;
        case 4:
            botFourCard1.enabled = false;
            botFourCard2.enabled = false;
            break;
    }

        //duplicate turn increment and call to start next turn
    /*currentPlayerIndex = (currentPlayerIndex + 1) % 5; // Move to the next player
    StartNextTurn();*/
}



void BotCall(int botIndex)
{
    Debug.Log($"Bot {botIndex} calls.");
    int callAmount = currentBet;

    if (botChips[botIndex - 1] >= callAmount)
    {
        botChips[botIndex - 1] -= callAmount;
        potSize += callAmount;
        UpdateUI();
        
    }
    else
    {
        Debug.Log($"Bot {botIndex} does not have enough chips to call.");
        // Optionally implement all-in logic here
    }
}

public void BotRaise(int botIndex, int raiseAmount)
{
    // Ensure the raise amount is valid (greater than or equal to the minimum bet and less than or equal to the bot's chips)
    if (raiseAmount >= currentBet + minimumBet && raiseAmount <= botChips[botIndex - 1])
    {
        currentBet = raiseAmount;
        botChips[botIndex - 1] -= raiseAmount;
        potSize += raiseAmount;
        Debug.Log($"Bot {botIndex} raises to {currentBet}.");
        UpdateUI();
    }
    else
    {
        Debug.Log($"Bot {botIndex} attempted an invalid raise of {raiseAmount}.");
        // Force the bot to take a valid action instead of skipping its turn
        if (botChips[botIndex - 1] >= currentBet)
        {
            Debug.Log($"Bot {botIndex} defaults to calling.");
            BotCall(botIndex); // Default to calling if raise is invalid
        }
        else
        {
            Debug.Log($"Bot {botIndex} defaults to folding due to insufficient chips.");
            BotFold(botIndex); // Fold if calling is not possible
        }
    }
}



private bool isProcessingTurn = false; // Add a flag to track ongoing turns

public void BotAction(int botIndex)
{
    if (!botActive[botIndex - 1]) return; // Skip inactive bots

    int botChipsRemaining = botChips[botIndex - 1];
    int callAmount = currentBet - (botChipsRemaining >= currentBet ? 0 : botChipsRemaining); // Amount bot needs to call
    int potOdds = potSize > 0 ? callAmount * 100 / potSize : 0; // Pot odds as a percentage
    int handStrength = EvaluateHandStrength(botIndex); // Evaluate the bot's hand strength (0-100)
    int aggressiveness = Random.Range(20, 80); // Adjust aggressiveness factor

    Debug.Log($"Bot {botIndex} hand strength: {handStrength}, pot odds: {potOdds}");

    // Decision logic
    if (handStrength < potOdds || botChipsRemaining < callAmount) // Weak hand or can't afford to call
    {
        Debug.Log($"Bot {botIndex} folds.");
        BotFold(botIndex);
    }
    else if (handStrength > aggressiveness) // Strong hand or aggressive bot    (smaller num = more aggressive)
    {
        int raiseAmount = Mathf.Min(currentBet + Random.Range(minimumBet, minimumBet * 3), botChipsRemaining);
        if (raiseAmount > currentBet) // Only raise if it's valid
        {
            Debug.Log($"Bot {botIndex} raises to {raiseAmount}.");
            BotRaise(botIndex, raiseAmount);
        }
        else // If the raise is invalid, default to calling
        {
            Debug.Log($"Bot {botIndex}'s raise amount was invalid. Defaulting to call.");
            BotCall(botIndex);
        }
    }
    else // Default action: Call
    {
        Debug.Log($"Bot {botIndex} calls.");
        BotCall(botIndex);
    }

    currentPlayerIndex = (currentPlayerIndex + 1) % 5; // Move to the next player
    StartNextTurn();
}


private int EvaluateHandStrength(int botIndex)
{
    // Get the bot's cards from the deck
    Card card1, card2;
    switch (botIndex)
    {
        case 1:
            card1 = deck[2];
            card2 = deck[3];
            break;
        case 2:
            card1 = deck[4];
            card2 = deck[5];
            break;
        case 3:
            card1 = deck[6];
            card2 = deck[7];
            break;
        case 4:
            card1 = deck[8];
            card2 = deck[9];
            break;
        default:
            Debug.LogError("Invalid bot index.");
            return 0;
    }

    // Base hand strength: Combine ranks
    int rank1 = GetCardRankValue(card1.Rank);
    int rank2 = GetCardRankValue(card2.Rank);
    int baseStrength = rank1 + rank2;

    // Pair bonus
    int pairBonus = (card1.Rank == card2.Rank) ? 30 : 0;

    // Suit bonus (same suit)
    int suitBonus = (card1.Suit == card2.Suit) ? 10 : 0;

    // Add community card strength
    int communityBonus = EvaluateCommunityCards(card1, card2);

    // Dynamic position bonus based on the round
    int positionBonus = EvaluatePositionBonus(botIndex);

    // Final hand strength calculation
    int handStrength = baseStrength + pairBonus + suitBonus + communityBonus + positionBonus;

    Debug.Log($"Bot {botIndex} hand: {card1.Rank} of {card1.Suit}, {card2.Rank} of {card2.Suit}. Strength: {handStrength}");
    return Mathf.Clamp(handStrength, 0, 100); // Ensure strength is within bounds
}


private int EvaluateCommunityCards(Card card1, Card card2)
{
    int bonus = 0;

    foreach (var communityCard in communityCardData)
    {
        // Add bonus for matching ranks
        if (card1.Rank == communityCard.Rank || card2.Rank == communityCard.Rank)
            bonus += 15;

        // Add bonus for matching suits
        if (card1.Suit == communityCard.Suit || card2.Suit == communityCard.Suit)
            bonus += 5;
    }

    return bonus;
}









private int EvaluatePositionBonus(int botIndex)
{
    // Early position is penalized slightly; later position is rewarded
    int positionBonus = 0;

    if (currentRound == 0) // Pre-Flop
        positionBonus = (5 - botIndex) * 2; // Later positions get a small bonus
    else if (currentRound == 1) // Flop
        positionBonus = (5 - botIndex) * 3;
    else if (currentRound == 2) // Turn
        positionBonus = (5 - botIndex) * 4;
    else if (currentRound == 3) // River
        positionBonus = (5 - botIndex) * 5;

    return positionBonus;
}

private int GetCardRankValue(string rank)
{
    // Map card ranks to numerical values
    switch (rank)
    {
        case "2": return 2;
        case "3": return 3;
        case "4": return 4;
        case "5": return 5;
        case "6": return 6;
        case "7": return 7;
        case "8": return 8;
        case "9": return 9;
        case "10": return 10;
        case "J": return 11; // Jack
        case "Q": return 12; // Queen
        case "K": return 13; // King
        case "A": return 14; // Ace
        default:
            Debug.LogError($"Invalid card rank: {rank}");
            return 0; // Default to 0 for invalid ranks
    }
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











    void StartBettingRound()
{
        //does this skip players turn if dealerindex is 0: (5 players + 1 dealer) %5?
    currentPlayerIndex = (dealerIndex + 1) % 5; // Start with the player after the dealer
    bettingRoundInProgress = true;
    Debug.Log("Starting betting round.");
    StartNextTurn();
}

void StartNextTurn()
{
    Debug.Log($"CurrentPlayerIndex: {currentPlayerIndex}");

    // Deactivate all highlights
    playerHighlight.SetActive(false);
    botHighlight1.SetActive(false);
    botHighlight2.SetActive(false);
    botHighlight3.SetActive(false);
    botHighlight4.SetActive(false);

    // Check if the round is complete
    if (IsRoundComplete())
    {
        Debug.Log("Round complete. Advancing game flow.");
        AdvanceGameFlow();
        return;
    }

    // Ensure currentPlayerIndex wraps correctly within bounds
    int loopCounter = 0; // Safety counter to avoid infinite loops
    while (loopCounter < botActive.Length + 1)
    {
        // If it's the player's turn and the player hasn't folded
        if (currentPlayerIndex == 0 && playerChips > 0 && playerCard1.enabled && playerCard2.enabled)
        {
            turnIndicatorText.text = "Player's Turn";
            playerHighlight.SetActive(true); // Highlight the player
            Debug.Log("Waiting for player input...");
            
            return; // STOP here and WAIT for player input
        }
        // If it's a bot's turn and the bot is active
        else if (currentPlayerIndex > 0 && currentPlayerIndex <= botActive.Length && botActive[currentPlayerIndex - 1])
        {
            turnIndicatorText.text = $"Bot {currentPlayerIndex}'s Turn";
            Debug.Log($"Bot {currentPlayerIndex}'s turn to act.");
            
            // Highlight the active bot
            switch (currentPlayerIndex)
            {
                case 1:
                    botHighlight1.SetActive(true);
                    break;
                case 2:
                    botHighlight2.SetActive(true);
                    break;
                case 3:
                    botHighlight3.SetActive(true);
                    break;
                case 4:
                    botHighlight4.SetActive(true);
                    break;
            }

            BotAction(currentPlayerIndex); // Perform bot action
            return;
        }

        // Move to the next player, wrapping around correctly
        currentPlayerIndex = (currentPlayerIndex + 1) % (botActive.Length + 1);
        loopCounter++;
    }

    // If no valid players or bots, log an error (shouldn't happen in a properly designed game)
    Debug.LogError("No valid players or bots found to take a turn!");
}





// Helper function to reset all highlights
void ResetHighlights()
{
    playerHighlight.SetActive(false);
    botHighlight1.SetActive(false);
    botHighlight2.SetActive(false);
    botHighlight3.SetActive(false);
    botHighlight4.SetActive(false);
}






bool AllPlayersOrBotsFolded()
{
    int activePlayers = botActive.Length;
    if (playerChips > 0) activePlayers++;
    foreach (bool bot in botActive)
    {
        if (!bot) activePlayers--;
    }

    return activePlayers <= 1;
}

public void PlayerFolded()
{
    Debug.Log("Player folded.");
    botActive[0] = false; // Mark player as inactive
    HidePlayerCards(); // Hide player's cards
    AdvanceTurn(); // Advance after player action
}

public void PlayerCalled()
{
    Debug.Log("Player called.");
    if (playerChips >= currentBet)
    {
        playerChips -= currentBet;
        potSize += currentBet;
        UpdateUI();
    }
    else
    {
        Debug.LogWarning("Player doesn't have enough chips to call.");
    }
    AdvanceTurn(); // Advance after player action
}

public void PlayerRaise(int raiseAmount)
{
    if (raiseAmount > 0 && playerChips >= raiseAmount)
    {
        currentBet += raiseAmount;
        playerChips -= raiseAmount;
        potSize += raiseAmount;
        Debug.Log($"Player raises to {currentBet}.");
        UpdateUI();
    }
    else
    {
        Debug.LogWarning("Invalid raise amount.");
    }
    AdvanceTurn(); // Advance after player action
}






void AdvanceTurn()
{
    currentPlayerIndex = (currentPlayerIndex + 1) % 5; // Move to the next player
    StartNextTurn();
}




bool IsRoundComplete()
{
    // Check if the player has acted
    bool playerActed = playerChips <= 0 || playerChips < currentBet;

    // Check if all bots have acted
    bool allBotsActed = true;
    foreach (bool bot in botActive)
    {
        if (bot)
        {
            allBotsActed = false;
            break;
        }
    }

    return playerActed && allBotsActed;
}

public void AdvanceGameFlow()
{
    if (!IsRoundComplete())
    {
        Debug.Log("Round is not complete. Continuing.");
        StartNextTurn();
        return;
    }

    Debug.Log("Advancing game flow.");

    switch (currentRound)
    {
        case 0: // Pre-Flop complete, move to Flop
            RevealFlop();
            currentRound++;
            break;
        case 1: // Flop complete, move to Turn
            RevealTurn();
            currentRound++;
            break;
        case 2: // Turn complete, move to River
            RevealRiver();
            currentRound++;
            break;
        case 3: // River complete, move to Showdown
            PerformShowdown();
            currentRound = 0; // Reset for the next game

            // Update blinds and start with the new small blind
            UpdateBlinds();
            currentPlayerIndex = smallBlindPlayer; // Betting starts with the small blind
            StartNextTurn(); // Begin the new betting round
            break;
    }
}



public AudioClip soundFX;

public void ExecuteWithDelay(float delay, System.Action action)
{
    if (action == null)
    {
        Debug.LogError("Action is null. Skipping delayed execution.");
        return;
    }

    StartCoroutine(WaitForAction(delay, action));
}

private IEnumerator WaitForAction(float seconds, System.Action action)
{
    yield return new WaitForSeconds(seconds);

    if (action != null)
    {
        action.Invoke();
    }
}

 

    public void UpdateUI()
    {
        potText.text = "Pot: $" + potSize;
        playerChipsText.text = "$" + playerChips;

        for (int i = 0; i < botChips.Length; i++)
        {
            botChipsTexts[i].text = "$" + botChips[i];
        }
    }

    void RevealFlop()
    {
        AudioSource.PlayClipAtPoint(soundFX, Camera.main.transform.position);
        communityCards[0].sprite = GetCardSprite(deck[0]);
        communityCards[1].sprite = GetCardSprite(deck[1]);
        communityCards[2].sprite = GetCardSprite(deck[2]);

            for (int i = 0; i < 3; i++)
            {
             deck.RemoveAt(0);
            }

    }

    void RevealTurn()
    {
        communityCards[3].sprite = GetCardSprite(deck[0]);
        deck.RemoveAt(0); // Remove the turn card from the deck

    }

    void RevealRiver()
    {
        communityCards[4].sprite = GetCardSprite(deck[0]);
        deck.RemoveAt(0); // Remove the turn card from the deck
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

int EvaluateHand(int playerIndex)
{
    Debug.Log($"Evaluating hand for player/bot {playerIndex}");
    return Random.Range(1, 10); // Placeholder: random ranking
}


void ResetGame()
{
    Debug.Log("Resetting game for the next round.");

    currentRound = 0;
    botActive = new bool[] { true, true, true, true }; // Reset bots as active
    dealerIndex = (dealerIndex + 1) % 5; // Move dealer position to the next player
    CreateDeck();
    ShuffleDeck();
    DealInitialCards();
    SetBlinds();
    UpdateUI();
    StartBettingRound(); // Start the first betting round of the new game
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
}

