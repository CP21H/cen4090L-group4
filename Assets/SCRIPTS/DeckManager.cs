using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;


public class DeckManager : MonoBehaviour
{
    public int playerChips = 1000;
    public int[] botChips = { 1000, 1000, 1000, 1000 };
    public int potSize = 0;
    public int minimumBet = 50;
    public int currentBet = 0;
    public enum PlayerType{player, bot}
    public bool roundOver=false;
    public int playerIndex=2;
    public int numPlayers = 5;

    private bool[] botActive = { true, true, true, true, true };    //added position for player 

    // UI references
    public TextMeshProUGUI potText;
    public TextMeshProUGUI playerChipsText;
    public TextMeshProUGUI[] botChipsTexts;
    public TextMeshProUGUI turnIndicatorText;

    public int smallBlind = 50;
    public int bigBlind = 100;
    private int dealerIndex = 4;    //bot5 = dealer, bot 1 takes turn first
    private int smallBlindIndex = 0;
    private int bigBlindIndex = 1;
    private int playerContributedChips = 0;

    public List<Card> communityCardData = new List<Card>();
    //public int turnsTaken=0;


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
    [SerializeField] private TextMeshProUGUI callAmountText; 

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
    private bool playerFolded = false; // Tracks whether the player has folded

    public enum GamePhase
    {
        PreFlop,
        Flop,
        Turn,
        River,
        Showdown
    }

    private GamePhase currentPhase = GamePhase.PreFlop;
    private int turnsTakenInPhase = 0; // Tracks the number of turns in the current phase



    void Start()
    {
        Debug.Log("Game started");
        InitializeGame();
    }
    void InitializeGame()
    {
        CreateDeck();
        ShuffleDeck();
        DealInitialCards();
        ResetCardVisibility();
        SetBlinds();
        UpdateUI();
        //UpdateCallAmount();
        //UpdateRaiseSlider();
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
        Debug.Log("Dealing new cards for the next round.");

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
        int c=1;
        while(playerCard1.sprite == cardBack)   //error checking in case cardBack gets assigned
        {
            c++;
            playerCard1.sprite=GetCardSprite(deck[c]);
        }
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
        if (c>1)
        {
            deck.RemoveAt(c);
        }

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
            case 0:
            {
                botOneCard1.enabled = false;
                botOneCard2.enabled = false;
                break;
            }
            case 1:
            {
                botTwoCard1.enabled = false;
                botTwoCard2.enabled = false;
                break;
            }
            case 3:
            {
                botThreeCard1.enabled = false;
                botThreeCard2.enabled = false;
                break;
            }
            case 4:
            {
                botFourCard1.enabled = false;
                botFourCard2.enabled = false;
                break;
            }
            default:
            {
                Debug.LogWarning($"Error in [ HideBotCards() ]: default switch case executed\nGiven value {botIndex} for botIndex");
                break;
            }
        }
        if(botIndex<playerIndex){botIndex++;}
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
        //Debug.Log($"SBIndex: {smallBlindIndex}, BBIndex: {bigBlindIndex}");
        int sb = (dealerIndex + 1) % GetActivePlayerCount();
        int bb = (dealerIndex + 2) % GetActivePlayerCount();
        smallBlindIndex = sb;       //accomodates for playerIndex=2
        bigBlindIndex = bb;
        if(sb<playerIndex){smallBlindIndex++;}
        if(bb<playerIndex){bigBlindIndex++;}

