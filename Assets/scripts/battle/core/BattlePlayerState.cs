using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerState
{
    public string playerName;
    public bool isAI;

    public int currentHealth = 100;

    public List<BattleCardInstance> deck = new List<BattleCardInstance>();
    public List<BattleCardInstance> hand = new List<BattleCardInstance>();
    public List<BattleHeroInstance> graveyard = new List<BattleHeroInstance>();

    public List<BattleBoardSlot> boardSlots = new List<BattleBoardSlot>();
    public List<BattleTacticSlot> tacticSlots = new List<BattleTacticSlot>();

    public BattlePlayerState(
        string playerName,
        bool isAI,
        List<HeroCardData> selectedHeroes,
        List<TacticCardData> selectedTactics,
        List<TerrainData> terrainOrder
    )
    {
        this.playerName = playerName;
        this.isAI = isAI;

        currentHealth = 100;

        BuildDeck(selectedHeroes, selectedTactics);
        ShuffleDeck();
        BuildBoardSlots(terrainOrder);
        BuildTacticSlots();
    }

    private void BuildDeck(
        List<HeroCardData> selectedHeroes,
        List<TacticCardData> selectedTactics
    )
    {
        deck.Clear();

        if (selectedHeroes != null)
        {
            foreach (HeroCardData hero in selectedHeroes)
            {
                if (hero != null)
                {
                    deck.Add(new BattleCardInstance(hero));
                }
            }
        }

        if (selectedTactics != null)
        {
            foreach (TacticCardData tactic in selectedTactics)
            {
                if (tactic != null)
                {
                    deck.Add(new BattleCardInstance(tactic));
                }
            }
        }
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);

            BattleCardInstance temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    private void BuildBoardSlots(List<TerrainData> terrainOrder)
    {
        boardSlots.Clear();

        for (int i = 0; i < 7; i++)
        {
            TerrainData terrain = null;

            if (terrainOrder != null && i < terrainOrder.Count)
            {
                terrain = terrainOrder[i];
            }

            boardSlots.Add(new BattleBoardSlot(i, terrain));
        }
    }

    private void BuildTacticSlots()
    {
        tacticSlots.Clear();

        for (int i = 0; i < 3; i++)
        {
            tacticSlots.Add(new BattleTacticSlot(i));
        }
    }

    public void DrawCard()
    {
        if (deck.Count <= 0)
        {
            return;
        }

        BattleCardInstance topCard = deck[0];
        deck.RemoveAt(0);
        hand.Add(topCard);
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
        }
    }
}