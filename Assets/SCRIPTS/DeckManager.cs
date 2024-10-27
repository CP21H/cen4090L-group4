using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
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
        botOneCard1.sprite = GetCardSprite(deck[2]);
        botOneCard2.sprite = GetCardSprite(deck[3]);

        botTwoCard1.sprite = GetCardSprite(deck[4]);
        botTwoCard2.sprite = GetCardSprite(deck[5]);

        botThreeCard1.sprite = GetCardSprite(deck[6]);
        botThreeCard2.sprite = GetCardSprite(deck[7]);

        botFourCard1.sprite = GetCardSprite(deck[8]);
        botFourCard2.sprite = GetCardSprite(deck[9]);

        // Remove dealt cards from the deck (2 cards for each player and bot = 10 cards)
        for (int i = 0; i < 10; i++)
        {
            deck.RemoveAt(0);
        }

        // Deal 5 community cards (Flop, Turn, River)
        for (int i = 0; i < 5; i++)
        {
            communityCards[i].sprite = GetCardSprite(deck[i]);
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
        switch (currentRound)
        {
        case 0: // Pre-Flop complete, move to Flop
            BotAction(1);  // Bot One takes action
            BotAction(2);  // Bot Two takes action
            BotAction(3);  // Bot Three takes action
            BotAction(4);  // Bot Four takes action
            RevealFlop();
            currentRound++;
            break;
        case 1: // Flop complete, move to Turn
            BotAction(1);
            BotAction(2);
            BotAction(3);
            BotAction(4);
            RevealTurn();
            currentRound++;
            break;
        case 2: // Turn complete, move to River
            BotAction(1);
            BotAction(2);
            BotAction(3);
            BotAction(4);
            RevealRiver();
            currentRound++;
            break;
        case 3: // River complete, move to Showdown
            BotAction(1);
            BotAction(2);
            BotAction(3);
            BotAction(4);
            PerformShowdown();
            break;
    }
     }

    void RevealFlop()
    {
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

    void PerformShowdown()
    {
        Debug.Log("Showdown! Compare hands to determine the winner.");
        // Add logic to evaluate hands and determine the winner
    }

    public void BotAction(int botNumber)
    {
        int randomAction = Random.Range(0, 3);  // 0 = Fold, 1 = Call, 2 = Raise

        switch (randomAction)
        {
            case 0:
                Debug.Log("Bot " + botNumber + " folds");
            // Logic for bot folding
                break;
            case 1:
                Debug.Log("Bot " + botNumber + " calls");
            // Logic for bot calling
                break;
            case 2:
                Debug.Log("Bot " + botNumber + " raises");
            // Logic for bot raising
                break;
    }

    // After the bot's action, advance the game flow
    AdvanceGameFlow();
    }   














}
    









