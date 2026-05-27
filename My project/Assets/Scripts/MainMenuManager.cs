using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Oyun sahnesini yükleyen fonksiyon (Play Butonuna bağlayacağız)
    public void PlayGame()
    {
        SceneManager.LoadScene("BattleScene");
    }

    // Oyundan çıkış fonksiyonu (Quit Butonuna bağlayacağız)
    public void QuitGame()
    {
        Debug.Log("> Exiting Game...");
        Application.Quit();
    }
}
