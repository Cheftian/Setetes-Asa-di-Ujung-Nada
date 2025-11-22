using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [Header("Pengaturan Waktu Fase")]
    [SerializeField] private float growDuration = 1.0f;
    [SerializeField] private float perfectWindowDuration = 0.3f;
    [SerializeField] private float overinflateDuration = 0.5f;

    [Header("Pengaturan Skala")]
    [SerializeField] private Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private Vector3 perfectScale = Vector3.one;
    [SerializeField] private Vector3 endScale = new Vector3(1.5f, 1.5f, 1.5f);

    [Header("Pengaturan Gerakan Mengambang")]
    [SerializeField] private float hoverSpeed = 1f;
    [SerializeField] private float hoverAmplitude = 0.2f;

    [Header("Pengaturan Tampilan")]
    [SerializeField] private Color brightColor = Color.white;
    [SerializeField] private Color darkColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    [SerializeField] private int frontOrderInLayer = 10;
    [SerializeField] private int backOrderInLayer = 5;

    private float timer = 0f;
    private RhythmController rhythmController;
    private SpriteRenderer spriteRenderer;
    private Collider2D noteCollider; // BARU: Simpan referensi collider

    private Vector3 initialPosition;
    private NoteObject noteToActivate = null;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        noteCollider = GetComponent<Collider2D>(); // BARU: Ambil komponen collider
    }

    public void Initialize(RhythmController controller)
    {
        rhythmController = controller;
        SetState(true);
    }

    void Start()
    {
        AudioManager.Instance.PlaySFX("Bubble-pop");
        initialPosition = transform.position;
        Color startColor = spriteRenderer.color;
        startColor.a = 0f;
        spriteRenderer.color = startColor;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < growDuration)
        {
            float progress = timer / growDuration;
            transform.localScale = Vector3.Lerp(startScale, perfectScale, progress);
            Color newColor = spriteRenderer.color;
            newColor.a = progress;
            spriteRenderer.color = newColor;
        }
        else if (timer < growDuration + perfectWindowDuration)
        {
            transform.localScale = perfectScale;
            Color newColor = spriteRenderer.color;
            newColor.a = 1f;
            spriteRenderer.color = newColor;
        }
        else if (timer < growDuration + perfectWindowDuration + overinflateDuration)
        {
            float overinflateTimer = timer - (growDuration + perfectWindowDuration);
            transform.localScale = Vector3.Lerp(perfectScale, endScale, overinflateTimer / overinflateDuration);
        }
        else
        {
            AudioManager.Instance.PlaySFX("Bubble-miss");
            rhythmController.NoteMissed();
            Destroy(gameObject);
        }

        float offsetX = Mathf.Sin(timer * hoverSpeed) * hoverAmplitude;
        float offsetY = Mathf.Cos(timer * hoverSpeed) * hoverAmplitude;
        transform.position = initialPosition + new Vector3(offsetX, offsetY, 0);
    }

    private void OnMouseDown()
    {
        AudioManager.Instance.PlaySFX("Button-click");

        if (noteToActivate != null)
        {
            noteToActivate.BecomeActiveNote();
        }

        Accuracy accuracy;
        if (timer >= growDuration && timer < growDuration + perfectWindowDuration)
        {
            accuracy = Accuracy.Perfect;
        }
        else if (timer >= growDuration * 0.5f && timer < growDuration)
        {
            accuracy = Accuracy.Good;
        }
        else
        {
            accuracy = Accuracy.Miss;
        }
        rhythmController.NoteHit(accuracy);
        noteCollider.enabled = false;
        Destroy(gameObject);
    }

    public void BecomeBackgroundNote()
    {
        SetState(false);
    }

    public void BecomeActiveNote()
    {
        SetState(true);
    }
    
    public void SetNoteToActivate(NoteObject note)
    {
        noteToActivate = note;
    }

    private void SetState(bool isBright)
    {
        if (isBright)
        {
            spriteRenderer.color = brightColor;
            spriteRenderer.sortingOrder = frontOrderInLayer;
            noteCollider.enabled = true; // PERUBAHAN UTAMA: Collider AKTIF
        }
        else
        {
            spriteRenderer.color = darkColor;
            spriteRenderer.sortingOrder = backOrderInLayer;
            noteCollider.enabled = false; // PERUBAHAN UTAMA: Collider MATI
        }
    }
}