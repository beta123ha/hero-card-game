using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpponentDeckPreviewController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text timerText;
    public TMP_Text messageText;

    [Header("Enemy Deck UI")]
    public Transform enemyHeroListParent;
    public Transform enemyTacticListParent;
    public DeckPreviewCardUI previewCardPrefab;

    [Header("Timer")]
    public float previewTime = 60f;

    private float remainingTime;
    private bool isFinished;

    private void Start()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession is missing");
            return;
        }

        remainingTime = previewTime;
        isFinished = false;

        SetupTexts();
        CreateEnemyHeroCards();
        CreateEnemyTacticCards();
        UpdateTimerUI();
    }

    private void Update()
    {
        if (isFinished)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            GoToTerrainSetup();
            return;
        }

        UpdateTimerUI();
    }

    private void SetupTexts()
    {
        if (titleText != null)
        {
            titleText.text = "Opponent Deck Preview";
        }

        if (messageText != null)
        {
            messageText.text = "Study the opponent deck before arranging terrain.";
        }
    }

    private void CreateEnemyHeroCards()
    {
        ClearChildren(enemyHeroListParent);

        foreach (HeroCardData hero in GameSession.Instance.enemySelectedHeroes)
        {
            DeckPreviewCardUI card = Instantiate(previewCardPrefab, enemyHeroListParent);
            card.SetupHero(hero);
        }
    }

    private void CreateEnemyTacticCards()
    {
        ClearChildren(enemyTacticListParent);

        foreach (TacticCardData tactic in GameSession.Instance.enemySelectedTactics)
        {
            DeckPreviewCardUI card = Instantiate(previewCardPrefab, enemyTacticListParent);
            card.SetupTactic(tactic);
        }
    }

    public void GoToTerrainSetup()
    {
        if (isFinished)
        {
            return;
        }

        isFinished = true;
        SceneManager.LoadScene("terrain_setup");
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
        {
            return;
        }

        timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);
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
}