using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { PLAYER_TURN, ENEMY_TURN, CHECK_WIN_LOSS }

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    public static bool isVictory;

    [Header("Battle State")]
    public BattleState currentState;
    public int currentTurn = 1;

    [Header("Action Points (AP)")]
    public int maxAP = 3;
    public int currentAP;

    [Header("Units")]
    public BattleUnit playerUnit;
    public BattleUnit enemyUnit;

    [Header("UI Reference - Sliders")]
    public Slider playerHPSlider;
    public Slider enemyHPSlider;
    public TMP_Text playerHPLabel;
    public TMP_Text enemyHPLabel;

    [Header("UI Reference - Control Panel")]
    public TMP_InputField xInputField;
    public TMP_InputField yInputField;
    public Button executeButton;
    public TMP_Text apCounterText;
    public TMP_Text logText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Oyuna oyuncunun sırasıyla başla
        currentAP = maxAP;
        currentState = BattleState.PLAYER_TURN;
        
        // Arayüzü güncelle
        UpdateUI();
        
        // Başlangıç loglarını temizle ve ilk logu yaz
        logText.text = "> Start Turn 1.";
        logText.text += "\n> Player selection pending...";
    }

    // Arayüzdeki sliderları, yazıları ve AP göstergesini yeniler
    public void UpdateUI()
    {
        // Can Barlarını Güncelle
        if (playerUnit != null && playerHPSlider != null)
        {
            playerHPSlider.value = playerUnit.currentHealth;
            playerHPLabel.text = $"Player        Health: {playerUnit.currentHealth}/{playerUnit.maxHealth} HP";
        }
        
        if (enemyUnit != null && enemyHPSlider != null)
        {
            enemyHPSlider.value = enemyUnit.currentHealth;
            enemyHPLabel.text = $"Enemy         Health: {enemyUnit.currentHealth}/{enemyUnit.maxHealth} HP";
        }

        // AP Sayacını Güncelle
        if (apCounterText != null)
        {
            apCounterText.text = $"AP: {currentAP}/{maxAP}";
        }
    }

    // Günlük (Log) ekranına yeni satır ekleyen yardımcı fonksiyon
    public void LogMessage(string message)
    {
        if (logText != null)
        {
            logText.text += $"\n{message}";
        }
    }

    // Oyuncunun hamle hakları tükendiğinde sırayı düşmana devreden fonksiyon
    public void EndPlayerTurn()
    {
        if (currentState != BattleState.PLAYER_TURN) return;

        currentState = BattleState.ENEMY_TURN;
        LogMessage("> Player turn ended. Paradox is thinking...");
        
        // Oyuncu turu bittiği için girdileri ve butonu kilitliyoruz (Hile engelleme)
        SetPlayerInputInteractable(false);

        // Düşman hamlesini 1.5 saniye gecikmeyle başlat (oyuncu okuyabilsin diye)
        StartCoroutine(EnemyTurnCoroutine());
    }

    // Oyuncu girdilerini açıp kapatan fonksiyon
    public void SetPlayerInputInteractable(bool interactable)
    {
        if (xInputField != null) xInputField.interactable = interactable;
        if (yInputField != null) yInputField.interactable = interactable;
        if (executeButton != null) executeButton.interactable = interactable;
    }

    // Düşman sırasını yöneten Coroutine (gecikmeli işlem)
    private IEnumerator EnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        if (currentState != BattleState.ENEMY_TURN) yield break;

        // Düşman oyuncuya saldırsın (Basit yapay zeka: Doğrudan 15 hasar vurur)
        if (playerUnit != null)
        {
            playerUnit.TakeDamage(15);
            LogMessage("<color=red>> Paradox attacks Scholar for 15 DMG!</color>");
            UpdateUI();
        }

        yield return new WaitForSeconds(1.0f);

        // Oyuncu öldü mü kontrol et, ölmediyse sırayı oyuncuya geri ver
        if (playerUnit != null && playerUnit.currentHealth <= 0)
        {
            currentState = BattleState.CHECK_WIN_LOSS;
            LogMessage("<color=red>> Scholar has fallen! Paradox has won...</color>");
            isVictory = false;
            StartCoroutine(LoadEndSceneAfterDelay(2f));
        }
        else
        {
            // Sırayı oyuncuya geri devret
            currentTurn++;
            currentAP = maxAP; // AP yenile (3/3)
            currentState = BattleState.PLAYER_TURN;
            
            LogMessage($"\n> Start Turn {currentTurn}.");
            LogMessage("> Player selection pending...");
            
            // Oyuncu girdilerini tekrar aç
            SetPlayerInputInteractable(true);
            UpdateUI();
        }
    }

    // AP harcama fonksiyonu
    public void UseAP(int amount)
    {
        currentAP = Mathf.Max(0, currentAP - amount);
        UpdateUI();

        if (currentAP <= 0)
        {
            EndPlayerTurn();
        }
    }

    // Oyuncunun butona bastığında çalışan Koordinat Saldırısı fonksiyonu
    public void ExecuteAttack()
    {
        // Sadece oyuncunun sırasındayken çalışır
        if (currentState != BattleState.PLAYER_TURN) return;

        string xStr = xInputField.text;
        string yStr = yInputField.text;

        // 1. Girdi Doğrulama (Validation)
        if (string.IsNullOrEmpty(xStr) || string.IsNullOrEmpty(yStr))
        {
            LogMessage("<color=yellow>> Error: Coordinates cannot be empty!</color>");
            return;
        }

        int xInput, yInput;
        bool xValid = int.TryParse(xStr, out xInput);
        bool yValid = int.TryParse(yStr, out yInput);

        if (!xValid || !yValid || xInput < 0 || xInput > 4 || yInput < 0 || yInput > 4)
        {
            LogMessage("<color=yellow>> Error: Coordinates must be numbers between 0 and 4!</color>");
            return;
        }

        // 2. Log Ekleme
        LogMessage($"> Player fires strike at ({xInput}, {yInput})...");

        // 3. İsabet Kontrolü (Cartesian Coordinate Check)
        if (enemyUnit != null && xInput == enemyUnit.gridX && yInput == enemyUnit.gridY)
        {
            // İSABET (HIT! - Pozitif Pekiştireç)
            enemyUnit.TakeDamage(25);
            LogMessage("<color=green>> HIT! Struck the Paradox for 25 DMG!</color>");

            // Düşman canı bitti mi kontrolü (Kazanma Durumu)
            if (enemyUnit.currentHealth <= 0)
            {
                currentState = BattleState.CHECK_WIN_LOSS;
                LogMessage("<color=green>> PARADOX DESTROYED! Logic is restored!</color>");
                isVictory = true;
                StartCoroutine(LoadEndSceneAfterDelay(2f));

                // Kalan girdileri kapat
                xInputField.text = "";
                yInputField.text = "";
                UpdateUI();
                SetPlayerInputInteractable(false);
                return;
            }
        }
        else
        {
            // ISKALAMA (MISS! - Yanılgıdan Öğrenme)
            LogMessage("<color=yellow>> MISS! Coordinates did not intersect the Paradox.</color>");
        }

        // Giriş kutularını temizle
        xInputField.text = "";
        yInputField.text = "";

        // Can barlarını ve değerlerini hemen güncelle
        UpdateUI();

        // 4. AP harca (1 AP)
        UseAP(1);
    }

    // Belirli bir gecikmeyle Bitiş Ekranını yükleyen coroutine
    private IEnumerator LoadEndSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
    }
}
