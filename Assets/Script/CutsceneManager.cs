using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CutscenePanel
{
    public Sprite panelImage;
    [TextArea(3, 10)]
    public string narrationText;
}

public class CutsceneManager : MonoBehaviour
{
    [Header("Data Cutscene")]
    [SerializeField] private List<CutscenePanel> panels;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Referensi Komponen UI")]
    [SerializeField] private Image panelImageUI;
    [SerializeField] private TextMeshProUGUI narrationTextUI;
    [SerializeField] private CanvasGroup cutsceneCanvasGroup;

    [Header("Pengaturan Khusus Epilog")]
    [SerializeField] private bool isEpilogue = false;
    [SerializeField] private GameObject endButton;

    private int currentPanelIndex = 0;
    private bool isTransitioning = false;

    private PauseManager pauseManager;

    void Start()
    {
        pauseManager = FindObjectOfType<PauseManager>();

        if (isEpilogue && endButton != null)
        {
            endButton.SetActive(false);
            CanvasGroup buttonCG = endButton.GetComponent<CanvasGroup>();
            if (buttonCG != null) buttonCG.alpha = 0;
        }

        if(cutsceneCanvasGroup != null) cutsceneCanvasGroup.alpha = 0;
        StartCoroutine(TransitionToPanel(0));
    }

    void Update()
    {
        bool isGamePaused = (pauseManager != null && pauseManager.IsGamePaused());

        if (Input.anyKeyDown && !isTransitioning && !isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) return;
            
            AdvanceToNextPanel();
        }
    }

    private void AdvanceToNextPanel()
    {
        currentPanelIndex++;

        if (currentPanelIndex >= panels.Count)
        {
            EndCutscene();
        }
        else
        {
            StartCoroutine(TransitionToPanel(currentPanelIndex));
        }
    }

    private IEnumerator TransitionToPanel(int index)
    {
        isTransitioning = true;
        yield return StartCoroutine(Fade(0f)); 
        if (index < panels.Count)
        {
            panelImageUI.sprite = panels[index].panelImage;
            narrationTextUI.text = panels[index].narrationText;
        }

        if (isEpilogue && index == panels.Count - 1)
        {
            StartCoroutine(FadeInButton());
        }

        yield return StartCoroutine(Fade(1f)); 
        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float timer = 0f;
        float startAlpha = cutsceneCanvasGroup.alpha;

        while (timer < fadeDuration)
        {
            cutsceneCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            timer += Time.unscaledDeltaTime; 
            yield return null;
        }

        cutsceneCanvasGroup.alpha = targetAlpha;
    }

    private void EndCutscene()
    {
        isTransitioning = true;
        
        if (!isEpilogue && !string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
    private IEnumerator FadeInButton()
    {
        if (endButton != null)
        {
            endButton.SetActive(true);
            CanvasGroup buttonCG = endButton.GetComponent<CanvasGroup>();
            if (buttonCG != null)
            {
                float timer = 0f;
                while (timer < fadeDuration)
                {
                    buttonCG.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                buttonCG.alpha = 1f;
            }
        }
    }
}