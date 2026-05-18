using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainSetupController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text messageText;

    [Header("Player Terrain UI")]
    public Transform playerTerrainListParent;
    public TerrainSlotButtonUI playerTerrainSlotPrefab;

    [Header("Enemy Terrain UI")]
    public Transform enemyTerrainListParent;
    public TerrainSlotButtonUI enemyTerrainSlotPrefab;

    [Header("AI Profiles")]
    public AIPlayStyleProfile aggressiveProfile;
    public AIPlayStyleProfile defensiveProfile;
    public AIPlayStyleProfile balancedProfile;

    [Header("Timer")]
    public float setupTime = 15f;

    [Header("Enemy AI Terrain")]
    public float enemySwapInterval = 1.5f;

    private float remainingTime;
    private float enemyThinkTimer;

    private bool isLocked;

    private int firstSelectedIndex = -1;

    private List<TerrainData> playerTerrainOrder = new List<TerrainData>();
    private List<TerrainData> enemyTerrainOrder = new List<TerrainData>();

    private List<TerrainSlotButtonUI> playerTerrainButtons = new List<TerrainSlotButtonUI>();
    private List<TerrainSlotButtonUI> enemyTerrainButtons = new List<TerrainSlotButtonUI>();

    private void Start()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession is missing");
            return;
        }

        remainingTime = setupTime;
        enemyThinkTimer = enemySwapInterval;
        isLocked = false;

        SetupPlayerTerrains();
        SetupEnemyTerrains();

        RefreshAllUI("Arrange your terrain. Watch the enemy terrain and counter it.");
    }

    private void Update()
    {
        if (isLocked)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        enemyThinkTimer -= Time.deltaTime;

        if (enemyThinkTimer <= 0f)
        {
            enemyThinkTimer = enemySwapInterval;
            LetEnemyThinkAndSwap();
        }

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            LockAndGoNext();
            return;
        }

        UpdateTimerUI();
    }

    private void SetupPlayerTerrains()
    {
        CountryData playerCountry = GameSession.Instance.playerCountry;

        if (playerCountry == null)
        {
            Debug.LogError("player country is missing");
            return;
        }

        if (playerCountry.battlefieldTerrains == null || playerCountry.battlefieldTerrains.Count != 7)
        {
            Debug.LogError("player battlefield terrains must have exactly 7 terrains");
            return;
        }

        playerTerrainOrder.Clear();
        playerTerrainOrder.AddRange(playerCountry.battlefieldTerrains);

        CreatePlayerTerrainButtons();
    }

    private void SetupEnemyTerrains()
    {
        CountryData enemyCountry = GameSession.Instance.enemyCountry;

        if (enemyCountry == null)
        {
            Debug.LogError("enemy country is missing");
            return;
        }

        if (enemyCountry.battlefieldTerrains == null || enemyCountry.battlefieldTerrains.Count != 7)
        {
            Debug.LogError("enemy battlefield terrains must have exactly 7 terrains");
            return;
        }

        enemyTerrainOrder = AITerrainPlanner.CreateInitialOrder(enemyCountry);

        if (enemyTerrainOrder == null || enemyTerrainOrder.Count != 7)
        {
            enemyTerrainOrder = new List<TerrainData>();
            enemyTerrainOrder.AddRange(enemyCountry.battlefieldTerrains);
        }

        CreateEnemyTerrainButtons();
    }

    private void CreatePlayerTerrainButtons()
    {
        ClearChildren(playerTerrainListParent);
        playerTerrainButtons.Clear();

        for (int i = 0; i < playerTerrainOrder.Count; i++)
        {
            TerrainSlotButtonUI button = Instantiate(playerTerrainSlotPrefab, playerTerrainListParent);
            button.Setup(i, playerTerrainOrder[i], this, true);
            playerTerrainButtons.Add(button);
        }
    }

    private void CreateEnemyTerrainButtons()
    {
        ClearChildren(enemyTerrainListParent);
        enemyTerrainButtons.Clear();

        for (int i = 0; i < enemyTerrainOrder.Count; i++)
        {
            TerrainSlotButtonUI button = Instantiate(enemyTerrainSlotPrefab, enemyTerrainListParent);
            button.Setup(i, enemyTerrainOrder[i], this, false);
            enemyTerrainButtons.Add(button);
        }
    }

    public void SelectSlot(int index)
    {
        if (isLocked)
        {
            return;
        }

        if (index < 0 || index >= playerTerrainOrder.Count)
        {
            return;
        }

        if (firstSelectedIndex == -1)
        {
            firstSelectedIndex = index;
            RefreshAllUI("First slot selected. Select another slot to swap.");
            return;
        }

        if (firstSelectedIndex == index)
        {
            firstSelectedIndex = -1;
            RefreshAllUI("Selection cancelled.");
            return;
        }

        SwapPlayerTerrains(firstSelectedIndex, index);

        firstSelectedIndex = -1;
        RefreshAllUI("You swapped terrain. Enemy may react soon.");
    }

    private void SwapPlayerTerrains(int firstIndex, int secondIndex)
    {
        TerrainData temp = playerTerrainOrder[firstIndex];
        playerTerrainOrder[firstIndex] = playerTerrainOrder[secondIndex];
        playerTerrainOrder[secondIndex] = temp;
    }

    private void LetEnemyThinkAndSwap()
    {
        AIPlayStyleProfile profile = GetProfileForStyle(GameSession.Instance.enemyCurrentPlayStyle);

        AITerrainSwapDecision decision = AITerrainPlanner.DecideNextSwap(
            enemyTerrainOrder,
            playerTerrainOrder,
            GameSession.Instance.enemySelectedHeroes,
            GameSession.Instance.selectedHeroes,
            profile,
            GameSession.Instance.enemyDifficulty
        );

        if (decision == null || !decision.shouldSwap)
        {
            return;
        }

        SwapEnemyTerrains(decision.firstIndex, decision.secondIndex);

        Debug.Log(
            "Enemy terrain swap: " +
            decision.firstIndex +
            " <-> " +
            decision.secondIndex +
            ". Reason: " +
            decision.reason
        );

        RefreshAllUI("Enemy changed terrain. Counter it before time runs out.");
    }

    private void SwapEnemyTerrains(int firstIndex, int secondIndex)
    {
        if (firstIndex < 0 || firstIndex >= enemyTerrainOrder.Count)
        {
            return;
        }

        if (secondIndex < 0 || secondIndex >= enemyTerrainOrder.Count)
        {
            return;
        }

        TerrainData temp = enemyTerrainOrder[firstIndex];
        enemyTerrainOrder[firstIndex] = enemyTerrainOrder[secondIndex];
        enemyTerrainOrder[secondIndex] = temp;
    }

    public void LockAndGoNext()
    {
        if (isLocked)
        {
            return;
        }

        isLocked = true;

        SaveTerrainOrders();

        Debug.Log("Player terrain order saved: " + playerTerrainOrder.Count);
        Debug.Log("Enemy terrain order saved: " + enemyTerrainOrder.Count);

        SceneManager.LoadScene("battle");
    }

    private void SaveTerrainOrders()
    {
        GameSession.Instance.playerTerrainOrder.Clear();
        GameSession.Instance.playerTerrainOrder.AddRange(playerTerrainOrder);

        GameSession.Instance.enemyTerrainOrder.Clear();
        GameSession.Instance.enemyTerrainOrder.AddRange(enemyTerrainOrder);
    }

    private AIPlayStyleProfile GetProfileForStyle(AIPlayStyle style)
    {
        if (style == AIPlayStyle.Aggressive)
        {
            return aggressiveProfile;
        }

        if (style == AIPlayStyle.Defensive)
        {
            return defensiveProfile;
        }

        return balancedProfile;
    }

    private void RefreshAllUI(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }

        UpdateTimerUI();
        RefreshPlayerTerrainButtons();
        RefreshEnemyTerrainButtons();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
        {
            return;
        }

        timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);
    }

    private void RefreshPlayerTerrainButtons()
    {
        for (int i = 0; i < playerTerrainButtons.Count; i++)
        {
            bool isSelected = i == firstSelectedIndex;
            playerTerrainButtons[i].Refresh(i, playerTerrainOrder[i], isSelected, true);
        }
    }

    private void RefreshEnemyTerrainButtons()
    {
        for (int i = 0; i < enemyTerrainButtons.Count; i++)
        {
            enemyTerrainButtons[i].Refresh(i, enemyTerrainOrder[i], false, false);
        }
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public void BackToOpponentDeckPreview()
    {
        SceneManager.LoadScene("opponent_deck_preview");
    }
}