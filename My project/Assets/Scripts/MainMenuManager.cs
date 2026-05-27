using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

    // Oyun sahnesini yükleyen fonksiyon (Play Butonuna bağlayacağız)
    public void PlayGame()
    {
        StartCoroutine(PlaySoundAndLoadScene("BattleScene"));
    }

    // Oyundan çıkış fonksiyonu (Quit Butonuna bağlayacağız)
    public void QuitGame()
    {
        StartCoroutine(PlaySoundAndQuit());
    }

    // Tıklama sesini 0.57. saniyeden başlatarak çalan fonksiyon
    public void PlayClickSound()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.clip = clickSound;
            if (0.57f < clickSound.length)
            {
                sfxSource.time = 0.57f;
            }
            else
            {
                sfxSource.time = 0f;
            }
            sfxSource.Play();
        }
    }

    // Sesi çalıp 0.25 saniye bekledikten sonra sahneyi yükleyen yardımcı coroutine
    private System.Collections.IEnumerator PlaySoundAndLoadScene(string sceneName)
    {
        PlayClickSound();
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(sceneName);
    }

    // Sesi çalıp 0.25 saniye bekledikten sonra oyundan çıkan yardımcı coroutine
    private System.Collections.IEnumerator PlaySoundAndQuit()
    {
        PlayClickSound();
        yield return new WaitForSeconds(0.25f);
        Debug.Log("> Exiting Game...");
        Application.Quit();
    }
}
