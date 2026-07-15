using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quản lý một Block trong game
public class Block : MonoBehaviour
{
    // Dữ liệu hình dạng của Block
    private BlockData blockData;

    // Prefab của 1 ô nhỏ trong Block
    [SerializeField] private GameObject cellPrefab;

    // Kích thước của mỗi ô trong Block
    private float cellSize;

    // Khởi tạo Block với dữ liệu hình dáng và kích thước từ bàn cờ
    public void Initialize(BlockData data, float size)
    {
        blockData = data;
        cellSize = size;

        GenerateCells();
    }

    // Lấy dữ liệu từ BlockData
    public BlockData GetBlockData()
    {
        return blockData;
    }

    private void GenerateCells()
    {
        // Chiều rộng của Block
        float width = blockData.Columns * cellSize;

        // Chiều cao của Block
        float height = blockData.Rows * cellSize;

        // Vị trí bắt đầu để Block nằm giữa Transform
        float startX = -width / 2f + cellSize / 2f;
        float startY = height / 2f - cellSize / 2f;

        // Sinh các ô theo dữ liệu Block
        for (int row = 0; row < blockData.Rows; row++)
        {
            for (int col = 0; col < blockData.Columns; col++)
            {
                if (!blockData.Shape[row, col])
                {
                    continue;
                }
                GameObject cell = Instantiate(cellPrefab, transform);

                // Căn vị trí cục bộ theo cellSize đồng bộ
                cell.transform.localPosition = new Vector3(startX + col * cellSize, startY - row * cellSize, 0f);

                // --- SỬA LỖI SCALE BẰNG 0: Nếu scale quá bé hoặc lỗi, gán bằng 0.5f để phòng vệ ---
                float finalScale = cellSize * 0.95f;
                if (finalScale <= 0.01f)
                {
                    finalScale = 0.5f;
                }
                cell.transform.localScale = Vector3.one * finalScale;
            }
        }
    }
}