using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Diğer kodların bu sınıfı kolayca bulabilmesi için Singleton yapısı
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float spacing = 1.05f; // Kareler arasındaki mesafe

    [Header("Prefabs")]
    public GameObject tilePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateGrid();
    }

    // 5x5 Izgarayı programatik olarak oluşturan döngü
    private void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Pozisyonu GridContainer'a göre hesapla
                Vector3 spawnPos = new Vector3(x * spacing, y * spacing, 0);
                
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = spawnPos;
                tile.name = $"Tile_{x}_{y}";

                // En alt satırın (y = 0) altına X koordinat etiketlerini yerleştir
                if (y == 0)
                {
                    CreateCoordinateLabel($"{x}", new Vector3(x * spacing, -0.7f, 0));
                }

                // En sol sütunun (x = 0) soluna Y koordinat etiketlerini yerleştir
                if (x == 0)
                {
                    CreateCoordinateLabel($"{y}", new Vector3(-0.7f, y * spacing, 0));
                }
            }
        }
    }

    // Koordinat sayı etiketlerini (0-4) otomatik oluşturan yardımcı fonksiyon
    private void CreateCoordinateLabel(string labelText, Vector3 localPos)
    {
        GameObject labelObj = new GameObject($"CoordLabel_{labelText}");
        labelObj.transform.SetParent(transform);
        labelObj.transform.localPosition = localPos;

        // Dinamik olarak TextMeshPro bileşeni ekliyoruz
        TMPro.TextMeshPro tmp = labelObj.AddComponent<TMPro.TextMeshPro>();
        tmp.text = labelText;
        tmp.fontSize = 5;
        tmp.fontStyle = TMPro.FontStyles.Bold;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.color = new Color(0.7f, 0.76f, 0.83f); // Hoş bir açık gri-mavi renk
    }

    // Grid koordinatını (örn: 3, 4) oyun dünyasındaki fiziksel Vector3 pozisyonuna çevirir
    public Vector3 GetWorldPosition(int x, int y)
    {
        return transform.position + new Vector3(x * spacing, y * spacing, 0);
    }

    // Belirli bir grid hücresini (Tile) hızlıca 2 kere kırmızı renge çeviren animasyon fonksiyonu
    public void FlashTileRed(int x, int y)
    {
        Transform tileTrans = transform.Find($"Tile_{x}_{y}");
        if (tileTrans != null)
        {
            StartCoroutine(FlashTileCoroutine(tileTrans.gameObject));
        }
    }

    private System.Collections.IEnumerator FlashTileCoroutine(GameObject tileObj)
    {
        SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        Color flashColor = Color.red;

        // 1. Kırmızı Işık Yanıp Sönmesi
        sr.color = flashColor;
        yield return new WaitForSeconds(0.15f);
        sr.color = originalColor;
        yield return new WaitForSeconds(0.15f);

        // 2. Kırmızı Işık Yanıp Sönmesi
        sr.color = flashColor;
        yield return new WaitForSeconds(0.15f);
        sr.color = originalColor;
    }
}
