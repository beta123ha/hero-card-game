using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleUIController : MonoBehaviour
{
    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;
    public TMP_Text turnText;

    private int playerHp = 100;
    private int enemyHp = 100;

    void Start()
    {
        updateUI();
    }

    void updateUI()
    {
        playerHpText.text = "Player HP: " + playerHp;
        enemyHpText.text = "Enemy HP: " + enemyHp;
        turnText.text = "Turn: Player";
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }
}