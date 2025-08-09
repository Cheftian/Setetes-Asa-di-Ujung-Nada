using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NoteInfo
{
    public float timestamp;
}

public class RhythmController : MonoBehaviour
{
    [Header("Data Level")]
    public AudioClip songClip;
    public List<NoteInfo> notes;
    public float levelDuration; 

    [Header("Pengaturan Skor")]
    public int targetScore = 10000;
    public int perfectScore = 100;
    public int goodScore = 50;
    public int missPenalty = 75;

    [Header("Pengaturan Gameplay")]
    public GameObject notePrefab;
    public BoxCollider2D spawnArea;
    
    [Header("Referensi UI")]
    public Image harmonyImage; 
    public Image comboImage;
    public Sprite comboActiveSprite;

    [Header("Referensi Akhir Level")]
    public string nextSceneName;
    public GameObject gameOverPanel;

    private int currentScore = 0;
    private int nextNoteIndex = 0;
    private float songTime;
    private int perfectStreak = 0;
    private bool isComboActive = false;
    private bool levelEnded = false;
    
    [Header("Referensi Latar Belakang")]
    public SpriteRenderer currentBackground;
    public SpriteRenderer nextBackground;
    public Sprite[] backgroundStages;
    private int currentBackgroundStage = 0;

    void Start()
    {
        // DIKEMBALIKAN: Musik dimulai TEPAT saat level mulai.
        AudioManager.Instance.PlayBGM(songClip.name);

        levelEnded = false;
        harmonyImage.fillAmount = 0;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (comboImage != null)
        {
            comboImage.enabled = false;
        }
        perfectStreak = 0;
        isComboActive = false;

        if (backgroundStages.Length > 0)
        {
            currentBackground.sprite = backgroundStages[0];
            Color tempColor = nextBackground.color;
            tempColor.a = 0f;
            nextBackground.color = tempColor;
        }
    }

    void Update()
    {
        if (levelEnded) return;

        // Waktu lagu akan terus berjalan sejak awal karena musik sudah diputar di Start()
        songTime = AudioManager.Instance.BGMSource.time;

        if (songTime >= levelDuration)
        {
            EndLevel();
            return;
        }

        if (nextNoteIndex < notes.Count && songTime >= notes[nextNoteIndex].timestamp)
        {
            SpawnNote();
            nextNoteIndex++;
        }
    }

    void EndLevel()
    {
        levelEnded = true;
        if (currentScore >= targetScore)
        {
            if(!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    void SpawnNote()
    {
        float randomX = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
        float randomY = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
        Vector2 spawnPosition = new Vector2(randomX, randomY);

        GameObject noteGO = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        noteGO.GetComponent<NoteObject>().Initialize(this);
    }

    public void NoteHit(Accuracy accuracy)
    {
        // DIHAPUS: Logika untuk memulai game dari sini dihapus.
        
        if (accuracy == Accuracy.Perfect)
        {
            perfectStreak++;
            if (!isComboActive && perfectStreak >= 5)
            {
                isComboActive = true;
                UpdateComboUI();
            }
        }
        else if (accuracy == Accuracy.Miss)
        {
            NoteMissed();
            return;
        }
        
        int points = (accuracy == Accuracy.Perfect) ? perfectScore : goodScore;
        
        if (isComboActive) { currentScore += points * 2; }
        else { currentScore += points; }

        currentScore = Mathf.Clamp(currentScore, 0, targetScore);
        UpdateUI();
    }

    public void NoteMissed()
    {
        perfectStreak = 0;
        if (isComboActive)
        {
            isComboActive = false;
            UpdateComboUI();
        }

        currentScore -= missPenalty;
        currentScore = Mathf.Clamp(currentScore, 0, targetScore);
        UpdateUI();
    }

    void UpdateUI()
    {
        float fillPercentage = (float)currentScore / targetScore;
        harmonyImage.fillAmount = Mathf.Clamp01(fillPercentage);

        int requiredStage = Mathf.FloorToInt(fillPercentage / 0.2f);
        if (requiredStage != currentBackgroundStage && requiredStage < backgroundStages.Length)
        {
            currentBackgroundStage = requiredStage;
            StartCoroutine(ChangeBackground(currentBackgroundStage));
        }
    }

    void UpdateComboUI()
    {
        if (comboImage == null) return;
        comboImage.sprite = comboActiveSprite;
        comboImage.enabled = isComboActive;
    }
    
    private IEnumerator ChangeBackground(int stageIndex)
    {
        nextBackground.sprite = backgroundStages[stageIndex];
        
        float timer = 0f;
        float duration = 1.0f;
        while(timer < duration)
        {
            float alpha = Mathf.Lerp(0, 1, timer / duration);
            Color tempColor = nextBackground.color;
            tempColor.a = alpha;
            nextBackground.color = tempColor;
            timer += Time.deltaTime;
            yield return null;
        }
        
        currentBackground.sprite = nextBackground.sprite;
        
        Color resetColor = nextBackground.color;
        resetColor.a = 0;
        nextBackground.color = resetColor;
    }
}