using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Sinh Block tại tất cả các điểm Spawn khi game bắt đầu
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnBlock(spawnPoints[i]);
        }
    }

    // Sinh một Block tại vị trí chỉ định
    private void SpawnBlock(Transform spawnPoint)
    {
        if (boardManager == null)
        {
            Debug.LogError("Chưa kéo thả BoardManager vào BlockSpawner trong Inspector!");
            return;
        }

        Debug.Log(spawnPoint.name + " : " + spawnPoint.position);

        // Sinh Block và đặt trực tiếp làm con của spawnPoint để quản lý vị trí
        GameObject blockObject = Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

        // Đưa block về chính giữa spawnPoint
        blockObject.transform.localPosition = Vector3.zero;

        // Lấy script Block
        Block block = blockObject.GetComponent<Block>();

        // Chọn ngẫu nhiên một kiểu dáng Block trong thư viện
        int randomIndex = Random.Range(0, blockList.Count);
        BlockData data = blockList[randomIndex];

        // Lấy CellSize thực tế từ boardManager (đã được Awake tính toán xong) truyền vào
        block.Initialize(data, boardManager.CellSize);

        // Thu nhỏ khối block lại bằng 60% kích thước chuẩn khi nằm ở khay chờ cho đẹp mắt
        block.transform.localScale = Vector3.one * 0.6f;
    }
}