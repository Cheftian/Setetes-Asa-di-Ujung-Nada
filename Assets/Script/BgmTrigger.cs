using UnityEngine;

public class BgmTrigger : MonoBehaviour
{
    public string bgName;

    void Awake()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(bgName);
        }
        else
        {
            Debug.LogError("AudioManager tidak ditemukan! Pastikan scene Main Menu dijalankan pertama kali.");
        }
    }
}
