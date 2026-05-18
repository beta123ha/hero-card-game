using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [Header("Enemy Setup")]
    public CountryData enemyCountry;
    public AIDifficulty enemyDifficulty = AIDifficulty.Normal;

    [Header("Enemy AI")]
    public AIPlayStyle enemyBasePlayStyle = AIPlayStyle.Balanced;
    public AIPlayStyle enemyCurrentPlayStyle = AIPlayStyle.Balanced;

    [Header("Player Setup")]
    public CountryData playerCountry;

    [Header("Player Deck Selection")]
    public List<HeroCardData> selectedHeroes = new List<HeroCardData>();
    public List<TacticCardData> selectedTactics = new List<TacticCardData>();

    [Header("Enemy Deck Selection")]
    public List<HeroCardData> enemySelectedHeroes = new List<HeroCardData>();
    public List<TacticCardData> enemySelectedTactics = new List<TacticCardData>();

    [Header("Terrain Setup")]
    public List<TerrainData> playerTerrainOrder = new List<TerrainData>();
    public List<TerrainData> enemyTerrainOrder = new List<TerrainData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetSession()
    {
        enemyCountry = null;
        enemyDifficulty = AIDifficulty.Normal;

        enemyBasePlayStyle = AIPlayStyle.Balanced;
        enemyCurrentPlayStyle = AIPlayStyle.Balanced;

        playerCountry = null;

        selectedHeroes.Clear();
        selectedTactics.Clear();

        enemySelectedHeroes.Clear();
        enemySelectedTactics.Clear();

        playerTerrainOrder.Clear();
        enemyTerrainOrder.Clear();
    }
}