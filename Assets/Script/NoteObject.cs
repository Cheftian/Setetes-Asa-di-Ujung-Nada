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
    
    [Header("Pengaturan Gerakan")]
    [SerializeField] private float movementRadius = 0.5f;
    [SerializeField] private float movementSpeed = 1f;
    
    private float timer = 0f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private RhythmController rhythmController; 
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(RhythmController controller)
    {
        rhythmController = controller;
    }

    void Start()
    {
        AudioManager.Instance.PlaySFX("Bubble-pop");
        initialPosition = transform.position;
        targetPosition = initialPosition + (Vector3)(Random.insideUnitCircle * movementRadius);

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

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);
    }

    private void OnMouseDown()
    {
        // SFX tidak diubah sesuai permintaan
        AudioManager.Instance.PlaySFX("Button-click"); 
        
        Accuracy accuracy;

        // Jendela "Sempurna"
        if (timer >= growDuration && timer < growDuration + perfectWindowDuration)
        {
            accuracy = Accuracy.Perfect;
        }
        // PERUBAHAN UTAMA: Jendela "Baik" sekarang lebih besar
        // Dimulai dari 50% masa pertumbuhan hingga sesaat sebelum jendela "Sempurna"
        else if (timer >= growDuration * 0.2f && timer < growDuration) 
        {
            accuracy = Accuracy.Good;
        }
        // Jika diklik terlalu cepat atau sudah lewat, dianggap "Miss"
        else
        {
            accuracy = Accuracy.Miss;
        }

        rhythmController.NoteHit(accuracy);

        GetComponent<Collider2D>().enabled = false; 
        Destroy(gameObject); 
    }
}