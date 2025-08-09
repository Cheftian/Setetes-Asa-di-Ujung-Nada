using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Komponen UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Pengaturan Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public bool IsGamePaused()
    {
        return isPaused;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        
        // BARU: Perintahkan AudioManager untuk menjeda musik BGM
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.BGMSource.Pause();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false); 
        
        // BARU: Perintahkan AudioManager untuk melanjutkan musik BGM
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.BGMSource.UnPause();
        }
    }

    public void RestartLevel()
    {
        // PENTING: Pastikan musik tidak dalam keadaan jeda saat pindah scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.BGMSource.UnPause();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        // PENTING: Pastikan musik tidak dalam keadaan jeda saat pindah scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.BGMSource.UnPause();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}