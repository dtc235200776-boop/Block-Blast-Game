using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] gemSprites;
    [SerializeField] private Sprite defaultEmptySprite;

    public bool IsOccupied { get; private set; }

    // Đã cập nhật đúng tên biến Rows và Columns để khớp với BlockDragger.cs
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    private Color originalColor;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // --- BẠN THÊM HÀM NÀY VÀO ĐÂY ---
    private void Start()
    {
        // Khi game bắt đầu chạy, nếu ô chưa có ngọc thì lập tức ẩn đi (tàng hình)
        if (!IsOccupied)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = defaultEmptySprite;
                spriteRenderer.color = new Color(0f, 0f, 0f, 0f); // Thiết lập độ trong suốt Alpha = 0 (Tàng hình)
            }
        }
    }
    // --------------------------------

    // Cập nhật hàm Initialize
    public void Initialize(int x, int y)
    {
        Rows = x;
        Columns = y;
    }

    public void SetOccupied(bool occupied, int gemIndex)
    {
        IsOccupied = occupied;

        if (spriteRenderer == null) return;

        if (IsOccupied)
        {
            if (gemSprites != null && gemIndex >= 0 && gemIndex < gemSprites.Length)
            {
                spriteRenderer.sprite = gemSprites[gemIndex];
            }
            spriteRenderer.color = Color.white;

            spriteRenderer.size = new Vector2(0.9f, 0.9f);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10) + 10;
        }
        else
        {
            // Trả về ảnh mặc định
            spriteRenderer.sprite = defaultEmptySprite;

            // SỬA DÒNG NÀY: Chuyển màu về trong suốt (Alpha = 0) để lộ tấm nền Board sprite phía dưới
            spriteRenderer.color = new Color(0f, 0f, 0f, 0f);

            spriteRenderer.size = new Vector2(0.9f, 0.9f);
            spriteRenderer.sortingOrder = 1;
        }
    }
}