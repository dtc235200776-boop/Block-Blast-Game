using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Sinh Block
public class BlockSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject blockPrefab;

    [Header("Điểm sinh Block")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("References")]
    [SerializeField] private BoardManager boardManager;

    // Danh sách chứa tất cả các loại Block
    private List<BlockData> blockList;

    // Lá cờ ngăn chặn CheckGameOver chạy sai thời điểm
    private bool isSpawningNewRound = false;

    private void Start()
    {
        // Khởi tạo danh sách Block
        blockList = new List<BlockData>()
        {
            BlockLibrary.CreateDotBlock(),          // Chấm nhỏ 1x1
            BlockLibrary.CreateSquare2x2(),         // Vuông 2x2
            BlockLibrary.CreateSquare3x3(),         // Vuông to 3x3
            BlockLibrary.CreateLine2(),             // Thanh ngang 2 ô
            BlockLibrary.CreateLine3(),             // Thanh ngang 3 ô
            BlockLibrary.CreateLine4(),             // Thanh ngang 4 ô
            BlockLibrary.CreateLineVertical3(),     // Thanh dọc 3 ô
            BlockLibrary.CreateLNormal(),           // Chữ L xuôi
            BlockLibrary.CreateLReverse(),          // Chữ L ngược
            BlockLibrary.CreateTBlock(),            // Chữ T
            BlockLibrary.CreateZBlock(),            // Chữ Z
            BlockLibrary.CreateSBlock(),            // Chữ S
            BlockLibrary.CreateMiniCorner()         // Khối góc chữ L mini 2x2
        };

        SpawnAllBlocks();
    }

    // SINH THÔNG MINH: Đảm bảo bộ 3 block sinh ra có ít nhất một nước đi khả dụng
    public void SpawnAllBlocks()
    {
        if (boardManager == null)
        {
            Debug.LogError("Chưa kéo thả BoardManager vào BlockSpawner trong Inspector!");
            return;
        }

        List<BlockData> selectedGroup = new List<BlockData>();
        int maxAttempts = 20; // Thử lại tối đa 20 lần để tìm ra bộ gạch giải được
        int attempts = 0;
        bool hasValidMove = false;

        while (attempts < maxAttempts && !hasValidMove)
        {
            selectedGroup.Clear();

            // 1. Tạo nháp bộ 3 khối gạch ngẫu nhiên
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                int randomIndex = Random.Range(0, blockList.Count);
                selectedGroup.Add(blockList[randomIndex]);
            }

            // 2. Ướmt thử xem có khối nào trong bộ 3 này đi được trên bàn cờ hiện tại không
            foreach (BlockData data in selectedGroup)
            {
                if (boardManager.HasAnyValidMove(data))
                {
                    hasValidMove = true;
                    break;
                }
            }

            attempts++;
        }

        Debug.Log($"[SmartSpawner] Tìm thấy bộ gạch hợp lệ sau {attempts} lần thử.");

        // 3. Thực hiện tạo thực tế các khối gạch đã được duyệt hợp lệ lên khay chờ
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnBlock(spawnPoints[i], selectedGroup[i]);
        }
    }

    // Sinh một Block cụ thể tại vị trí chỉ định
    private void SpawnBlock(Transform spawnPoint, BlockData data)
    {
        // Sinh Block và đặt trực tiếp làm con của spawnPoint để quản lý vị trí
        GameObject blockObject = Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

        // Đưa block về chính giữa spawnPoint
        blockObject.transform.localPosition = Vector3.zero;

        // Lấy script Block
        Block block = blockObject.GetComponent<Block>();

        // Khởi tạo hình dáng
        block.Initialize(data, boardManager.CellSize);

        // Thu nhỏ khối block lại bằng 60% kích thước chuẩn khi nằm ở khay chờ cho đẹp mắt
        block.transform.localScale = Vector3.one * 0.6f;
    }

    // --- KIỂM TRA VÀ TỰ ĐỘNG BƠM LƯỢT HÌNH MỚI ---
    public void CheckAndRepopulate()
    {
        StartCoroutine(CheckAfterFrame());
    }

    // Hãy gọi hàm này mỗi khi người chơi đặt thành công một khối gạch xuống bàn cờ
    public void CheckGameOver()
    {
        // Nếu đang trong quá trình sinh lượt mới, KHÔNG được phép check Game Over để tránh reset nhầm
        if (isSpawningNewRound)
        {
            Debug.Log("[CheckGameOver] Đang sinh lượt mới, BỎ QUA kiểm tra!");
            return;
        }

        // Chỉ quét các Block thực sự đang tồn tại trên Scene
        Block[] activeBlocks = FindObjectsByType<Block>(FindObjectsSortMode.None);

        List<Block> actualActiveBlocks = new List<Block>();
        foreach (Block b in activeBlocks)
        {
            // Loại bỏ các khối gạch null hoặc đang bị ẩn/chờ xóa
            if (b != null && b.gameObject != null && b.gameObject.activeInHierarchy)
            {
                actualActiveBlocks.Add(b);
            }
        }

        Debug.Log($"[CheckGameOver] Đang kiểm tra. Số lượng Block hoạt động: {actualActiveBlocks.Count}");

        // Nếu khay trống thực sự, không check thua
        if (actualActiveBlocks.Count == 0)
        {
            Debug.Log("[CheckGameOver] Khay trống hoàn toàn, BỎ QUA!");
            return;
        }

        if (boardManager == null) return;

        // Duyệt qua từng khối gạch còn hoạt động
        foreach (Block block in actualActiveBlocks)
        {
            BlockData data = block.GetBlockData();

            // Đảm bảo dữ liệu block đã tồn tại và đi được
            if (data != null)
            {
                bool canPlace = boardManager.HasAnyValidMove(data);
                Debug.Log($"[CheckGameOver] Khối {block.name} - Có nước đi: {canPlace}");
                if (canPlace)
                {
                    Debug.Log("[CheckGameOver] GAME TIẾP TỤC - Vẫn còn nước đi hợp lệ!");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("[CheckGameOver] Phát hiện một Block có dữ liệu BlockData bằng NULL!");
            }
        }

        // Nếu không có bất kỳ khối nào còn nước đi
        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        Debug.LogError("GAME OVER! Reset màn chơi.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator CheckAfterFrame()
    {
        Debug.Log("[CheckAfterFrame] Bắt đầu kiểm tra khay gạch...");
        // Chờ hết khung hình hiện tại để đảm bảo việc Destroy gạch cũ hoàn tất
        yield return new WaitForEndOfFrame();

        int activeBlocksCount = 0;

        // Kiểm tra xem các điểm spawnPoint còn gạch hoạt động thực sự hay không
        foreach (Transform point in spawnPoints)
        {
            if (point != null && point.childCount > 0)
            {
                foreach (Transform child in point)
                {
                    if (child != null && child.gameObject.activeInHierarchy)
                    {
                        activeBlocksCount++;
                    }
                }
            }
        }

        Debug.Log($"[CheckAfterFrame] Số gạch hoạt động đếm được: {activeBlocksCount}");

        // Nếu khay chờ đã trống hoàn toàn
        if (activeBlocksCount == 0)
        {
            Debug.Log("[CheckAfterFrame] Đã đặt hết 3 hình. Đang tạm khóa kiểm tra để sinh lượt mới...");
            // 1. Khóa tính năng check GameOver lại ngay lập tức
            isSpawningNewRound = true;

            // 2. Tiến hành sinh 3 gạch mới
            SpawnAllBlocks();

            // 3. Chờ tiếp một khoảng thời gian đủ lâu (khoảng 0.1 giây) để Unity đồng bộ hóa
            yield return new WaitForSeconds(0.1f);

            // 4. Mở khóa khi mọi thứ đã sẵn sàng hoàn toàn
            isSpawningNewRound = false;
            Debug.Log("[CheckAfterFrame] Đã mở khóa kiểm tra.");
        }

        // 5. Lúc này mới tiến hành kiểm tra Game Over an toàn tuyệt đối
        CheckGameOver();
    }
}