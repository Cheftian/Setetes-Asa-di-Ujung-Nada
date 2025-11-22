using UnityEngine;
using System.Collections;

// Pastikan komponen CanvasGroup ada pada GameObject yang sama
[RequireComponent(typeof(CanvasGroup))]
public class GuideController : MonoBehaviour
{
    [Header("Pengaturan Waktu")]
    [Tooltip("Jeda sebelum panel muncul (detik).")]
    [SerializeField] private float startDelay = 2f;
    [Tooltip("Berapa lama panel ditampilkan (detik).")]
    [SerializeField] private float displayDuration = 5f;
    [Tooltip("Berapa lama efek fade-in/out (detik).")]
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Ambil komponen CanvasGroup secara otomatis
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        // Mulai rutin secara otomatis saat objek ini aktif
        StartCoroutine(ShowGuideRoutine());
    }

    private IEnumerator ShowGuideRoutine()
    {
        // 1. Atur kondisi awal (sepenuhnya transparan)
        canvasGroup.alpha = 0f;
        
        // 2. Tunggu beberapa detik setelah level dimulai
        yield return new WaitForSeconds(startDelay);

        // 3. Proses Fade-in
        float timer = 0f;
        while(timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 4. Tunggu selama panel ditampilkan
        yield return new WaitForSeconds(displayDuration);

        // 5. Proses Fade-out
        timer = 0f; // Reset timer
        while(timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        
        // 6. Nonaktifkan GameObject setelah selesai agar tidak mengganggu
        gameObject.SetActive(false);
    }
}