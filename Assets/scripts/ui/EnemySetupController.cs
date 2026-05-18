using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySetupController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text selectedCountryText;
    public TMP_Text selectedDifficultyText;

    [Header("Data")]
    public CountryData selectedCountry;

    private AIDifficulty selectedDifficulty = AIDifficulty.Normal;

    private void Start()
    {
        UpdateUI();
    }

    public void SelectEnemyCountry(CountryData country)
    {
        selectedCountry = country;
        UpdateUI();
    }

    public void SetEasy()
    {
        selectedDifficulty = AIDifficulty.Easy;
        UpdateUI();
    }

    public void SetNormal()
    {
        selectedDifficulty = AIDifficulty.Normal;
        UpdateUI();
    }

    public void SetHard()
    {
        selectedDifficulty = AIDifficulty.Hard;
        UpdateUI();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("menu");
    }

    public void ConfirmAndGoNext()
    {
        if (selectedCountry == null)
        {
            Debug.LogWarning("enemy country has not been selected");
            return;
        }

        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession is missing");
            return;
        }

        AIPlayStyle randomPlayStyle = AIStyleSelector.PickRandomStyle();

        GameSession.Instance.enemyCountry = selectedCountry;
        GameSession.Instance.enemyDifficulty = selectedDifficulty;
        GameSession.Instance.enemyBasePlayStyle = randomPlayStyle;
        GameSession.Instance.enemyCurrentPlayStyle = randomPlayStyle;

        Debug.Log(
            "Enemy AI setup: " +
            "Country = " + selectedCountry.countryName +
            ", Difficulty = " + selectedDifficulty +
            ", Base Style = " + randomPlayStyle
        );

        SceneManager.LoadScene("player_setup");
    }

    private void UpdateUI()
    {
        if (selectedCountry == null)
        {
            selectedCountryText.text = "Enemy Country: Not Selected";
        }
        else
        {
            selectedCountryText.text = "Enemy Country: " + selectedCountry.countryName;
        }

        selectedDifficultyText.text = "Difficulty: " + selectedDifficulty;
    }
}