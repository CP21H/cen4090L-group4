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

    private bool[] botActive = { true, true, true, true };

    // UI references
    public TextMeshProUGUI potText;
    public TextMeshProUGUI playerChipsText;
    public TextMeshProUGUI[] botChipsTexts;
    public TextMeshProUGUI turnIndicatorText;

    public int smallBlind = 50;
    public int bigBlind = 100;
    private int dealerIndex = 0;
    private int smallBlindIndex;
    private int bigBlindIndex;

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
        playerCard1.sprite = GetCardSprite(deck[0]);
        playerCard2.sprite = GetCardSprite(deck[1]);

        botOneCard1.sprite = cardBack;
        botOneCard2.sprite = cardBack;

        botTwoCard1.sprite = cardBack;
        botTwoCard2.sprite = cardBack;

        botThreeCard1.sprite = cardBack;
        botThreeCard2.sprite = cardBack;

        botFourCard1.sprite = cardBack;
        botFourCard2.sprite = cardBack;

        for (int i = 0; i < 10; i++)
        {
            deck.RemoveAt(0);
        }

        for (int i = 0; i < 5; i++)
        {
            communityCards[i].sprite = cardBack;
            deck.RemoveAt(0);
        }
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


void BotFold(int botIndex)
{
    Debug.Log($"Bot {botIndex} folds.");
    botActive[botIndex - 1] = false; // Mark bot as inactive

    // Optionally hide the bot's cards
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

void BotRaise(int botIndex, int raiseAmount)
{
    Debug.Log($"Bot {botIndex} raises.");
    if (raiseAmount >= currentBet + minimumBet && botChips[botIndex - 1] >= raiseAmount)
    {
        currentBet = raiseAmount;
        botChips[botIndex - 1] -= raiseAmount;
        potSize += raiseAmount;
        UpdateUI();
        
    }
    else
    {
        Debug.Log($"Bot {botIndex} attempted an invalid raise.");
    }
}


private bool isProcessingTurn = false; // Add a flag to track ongoing turns

public void BotAction(int botIndex)
{
    if (!botActive[botIndex - 1]) return; // Skip inactive bots

    Debug.Log($"Bot {botIndex}'s turn to act.");

    int randomAction = Random.Range(0, 3); // 0 = Fold, 1 = Call, 2 = Raise
    switch (randomAction)
    {
        case 0: // Bot folds
            Debug.Log($"Bot {botIndex} folds.");
            botActive[botIndex - 1] = false;
            break;
        case 1: // Bot calls
            Debug.Log($"Bot {botIndex} calls.");
            BotCall(botIndex);
            break;
        case 2: // Bot raises
            int raiseAmount = Random.Range(minimumBet, Mathf.Min(currentBet + minimumBet, botChips[botIndex - 1]));
            Debug.Log($"Bot {botIndex} raises to {raiseAmount}.");
            BotRaise(botIndex, raiseAmount);
            break;
    }

    UpdateUI();

    // Increment index and move to the next turn
    currentPlayerIndex = (currentPlayerIndex + 1) % 5;
    StartNextTurn();
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
    currentPlayerIndex = (dealerIndex + 1) % 5; // Start with the player after the dealer
    bettingRoundInProgress = true;
    Debug.Log("Starting betting round.");
    StartNextTurn();
}

void StartNextTurn()
{
    Debug.Log("StartNextTurn called.");

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

    // Skip inactive bots or players
    while (currentPlayerIndex != 0 && !botActive[currentPlayerIndex - 1])
    {
        Debug.Log($"Skipping inactive bot {currentPlayerIndex}.");
        currentPlayerIndex = (currentPlayerIndex + 1) % 5;
    }

    // Highlight the current player or bot
    if (currentPlayerIndex == 0) // Player's turn
    {
        turnIndicatorText.text = "Player's Turn";
        playerHighlight.SetActive(true);
        Debug.Log("Waiting for player's action...");
        // Do not call StartNextTurn() here, wait for player's input
    }
    else // Bot's turn
    {
        turnIndicatorText.text = $"Bot {currentPlayerIndex}'s Turn";
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

        // Let the bot act
        BotAction(currentPlayerIndex);
    }
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
    Debug.Log("Player folded. Moving to next turn.");
    currentPlayerIndex = (currentPlayerIndex + 1) % 5; // Move to the next player
    StartNextTurn();
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
        Debug.Log("Player doesn't have enough chips to call.");
    }

    currentPlayerIndex = (currentPlayerIndex + 1) % 5; // Move to the next player
    StartNextTurn();
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
        Debug.LogError("Invalid raise amount!");
    }

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

