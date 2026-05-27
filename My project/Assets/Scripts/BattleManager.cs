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

    [Header("Audio Settings")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip warpSound;
    public AudioClip enemyAttackSound;
    public AudioClip winSound;
    public AudioClip loseSound;

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
        
        // Başlangıç loglarını temizle ve ilk logları ekle
        LogMessage("> Start Turn 1.");
        LogMessage("> Player selection pending...");

        // Arkaplan müziğini başlat
        if (bgmSource != null && bgmSource.clip != null)
        {
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // Input alanlarına tıklandığında (seçildiğinde) klik sesini otomatik çal (0.57s'den başlat)
        if (xInputField != null)
        {
            xInputField.onSelect.AddListener(delegate { PlaySFXFromTime(clickSound, 0.57f); });
        }
        if (yInputField != null)
        {
            yInputField.onSelect.AddListener(delegate { PlaySFXFromTime(clickSound, 0.57f); });
        }
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

    private System.Collections.Generic.List<string> logLines = new System.Collections.Generic.List<string>();

    // Günlük (Log) ekranına yeni satır ekleyen ve son 8 satırı tutarak taşmayı önleyen fonksiyon
    public void LogMessage(string message)
    {
        if (logText != null)
        {
            logLines.Add(message);
            // Konsolun dışına taşmaması için sadece son 8 satırı tutuyoruz
            if (logLines.Count > 8)
            {
                logLines.RemoveAt(0);
            }
            logText.text = string.Join("\n", logLines);
        }
    }

    // Ses Efekti (SFX) çalan yardımcı fonksiyon
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Ses Efektini (SFX) belirli bir saniyeden başlatarak çalan gelişmiş yardımcı fonksiyon
    public void PlaySFXFromTime(AudioClip clip, float startTime)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.clip = clip;
            // Emniyet Kontrolü: Başlangıç süresi ses dosyasının uzunluğundan kısa olmalı
            if (startTime < clip.length)
            {
                sfxSource.time = startTime;
            }
            else
            {
                sfxSource.time = 0f;
            }
            sfxSource.Play();
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

        // Düşman oyuncuya saldırsın (Basit yapay zeka: Doğrudan 25 hasar vurur)
        if (playerUnit != null)
        {
            playerUnit.TakeDamage(25);
            LogMessage("<color=red>> Paradox attacks Scholar for 25 DMG!</color>");
            PlaySFX(enemyAttackSound); // Düşman saldırı sesi

            // Oyuncunun durduğu kareyi (Tile) hızlıca 2 kez kırmızı yakıp söndür (Gecikmesiz!)
            if (GridManager.Instance != null)
            {
                GridManager.Instance.FlashTileRed(playerUnit.gridX, playerUnit.gridY);
            }

            UpdateUI();
        }

        yield return new WaitForSeconds(1.0f);

        // Oyuncu öldü mi kontrol et, ölmediyse sırayı oyuncuya geri ver
        if (playerUnit != null && playerUnit.currentHealth <= 0)
        {
            currentState = BattleState.CHECK_WIN_LOSS;
            LogMessage("<color=red>> Scholar has fallen! Paradox has won...</color>");
            PlaySFX(loseSound); // Kaybetme sesi
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
                PlaySFX(winSound); // Kazanma sesi

                // Son duran kareyi hemen 2 kere kırmızı yap
                if (GridManager.Instance != null)
                {
                    GridManager.Instance.FlashTileRed(enemyUnit.gridX, enemyUnit.gridY);
                }

                isVictory = true;
                StartCoroutine(LoadEndSceneAfterDelay(4f));

                // Kalan girdileri kapat
                xInputField.text = "";
                yInputField.text = "";
                UpdateUI();
                SetPlayerInputInteractable(false);
                return;
            }
            else
            {
                // Düşman ölmediyse 7 saniye gecikmeli isabet ve ışınlanma coroutine'ini başlat!
                StartCoroutine(HitAndTeleportCoroutine());
            }
        }
        else
        {
            // ISKALAMA (MISS! - Yanılgıdan Öğrenme)
            LogMessage("<color=yellow>> MISS! Coordinates did not intersect the Paradox.</color>");
            PlaySFXFromTime(missSound, 0.5f); // Iskalamada ses çal (0.5sn'den başlat)

            // Iskalanan hücreyi hızlıca 2 kez kırmızı yap (Gecikmesiz!)
            if (GridManager.Instance != null)
            {
                GridManager.Instance.FlashTileRed(xInput, yInput);
            }
        }

        // Giriş kutularını temizle
        xInputField.text = "";
        yInputField.text = "";

        // Can barlarını ve değerlerini hemen güncelle
        UpdateUI();

        // 4. AP harca (1 AP)
        UseAP(1);
    }

    // İsabet aldığında hemen ses çalan, kırmızı yanan ve 3 saniye sonra yer değiştiren coroutine
    private IEnumerator HitAndTeleportCoroutine()
    {
        // 1. İsabet sesini 3. saniyesinden başlatarak çal (Gelişmiş Audio API)
        PlaySFXFromTime(hitSound, 3.0f);

        // 2. Düşmanın olduğu kareyi hemen hızlıca 2 kez kırmızı yap
        if (GridManager.Instance != null && enemyUnit != null)
        {
            GridManager.Instance.FlashTileRed(enemyUnit.gridX, enemyUnit.gridY);
        }

        // Girişleri hemen kilitle ki oyuncu bekleme süresinde hile yapamasın
        SetPlayerInputInteractable(false);

        // 3. Gecikmeyi tam 3 saniye olarak ayarla
        yield return new WaitForSeconds(3f);

        // 4. Düşmanın yerini değiştir (ışınlanma sesi ve log Teleport fonksiyonunun içindedir)
        TeleportEnemyToRandomPosition();

        // Girişleri tekrar aç
        SetPlayerInputInteractable(true);
    }

    // Düşmanı, oyuncunun üstünde olmayacak şekilde rastgele bir koordinata ışınlar
    private void TeleportEnemyToRandomPosition()
    {
        if (enemyUnit == null || playerUnit == null) return;

        int newX;
        int newY;

        // Oyuncunun durduğu kareyle çakışmayana kadar rastgele koordinat üret
        do
        {
            newX = Random.Range(0, 5);
            newY = Random.Range(0, 5);
        }
        while (newX == playerUnit.gridX && newY == playerUnit.gridY);

        // Yeni koordinatları düşmana ata ve fiziksel olarak ışınla
        enemyUnit.gridX = newX;
        enemyUnit.gridY = newY;
        enemyUnit.SnapToGrid();

        PlaySFXFromTime(warpSound, 1.5f); // Işınlanma sesi çal (1.5sn'den başlat)
        LogMessage("<color=cyan>> Paradox warped! Find it at its new coordinates.</color>");
    }

    // Belirli bir gecikmeyle Bitiş Ekranını yükleyen coroutine
    private IEnumerator LoadEndSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
    }
}
