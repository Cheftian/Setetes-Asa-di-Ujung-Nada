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
    
    private float timer = 0f;
    private RhythmController rhythmController; 
    private SpriteRenderer spriteRenderer;

    private Vector3 initialPosition;

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

        Color startColor = spriteRenderer.color;
        startColor.a = 0f;
        spriteRenderer.color = startColor;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Logika Skala & Opacity
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

        // Logika Gerakan Mengambang
        float offsetX = Mathf.Sin(timer * hoverSpeed) * hoverAmplitude;
        float offsetY = Mathf.Cos(timer * hoverSpeed) * hoverAmplitude;
        transform.position = initialPosition + new Vector3(offsetX, offsetY, 0);
    }

    private void OnMouseDown()
    {
        AudioManager.Instance.PlaySFX("Button-click"); 
        
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

        GetComponent<Collider2D>().enabled = false; 
        Destroy(gameObject); 
    }
}