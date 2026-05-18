using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetupController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text selectedCountryText;

    [Header("Data")]
    public CountryData selectedCountry;

    private void Start()
    {
        UpdateUI();
    }

    public void SelectPlayerCountry(CountryData country)
    {
        selectedCountry = country;
        UpdateUI();
    }

    public void BackToEnemySetup()
    {
        SceneManager.LoadScene("enemy_setup");
    }

    public void ConfirmAndGoNext()
    {
        if (selectedCountry == null)
        {
            Debug.LogWarning("player country has not been selected");
            return;
        }

        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession is missing");
            return;
        }

        GameSession.Instance.playerCountry = selectedCountry;

        SceneManager.LoadScene("deck_setup");
    }

    private void UpdateUI()
    {
        if (selectedCountry == null)
        {
            selectedCountryText.text = "Your Country: Not Selected";
        }
        else
        {
            selectedCountryText.text = "Your Country: " + selectedCountry.countryName;
        }
    }
}