        if (sb == playerIndex)
        {
            playerChips -= smallBlind;
            potSize += smallBlind;
        }
        else
        {
            botChips[smallBlindIndex - 1] -= smallBlind;
            potSize += smallBlind;
        }
        if (bb == playerIndex)
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
        UpdateCallAmount(); // Update the displayed call amount
    }




    public void UpdateRaiseSlider(Slider slider)
    {
        slider.minValue = minimumBet;
        slider.maxValue = playerChips; // Update slider max value to match player's chips
        if(playerChips == 0)
        {
            slider.minValue = slider.maxValue = 0;
        }
    }

    private void UpdateBlinds()
    {
        //Debug.Log("UpdateBlinds called"); // Keep the debug log for confirmation

        // Clear previous indicators
        playerBlindIndicator.text = "";
        foreach (var indicator in botBlindIndicators)
        {
            indicator.text = "";
        }
        
        // Update the blind player indices
        smallBlindPlayer = (smallBlindPlayer + 1) % (botChips.Length + 1); // Rotate to the next player
        bigBlindPlayer = (smallBlindPlayer + 1) % (botChips.Length + 1); // Big blind follows small blind
        int sb = smallBlindPlayer;
        int bb=bigBlindPlayer;

        Debug.Log($"Small Blind Player: {smallBlindPlayer}, Big Blind Player: {bigBlindPlayer}");

        // Update and set indicators for the new blinds
        if (smallBlindPlayer == playerIndex)
        {
            playerBlindIndicator.text = "Small Blind";
            Debug.Log("Player is Small Blind");
        }
        else
        {
            if(sb<playerIndex){sb++;}
            botBlindIndicators[sb - 1].text = "Small Blind";
            Debug.Log($"Bot {sb} is Small Blind");
        }

        if (bigBlindPlayer == playerIndex)
        {
            playerBlindIndicator.text = "Big Blind";
            Debug.Log("Player is Big Blind");
        }
        else
        {
            if(bb<playerIndex){bb++;}
            botBlindIndicators[bb - 1].text = "Big Blind";
            Debug.Log($"Bot {bb} is Big Blind");
        }

        // Deduct blinds for the new round
        DeductBlinds();
    }

    private void UpdateCallAmount()
    {
        int callAmount = currentBet - playerContributedChips;

        // Ensure the call amount doesn't go below zero
        callAmount = Mathf.Max(callAmount, 0);

        // Update the call amount display
        callAmountText.text = $"Amount to Call: ${callAmount}";   //not yet linked

        Debug.Log($"Updated Call Amount: {callAmount}");
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
        
        int sb = smallBlindPlayer;
        int bb = bigBlindPlayer;
        // Deduct chips from the small blind player
        if (smallBlindPlayer == playerIndex) // Player is the small blind
        {
            playerChips -= smallBlindAmount;
            Debug.Log($"Player pays small blind: {smallBlindAmount}");
        }
        else // A bot is the small blind
        {
            if(sb<playerIndex){sb++;}
            botChips[sb - 1] -= smallBlindAmount;
            Debug.Log($"Bot {sb} pays small blind: {smallBlindAmount}");
        }

        // Deduct chips from the big blind player
        if (bigBlindPlayer == playerIndex) // Player is the big blind
        {
            playerChips -= bigBlindAmount;
            Debug.Log($"Player pays big blind: {bigBlindAmount}");
        }
        else // A bot is the big blind
        {
            if(bb<playerIndex){bb++;}
            botChips[bb - 1] -= bigBlindAmount;
            Debug.Log($"Bot {bb} pays big blind: {bigBlindAmount}");
        }

        // Add blinds to the pot
        potSize += smallBlindAmount + bigBlindAmount;

        // Update the UI to reflect new chip counts
        UpdateUI();
    }




    public void BotFold(int botIndex)
    {
        //Debug.Log($"Bot {botIndex} folds.");
        int botNum=botIndex;
        if(botIndex<playerIndex){botNum++;}
        botActive[botIndex] = false; // Mark bot as inactive

        // Hide bot's cards based on index
        HideBotCards(botIndex);
        switch (botNum)
        {
            case 1:
            {
                botOneCard1.enabled = false; // Use 'enabled' to hide the Image
                botOneCard2.enabled = false;
                break;
            }
            case 2:
            {
                botTwoCard1.enabled = false;
                botTwoCard2.enabled = false;
                break;
            }
            case 3:
            {
                botThreeCard1.enabled = false;
                botThreeCard2.enabled = false;
                break;
            }
            case 4:
            {
                botFourCard1.enabled = false;
                botFourCard2.enabled = false;
                break;
            }
            default:
            {
                Debug.LogWarning($"Error in [BotFold(int botIndex)]: default case executed; value given{botIndex}");
                break;
            }

        }
    }



    void BotCall(int botIndex)
    {
        if(botIndex < playerIndex)
            botIndex++;
        Debug.Log($"Bot {botIndex} calls.");
        int callAmount = currentBet;

        if (botChips[botIndex - 1] >= callAmount)
        {
            botChips[botIndex - 1] -= callAmount;
            potSize += callAmount;    
            UpdateUI();    
        }
        else if(botChips[botIndex-1] > 0)  //implemented all in logic
        {
            potSize += botChips[botIndex - 1];
            botChips[botIndex - 1] = 0;
            Debug.Log($"Bot {botIndex} does not have enough chips to call. Going all in");
            UpdateUI();
        }
        else
        {
            Debug.Log($"Bot {botIndex} has gone all in, no chips left to bet");
            
        }
        
    }

    public void BotRaise(int botIndex, int raiseAmount)
    {
        int botNum = botIndex;
        //bool smallPlayer = false;
        if(botNum < playerIndex){botNum++;}
            //smallPlayer=true;
        
        // Ensure the raise amount is valid (greater than or equal to the minimum bet and less than or equal to the bot's chips)
        if (raiseAmount >= currentBet + minimumBet && raiseAmount <= botChips[botNum-1])/*raiseAmount <= botChips[botIndex - 1])*/
        {
            currentBet = raiseAmount;
            botChips[botNum-1] -= raiseAmount;
            potSize += raiseAmount;
            Debug.Log($"Bot {botNum} raises to {currentBet}.");
            UpdateCallAmount();
            UpdateUI();
        }
        else
        {
            Debug.Log($"Bot {botNum} attempted an invalid raise of {raiseAmount}.");
            // Force the bot to take a valid action instead of skipping its turn
            if (botChips[botNum - 1] > 0)
            {
                Debug.Log($"Bot {botNum} defaults to calling.");
                BotCall(botIndex); // Default to calling if raise is invalid
            }
            else if(botChips[botNum - 1] == 0)
            {
                Debug.Log($"Bot {botNum} has gone all in, insufficient chips to raise.");
                //BotFold(botIndex); // Fold if calling is not possible
            }
        }
    }



    //private bool isProcessingTurn = false; // Add a flag to track ongoing turns

    public void BotAction(int botIndex)
    {
        int botNum=botIndex;
        //bool smallPlayer=false;
        if(botIndex<playerIndex){botNum++;}
            //smallPlayer=true;
        

        if (!botActive[botIndex/*-1*/]) return; // Skip inactive bots
        //isProcessingTurn=true;
        int botChipsRemaining = botChips[botNum-1];
        int callAmount = currentBet - (botChipsRemaining >= currentBet ? 0 : botChipsRemaining); // Amount bot needs to call
        int potOdds = potSize > 0 ? callAmount * 100 / potSize : 0; // Pot odds as a percentage
        int handStrength = EvaluateHandStrength(botIndex); // Evaluate the bot's hand strength (0-100)
        int aggressiveness = Random.Range(20, 80); // Adjust aggressiveness factor

        Debug.Log($"Bot {botNum} hand strength: {handStrength}, pot odds: {potOdds}");

        //Bluff Factor
        bool isBluffing = Random.Range(0, 100) < 10; // 10% chance to bluff
        if (isBluffing)
    
        {
            int bluffRaise = Mathf.Min(currentBet + Random.Range(minimumBet, minimumBet * 3), botChipsRemaining);
            Debug.Log($"Bot {botNum} is bluffing and raises to {bluffRaise}.");
            BotRaise(botIndex, bluffRaise);
            return;
        }


        // Decision logic
        if (handStrength > potOdds - Random.Range(10, 30)) // Forgiving logic for marginal hands
        {
            Debug.Log($"Bot {botNum} calls due to marginal hand.");
            BotCall(botIndex);
        }
        else if ((handStrength < potOdds || botChipsRemaining < callAmount) /*&& currentRound>0*/) // Weak hand or can't afford to call
        {
            Debug.Log($"Bot {botNum} folds.");
            BotFold(botIndex);
        }
        else if (handStrength > aggressiveness) // Strong hand or aggressive bot    (smaller num = more aggressive)
        {
            int raiseAmount = Mathf.Min(currentBet + Random.Range(minimumBet, minimumBet * 3), botChipsRemaining);
            if (raiseAmount > currentBet) // Only raise if it's valid
            {
                Debug.Log($"Bot {botNum} raises to {raiseAmount}.");
                BotRaise(botIndex, raiseAmount);
            }
            else // If the raise is invalid, default to calling
            {
                Debug.Log($"Bot {botNum}'s raise amount was invalid. Defaulting to call.");
                BotCall(botIndex);
            }
        }
        else // Default action: Call
        {
            Debug.Log($"Bot {botNum} calls.");
            BotCall(botIndex);
        }
        turnsTakenInPhase++;
    }

    public int GetActivePlayerCount()
    {
        int playerCount = 0;
        foreach (bool isActive in botActive)
        {
            if (isActive)            
            playerCount++; // If any bot is active, return false
        }
        return playerCount;
    }


    private int EvaluateHandStrength(int botIndex)
    {
        int botNum=botIndex;
        if(botNum<playerIndex){botNum++;}
        else if(botIndex == playerIndex)
        {
            Debug.LogWarning($"Error in [ EvaluateHandStrength() ]: given playerIndex\nContinuing with Index {(--botIndex % numPlayers)}");
        }
        // Get the bot's cards from the deck
        Card card1, card2;
        switch (botNum)
        {
            case 1:
            {
                card1 = deck[2];
                card2 = deck[3];
                break;
            }
            case 2:
            {
                card1 = deck[4];
                card2 = deck[5];
                break;
            }
            case 3:
            {
                card1 = deck[6];
                card2 = deck[7];
                break;
            }
            case 4:
            {
                card1 = deck[8];
                card2 = deck[9];
                break;
            }
            default:
                Debug.LogError("Invalid bot index.");
                return 50;  // Default to a neutral strength
        }

        // Base hand strength: Combine ranks
        int rank1 = GetCardRankValue(card1.Rank);
        int rank2 = GetCardRankValue(card2.Rank);
        int baseStrength = rank1 + rank2;

        // Pair bonus
        int pairBonus = (card1.Rank == card2.Rank) ? 30 : 0;

        // Suit bonus (same suit)
        int suitBonus = (card1.Suit == card2.Suit) ? 10 : 0;

        // Add randomness to create variability
        int randomFactor = Random.Range(-10, 10);

        // Final hand strength calculation
        int handStrength = baseStrength + pairBonus + suitBonus + randomFactor;/*communityBonus + positionBonus;*/

        Debug.Log($"Bot {botNum} hand: {card1.Rank} of {card1.Suit}, {card2.Rank} of {card2.Suit}. Strength: {handStrength}");
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
        Debug.Log($"Starting betting round for phase: {currentPhase}");
        playerContributedChips = 0;
        UpdateCallAmount();
        currentPlayerIndex = (dealerIndex + 1) % numPlayers; // Start with the player after the dealer
        bettingRoundInProgress = true;
        StartNextTurn();
        //AdvanceTurn(); // Automatically process turns
    }

    //
    void StartNextTurn(int playerPos=0)
    {
        Debug.Log($"CurrentPlayerIndex: {currentPlayerIndex}");

        ResetHighlights();

        // Check if the round is complete
        if (IsRoundComplete())
        {
            Debug.Log("Round complete. Advancing game flow.");
            AdvanceGameFlow();
            return;
        }
        

        // Ensure currentPlayerIndex wraps correctly within bounds
        int loopCounter = currentPlayerIndex; // Safety counter to avoid infinite loops
        while (loopCounter < botActive.Length)  //cycles through all active players
        {
            //Debug.Log($"loop count in StartNextTurn: {loopCounter}");
            // If it's the player's turn and the player hasn't folded
            if (currentPlayerIndex == playerIndex && playerChips > 0 && playerCard1.enabled && playerCard2.enabled)
            {
                turnIndicatorText.text = "Player's Turn";
                playerHighlight.SetActive(true); // Highlight the player
                Debug.Log("Waiting for player input...");
                if(!playerCard1.enabled && !playerCard2.enabled){break;}


                return; // STOP here and WAIT for player input
            }
            // If it's a bot's turn and the bot is active
            else if (currentPlayerIndex != playerIndex && currentPlayerIndex <= botActive.Length && botActive[currentPlayerIndex/* - 1*/])
            {
                int botNum = currentPlayerIndex;
                if(botNum<playerIndex){botNum++;}
                turnIndicatorText.text = $"Bot {botNum}'s Turn";
                Debug.Log($"Bot {botNum}'s turn to act.");

                // Highlight the active bot
                switch (botNum)
                {
                    case 1:
                    {
                        botHighlight1.SetActive(true);
                        break;
                    }
                    case 2:
                    {
                        botHighlight2.SetActive(true);
                        break;
                    }
                    case 3:
                    {
                        botHighlight3.SetActive(true);
                        break;
                    }
                    case 4:
                    {
                        botHighlight4.SetActive(true);
                        break;
                    }
                    default:
                    {
                        Debug.LogWarning("Error in [StartNextTurn()]: switch default case executed");
                        break;
                    }                        
                }
                BotAction(currentPlayerIndex); // Perform bot action
            }

            ResetHighlights();
            // Move to the next player, wrapping around correctly
            currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
            loopCounter++;
        }
        //Debug.Log($"End of Turn loop, CurrentPlayerIndex: {currentPlayerIndex}");
        turnsTakenInPhase=0;
        AdvanceGameFlow();
        return;
        // If no valid players or bots, log an error (shouldn't happen in a properly designed game)
        //Debug.LogError("No valid players or bots found to take a turn!");

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
        int activeParticipants = 0;
        // Check active bots + player
        foreach (bool isActive in botActive)
        {
            if (isActive) activeParticipants++;
        }

        // If only one participant remains, return true
        return activeParticipants <= 1;
    }
