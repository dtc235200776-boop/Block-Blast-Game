using UnityEngine;

public class BlockDragger : MonoBehaviour
{
    private Camera mainCamera;
    private Block block;
    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private BoardManager boardManager;

    [Header("Cài đặt kéo thả")]
    [SerializeField] private float dragOffsetY = 1f; // Tránh ngón tay/chuột che khuất khối hình khi kéo

    private void Awake()
    {
        mainCamera = Camera.main;
        block = GetComponent<Block>();
        boardManager = FindFirstObjectByType<BoardManager>();
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

        // Phóng to về kích thước 100% để khớp hoàn toàn với ô cờ trên bàn cờ khi kéo
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

            // THUẬT TOÁN MỚI: Tìm ô con thực tế đầu tiên (ô hiển thị trực quan) của Block
            // thay vì lấy tâm transform.position của Block cha.
            Vector3 firstCellWorldPos = GetFirstActiveCellWorldPosition();

            Cell nearestCell = boardManager.GetNearestCell(firstCellWorldPos);

            if (nearestCell != null)
            {
                // Nếu bàn cờ còn chỗ trống hợp lệ cho khối này tại vị trí đích
                if (boardManager.CanPlaceBlock(data, nearestCell.Rows, nearestCell.Columns))
                {
                    // Tiến hành đặt khối và nhuộm màu bàn cờ
                    boardManager.PlaceBlock(data, nearestCell.Rows, nearestCell.Columns);

                    // Xóa khối block kéo đi vì nó đã được vẽ cố định lên bàn cờ
                    Destroy(gameObject);
                    return;
                }
            }
        }

        // Nếu thả sai chỗ hoặc thả ra ngoài, tự bay về vị trí ban đầu và thu nhỏ 60%
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

    // Hàm tìm vị trí thế giới (World Position) của ô con hiển thị đầu tiên của Block
    private Vector3 GetFirstActiveCellWorldPosition()
    {
        // Duyệt qua tất cả các ô con được sinh ra (các Cell(Clone) bên dưới Block)
        if (transform.childCount > 0)
        {
            // Trả về vị trí thực tế của ô con đầu tiên trong không gian 2D
            return transform.GetChild(0).position;
        }

        // Dự phòng nếu không có ô con nào thì lấy vị trí của cha
        return transform.position;
    }
}