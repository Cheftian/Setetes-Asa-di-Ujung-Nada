using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Membuatnya bisa diakses dari mana saja (Singleton)
    public static CameraShake Instance;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Simpan posisi awal kamera
        originalPosition = transform.localPosition;
    }

    // Fungsi utama yang akan kita panggil dari script lain
    public void Shake(float duration, float magnitude)
    {
        // Hentikan guncangan sebelumnya jika masih berjalan
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        // Mulai guncangan baru
        shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Buat posisi acak di dalam sebuah lingkaran
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Terapkan posisi acak itu ke posisi kamera
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null; // Tunggu frame berikutnya
        }

        // Kembalikan kamera ke posisi semula setelah selesai
        transform.localPosition = originalPosition;
    }
}