//Edit this to rearrange the player position
    void HandleRoundWin()
    {
        List<int> botNums = new List<int>{};
        for(int i=0; i<GetActivePlayerCount(); i++)
        {
            if(botActive[i] && i!=playerIndex)
            {
                if(i<playerIndex)
                {
                    botNums.Add(i+1);
                }
                else{botNums.Add(i);}

            }
        }
        int winnerIndex = -1;

        // Determine the winner
        if (!playerFolded /*&& playerChips > 0*/) // Player is active and hasn't folded
        {
            winnerIndex = playerIndex;
            Debug.Log("Player wins the pot!");
            playerChips += potSize; // Award pot to the player
        }
        else // Check for the last active bot
        {
            for (int i = 0; i < GetActivePlayerCount(); i++)
            {
                if (botActive[i])
                {
                    
                    winnerIndex = i; // Bot IDs are 1-based
                    Debug.Log($"Bot {winnerIndex} wins the pot!");
                    botChips[i-1] += potSize; // Award pot to the bot
                    break;
                }
            }
        }

        // Reset pot size
        potSize = 0;

        // Update UI
        UpdateUI();
        //UpdateBlinds();

        // Reset the game for the next round
        ResetGame();
    }

    public void PlayerFolded()
    {
        Debug.Log("Player folded.");
        botActive[playerIndex] = false; // Mark player as inactive
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
            UpdateCallAmount();
        }
        else
        {
            Debug.LogWarning("Player doesn't have enough chips to call. Going all in");
            potSize += playerChips;
            playerChips = 0;
        }
        AdvanceTurn(); // Advance after player action
    }

    public void PlayerRaise(int raiseAmount)
    {
        int minRaise = currentBet + minimumBet;
        if (raiseAmount > minRaise && playerChips >= minRaise)
        {
            currentBet = raiseAmount;
            playerChips -= raiseAmount;
            potSize += raiseAmount;
            Debug.Log($"Player raises to {currentBet}.");
            UpdateUI();
            UpdateCallAmount();
        }
        else
        {
            Debug.LogWarning("Invalid raise amount.");
        }
        AdvanceTurn(); // Advance after player action
    }


        public bool ExitGame()
        {
            ResetGame(true);    //ResetGame(exitGame = false){}


            return true;
        }






    void AdvanceTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers; // Move to the next player
        turnsTakenInPhase++;
        StartNextTurn(currentPlayerIndex);
    }

    //may need to add turn indicator variable
    //player should be able to complete a round without losing all chips
    //round only advances if all bots fold(are not active)
    bool IsRoundComplete()
    {
        return (turnsTakenInPhase>=botActive.Length || (currentPlayerIndex == dealerIndex && currentRound > 1));
    /*
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


        return playerActed && allBotsActed;*/
    }

    bool IsPhaseComplete()
    {
        // One turn per bot and player per phase
       int activeParticipants = botActive.Length /*+ (!playerFolded ? 1 : 0)*/;
       return turnsTakenInPhase >= activeParticipants;
    }

 

