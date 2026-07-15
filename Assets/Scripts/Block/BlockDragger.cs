using UnityEngine;

public class BlockDragger : MonoBehaviour
{
    private Camera mainCamera;
    private Block block;
    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private BoardManager boardManager;
    private BlockSpawner blockSpawner; // Thêm tham chiếu tới BlockSpawner

    [Header("Cài đặt kéo thả")]
    [SerializeField] private float dragOffsetY = 1f;

    private void Awake()
    {
        mainCamera = Camera.main;
        block = GetComponent<Block>();

        // Tự động tìm kiếm các Manager trong Scene khi bắt đầu
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
        transform.localScale = Vector3.one;
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
            Vector3 firstCellWorldPos = GetFirstActiveCellWorldPosition();

            Cell nearestCell = boardManager.GetNearestCell(firstCellWorldPos);

            if (nearestCell != null)
            {
                if (boardManager.CanPlaceBlock(data, nearestCell.Rows, nearestCell.Columns))
                {
                    boardManager.PlaceBlock(data, nearestCell.Rows, nearestCell.Columns);

                    // --- BÁO CHO SPAWNER KIỂM TRA KHAY CHỜ TRƯỚC KHI HỦY BLOCK ---
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
        transform.localScale = Vector3.one * 0.6f;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private Vector3 GetFirstActiveCellWorldPosition()
    {
        if (transform.childCount > 0)
        {
            return transform.GetChild(0).position;
        }
        return transform.position;
    }
}