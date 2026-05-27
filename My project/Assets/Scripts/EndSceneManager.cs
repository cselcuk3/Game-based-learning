using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text resultTitleText;
    public TMP_Text resultDescriptionText;

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
        SceneManager.LoadScene("BattleScene");
    }

    // Ana menüye dön butonu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