public void AdvanceGameFlow()
{
    Debug.Log($"Advancing game flow. Current phase: {currentPhase}");

    // Reset turn counter for the new phase
    turnsTakenInPhase = 0;
    // Reset call amount for new betting round
    currentBet = 0;
    UpdateCallAmount();

    switch (currentPhase)
    {
        case GamePhase.PreFlop:
            RevealFlop();
            currentPhase = GamePhase.Flop;
            StartBettingRound();
            break;
        case GamePhase.Flop:
            RevealTurn();
            currentPhase = GamePhase.Turn;
            StartBettingRound();
            break;
        case GamePhase.Turn:
            RevealRiver();
            currentPhase = GamePhase.River;
            StartBettingRound();
            break;
        case GamePhase.River:
            currentPhase = GamePhase.Showdown;
            PerformShowdown();
            break;
    }
    // Ensure callAmountText is visible and updated
    callAmountText.gameObject.SetActive(true);
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
        //AudioSource.PlayClipAtPoint(soundFX, Camera.main.transform.position);
        Debug.Log("Revealing Flop.");
        for (int i = 0; i < 3; i++)
        {
            communityCards[i].sprite = GetCardSprite(deck[i]);
        }
        deck.RemoveRange(0, 3); // Remove revealed cards from the deck

    }
    void RevealTurn()
    {
        Debug.Log("Revealing Turn.");
        communityCards[3].sprite = GetCardSprite(deck[0]);
        deck.RemoveAt(0); // Remove the turn card from the deck

    }
    void RevealRiver()
    {
        Debug.Log("Revealing River.");
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

//Edit this to evaluate the hands
    int EvaluateHand(int playerIndex)
    {
        Debug.Log($"Evaluating hand for player/bot {playerIndex}");
        return Random.Range(1, 10); // Placeholder: random ranking
    }


    void ResetGame(bool exitGame = false)
    {
        Debug.Log("Resetting game for the next round.");

        //currentRound = 0;
        // Reset round-related data
        currentPhase = GamePhase.PreFlop;
        turnsTakenInPhase = 0;
        playerFolded = false;
        botActive = new bool[] { true, true, true, true, true }; // Reset bots as active
        potSize = 0;
        currentBet = 0;
        playerContributedChips = 0;
        callAmountText.gameObject.SetActive(true); // Ensure it is visible
        dealerIndex = (dealerIndex + 1) % numPlayers; // Move dealer position to the next player
        
        ResetHighlights();
        deck.Clear();

            // Reset community cards
        foreach (var card in communityCards)
        {
            card.sprite = cardBack;
        }
        communityCardData.Clear();

        if(!exitGame)
        {
            InitializeGame();
        }
    }



    void PerformShowdown()
    {
        Debug.Log("Showdown! Comparing hands to determine the winner.");
        
    // Reveal each bot's cards if they are still active
        for (int i = 0; i < botActive.Length; i++)
        {
            if (i != playerIndex && botActive[i])
            {
                RevealBotCards(i); // Bot IDs are 1-based
            }
        }

    // Assume player and bots are in a list for comparison
        List<int> activePlayers = new List<int> {}; //player in botActive
        for (int i = 0; i < numPlayers; i++)
        {
            if (botActive[i])
            activePlayers.Add(i); // Adding active bots to the list
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

        if (bestPlayer == playerIndex)
        {
            Debug.Log("The player wins the pot!");
            playerChips += potSize; // Award pot to player
        }
        else
        {
            int adjBestPlayer = bestPlayer;
            if(bestPlayer<playerIndex){adjBestPlayer++;}
            Debug.Log("Bot " + adjBestPlayer + " wins the pot!");
            botChips[adjBestPlayer-1] += potSize; // Award pot to the winning bot
        }

        potSize = 0; // Reset pot for the next round
        roundOver=true;
        UpdateUI();
        UpdateBlinds();
        ResetGame();
    }

    public enum DumpType{player, bot, active, allPlayers, game, bet, chips, round, blind, cards, role, all}
    public void DumpData(DumpType type)
    {
        if(type == DumpType.all || type == DumpType.player || type == DumpType.allPlayers )
        Debug.LogWarning($"Player Data Dump::/n PlayerChips: {playerChips}\nplayerActive: {botActive[playerIndex]}");
        if(type == DumpType.all || type == DumpType.bot || type == DumpType.allPlayers)
        Debug.LogWarning($"Bot Data Dump:\nBotChips: {botChips[0]}, {botChips[1]}, {botChips[2]}, {botChips[3]}\nbotActive: {botActive[1]}, {botActive[2]}, {botActive[3]}, {botActive[4]}  ");
        if(type == DumpType.all ||type == DumpType.bet || type == DumpType.game)
        Debug.LogWarning($"Bet Data Dump:\npotSize: {potSize}\nminimumBet: {minimumBet}\ncurrentBet: {currentBet}");
        if(type == DumpType.all ||type == DumpType.blind || type == DumpType.game)
        Debug.LogWarning($"Blind Data Dump:\nsmBlind: {smallBlind}\nbigBlind{bigBlind}\nsBIndex: {smallBlindIndex}\nbBIndex: {bigBlindIndex}\nsBPlayer: {smallBlindPlayer}/nbBPlayer{bigBlindPlayer}");
        if(type == DumpType.all ||type == DumpType.round || type == DumpType.game)
        Debug.LogWarning($"Round Data Dump:\nroundOver?: {roundOver}\ncurrentRound: {currentRound}\nturnsTaken: {turnsTakenInPhase}\nbettingRoundIP: {bettingRoundInProgress}");
        if(type == DumpType.all ||type == DumpType.role || type == DumpType.allPlayers)
        Debug.LogWarning($"Role Data Dump:\ndealerIndex: {dealerIndex}\ncurrentPlayerIndex: {currentPlayerIndex}\nsbIndex: {smallBlindIndex}\nbBIndex: {bigBlindIndex}");
    }
}

