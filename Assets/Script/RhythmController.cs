using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class NoteInfo
{
    public float timestamp;
}

// DIHAPUS: Kelas ComboStage tidak kita perlukan lagi untuk sistem yang lebih sederhana.

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
    [SerializeField] private float maxSpawnDistance = 4f;
    
    [Header("Referensi UI")]
    public Image harmonyImage; 
    public Image comboImage;
    public TextMeshProUGUI scoreText;
    
    // DIUBAH: Kita hanya butuh satu sprite untuk status kombo aktif.
    [Header("Pengaturan Gambar Kombo")]
    public Sprite comboActiveSprite; 

    [Header("Referensi Akhir Level")]
    public string nextSceneName;
    public GameObject gameOverPanel;

    [Header("Referensi Latar Belakang")]
    public SpriteRenderer currentBackground;
    public SpriteRenderer nextBackground;
    public Sprite[] backgroundStages;

    [Header("Pengaturan Efek Visual")]
    [SerializeField] private float perfectShakeDuration = 0.1f;
    [SerializeField] private float perfectShakeMagnitude = 0.05f;

    private int currentScore = 0;
    private int nextNoteIndex = 0;
    private float songTime;
    private int perfectStreak = 0;
    private bool isComboActive = false;
    private bool levelEnded = false;
    private NoteObject lastNoteObject;
    private int currentBackgroundStage = 0;
    private bool isFirstNote = true;
    
    void Start()
    {
        AudioManager.Instance.PlayBGM(songClip.name);
        levelEnded = false;
        isFirstNote = true;
        lastNoteObject = null;
        
        harmonyImage.fillAmount = 0;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (comboImage != null)
        {
            comboImage.enabled = false;
            comboImage.sprite = comboActiveSprite; // Langsung atur spritenya di awal
        }
        if (scoreText != null) scoreText.text = "Poin : 0/" + targetScore;
        
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

        songTime = AudioManager.Instance.BGMSource.time;

        if (songTime >= levelDuration) { EndLevel(); return; }
        
        if (nextNoteIndex < notes.Count && songTime >= notes[nextNoteIndex].timestamp)
        {
            bool isChainNote = IsChainNote(nextNoteIndex);
            Vector2 spawnPosition = GenerateNextNotePosition();
            
            GameObject noteGO = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
            NoteObject newNoteObject = noteGO.GetComponent<NoteObject>();
            newNoteObject.Initialize(this);
            
            // PERUBAHAN UTAMA: Logika memberi "tongkat estafet"
            if (isChainNote && lastNoteObject != null)
            {
                // Nada baru (depan) diberi tugas untuk mengaktifkan nada lama (belakang)
                newNoteObject.SetNoteToActivate(lastNoteObject);
                // Nada lama (belakang) disuruh tidur (menjadi gelap & non-aktif)
                lastNoteObject.BecomeBackgroundNote();
            }
            
            lastNoteObject = newNoteObject;
            isFirstNote = false;
            nextNoteIndex++;
        }
    }
    void SpawnNote()
    {
        Vector2 spawnPosition = GenerateNextNotePosition();
        
        GameObject noteGO = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        NoteObject newNoteObject = noteGO.GetComponent<NoteObject>();
        newNoteObject.Initialize(this);
        
        if (lastNoteObject != null && IsChainNote(nextNoteIndex))
        {
            lastNoteObject.BecomeBackgroundNote();
        }
        
        lastNoteObject = newNoteObject;
        isFirstNote = false;
    }

    bool IsChainNote(int noteIndex)
    {
        if (noteIndex > 0 && noteIndex < notes.Count)
        {
            if (notes[noteIndex].timestamp - notes[noteIndex - 1].timestamp <= 0.35f)
            {
                return true;
            }
        }
        return false;
    }

    Vector2 GenerateNextNotePosition()
    {
        if (isFirstNote || lastNoteObject == null)
        {
            return new Vector2(
                Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y)
            );
        }
        
        if (IsChainNote(nextNoteIndex))
        {
            return lastNoteObject.transform.position;
        }

        Vector2 randomPosition;
        int attempts = 0;
        do
        {
            randomPosition = (Vector2)lastNoteObject.transform.position + Random.insideUnitCircle * maxSpawnDistance;
            attempts++;
            if (attempts > 20) { return spawnArea.bounds.center; }
        }
        while (!spawnArea.bounds.Contains(randomPosition));
        return randomPosition;
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
                gameOverPanel.SetActive(true);
        }
    }

    public void NoteHit(Accuracy accuracy)
    {
        if (accuracy == Accuracy.Miss)
        {
            NoteMissed();
            return;
        }
        
        if (accuracy == Accuracy.Perfect)
        {
            perfectStreak++;
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(perfectShakeDuration, perfectShakeMagnitude);
            }
            if (!isComboActive && perfectStreak >= 5)
            {
                isComboActive = true;
                UpdateComboUI();
            }
        }
        else
        {
            perfectStreak = 0;
        }
        
        int points = (accuracy == Accuracy.Perfect) ? perfectScore : goodScore;
        
        if (isComboActive) { currentScore += points * 2; }
        else { currentScore += points; }

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
        UpdateUI();
    }

    void UpdateUI()
    {
        currentScore = Mathf.Clamp(currentScore, 0, targetScore + perfectScore * 10);
        
        int displayScore = Mathf.Min(currentScore, targetScore);
        float fillPercentage = (float)displayScore / targetScore;
        harmonyImage.fillAmount = Mathf.Clamp01(fillPercentage);

        if (scoreText != null)
        {
            scoreText.text = "Poin : " + currentScore + "/" + targetScore;
        }

        int requiredStage = Mathf.FloorToInt(fillPercentage / 0.2f);
        if (requiredStage != currentBackgroundStage && requiredStage >= 0 && requiredStage < backgroundStages.Length)
        {
            currentBackgroundStage = requiredStage;
            StartCoroutine(ChangeBackground(currentBackgroundStage));
        }
    }

    // DIUBAH: Fungsi menjadi lebih sederhana
    void UpdateComboUI()
    {
        if (comboImage == null) return;
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