using UnityEngine;

public class BlockDragger : MonoBehaviour
{
    private Camera mainCamera;
    private Block block;
    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private BoardManager boardManager;
    private BlockSpawner blockSpawner;

    [Header("Cài đặt kéo thả")]
    [SerializeField] private float dragOffsetY = 1f;

    private void Awake()
    {
        mainCamera = Camera.main;
        block = GetComponent<Block>();

        // Tự động tìm kiếm các Manager trong Scene
        boardManager = FindFirstObjectByType<BoardManager>();
        blockSpawner = FindFirstObjectByType<BlockSpawner>();
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = transform.position - mouseWorldPos;
        transform.localScale = Vector3.one; // Đưa về kích thước 100% khi kéo
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 targetPos = GetMouseWorldPosition() + offset;
        targetPos.y += dragOffsetY;
        targetPos.z = 0f;

        transform.position = targetPos;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if (boardManager != null && block != null)
        {
            BlockData data = block.GetBlockData();

            // 1. Lấy vị trí thế giới của ô con đầu tiên (bám bắt cực nhạy và chính xác)
            Vector3 firstCellWorldPos = GetFirstActiveCellWorldPosition();
            Cell nearestCell = boardManager.GetNearestCell(firstCellWorldPos);

            if (nearestCell != null)
            {
                // 2. Tìm xem ô con đầu tiên này nằm ở dòng/cột nào trong ma trận cấu trúc của Block
                GetFirstActiveCellIndices(data, out int firstActiveRow, out int firstActiveCol);

                // 3. THUẬT TOÁN BÙ TRỪ: Tính toán ngược lại vị trí gốc (startRow, startCol) chuẩn xác trên Board
                int startRow = nearestCell.Rows + firstActiveRow;
                int startCol = nearestCell.Columns - firstActiveCol;

                // 4. Ướm thử và đặt khối gạch
                if (boardManager.CanPlaceBlock(data, startRow, startCol))
                {
                    boardManager.PlaceBlock(data, startRow, startCol);

                    // Báo cho Spawner kiểm tra khay chờ để nạp lượt mới
                    if (blockSpawner != null)
                    {
                        blockSpawner.CheckAndRepopulate();
                    }

                    Destroy(gameObject);
                    return;
                }
            }
        }

        ReturnToDefault();
    }

    private void ReturnToDefault()
    {
        transform.position = originalPosition;
        transform.localScale = Vector3.one * 0.6f; // Thu nhỏ lại 60% khi về khay chờ
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // Lấy vị trí ô con đầu tiên thực tế của Block
    private Vector3 GetFirstActiveCellWorldPosition()
    {
        if (transform.childCount > 0)
        {
            return transform.GetChild(0).position;
        }
        return transform.position;
    }

    // Hàm phụ tính toán vị trí tương đối của ô con đầu tiên trong ma trận BlockData
    private void GetFirstActiveCellIndices(BlockData data, out int firstRow, out int firstCol)
    {
        firstRow = 0;
        firstCol = 0;
        for (int r = 0; r < data.Rows; r++)
        {
            for (int c = 0; c < data.Columns; c++)
            {
                if (data.Shape[r, c])
                {
                    firstRow = r;
                    firstCol = c;
                    return;
                }
            }
        }
    }
}