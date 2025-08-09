using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Prologue");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}