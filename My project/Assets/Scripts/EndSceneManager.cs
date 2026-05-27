using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text resultTitleText;
    public TMP_Text resultDescriptionText;

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

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

    private void Start()
    {
        // BattleManager'daki static değişkenden galibiyet durumunu okuyoruz
        if (BattleManager.isVictory)
        {
            // OYUNCU KAZANDI (LOGIC RESTORED!)
            if (resultTitleText != null)
            {
                resultTitleText.text = "<color=green>LOGIC RESTORED</color>";
            }
            if (resultDescriptionText != null)
            {
                resultDescriptionText.text = "The Paradoxes have been neutralized.\nThe Prime Continuum is saved!";
            }
        }
        else
        {
            // OYUNCU KAYBETTİ (PARADOX OVERLOAD!)
            if (resultTitleText != null)
            {
                resultTitleText.text = "<color=red>PARADOX OVERLOAD</color>";
            }
            if (resultDescriptionText != null)
            {
                resultDescriptionText.text = "Mathematical anomalies have collapsed reality.\nThe Scholar has been deleted.";
            }
        }
    }

    // Yeniden oyna butonu (BattleScene'i baştan yükler)
    public void RestartGame()
    {
        StartCoroutine(PlaySoundAndLoadScene("BattleScene"));
    }

    // Ana menüye dön butonu
    public void LoadMainMenu()
    {
        StartCoroutine(PlaySoundAndLoadScene("MainMenu"));
    }

    // Sesi çalıp 0.25 saniye bekledikten sonra sahneyi yükleyen yardımcı coroutine
    private System.Collections.IEnumerator PlaySoundAndLoadScene(string sceneName)
    {
        PlayClickSound();
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(sceneName);
    }
}
