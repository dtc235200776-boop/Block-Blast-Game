using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public bool IsOccupied { get; private set; } = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void Initialize(int row, int col)
    {
        Rows = row;
        Columns = col;
    }

    // Thiết lập trạng thái ô đã bị chiếm hay chưa
    public void SetOccupied(bool occupied, Color color)
    {
        IsOccupied = occupied;
        if (spriteRenderer != null)
        {
            // Nếu bị chiếm thì đổi sang màu của Block, nếu trống thì quay về màu xám ban đầu
            spriteRenderer.color = occupied ? color : originalColor;
        }
    }
}