using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void playGame(){
        SceneManager.LoadScene("enemy_setup");
    }

    public void quitGame(){
        Application.Quit();
    }
}
