using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [Header("Unit Stats")]
    public string unitName;
    public bool isPlayer;
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Grid Position (0-4)")]
    public int gridX;
    public int gridY;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        SnapToGrid();
    }

    // Karakteri grid üzerindeki koordinatının tam üstüne ışınlar
    public void SnapToGrid()
    {
        if (GridManager.Instance != null)
        {
            transform.position = GridManager.Instance.GetWorldPosition(gridX, gridY);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
    }
}
