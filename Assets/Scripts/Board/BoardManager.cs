using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private Cell[,] board;

    [Header("Board setting")]
    [SerializeField] private int rows = 8;
    [SerializeField] private int columns = 8;
    [SerializeField] private float boardPadding = 0.5f;

    [Header("Cell")]
    [SerializeField] GameObject cellPrefab;

    public float CellSize { get; private set; }

    private void Awake()
    {
        board = new Cell[rows, columns];
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCamera.aspect;

        CellSize = (screenWidth - boardPadding * 2) / columns;

        float startX = -(columns * CellSize) / 2f + CellSize / 2f;
        float startY = -(rows * CellSize) / 2f + CellSize / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(startX + col * CellSize, startY + row * CellSize, 0);
                GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cellObject.transform.localScale = Vector3.one * CellSize * 0.95f;

                Cell cell = cellObject.GetComponent<Cell>();
                cell.Initialize(row, col);
                board[row, col] = cell;
            }
        }
    }

    // Tìm ô Cell gần nhất với vị trí thả của tâm Block
    public Cell GetNearestCell(Vector3 worldPosition)
    {
        Cell nearestCell = null;
        float minDistance = float.MaxValue;

        foreach (Cell cell in board)
        {
            float distance = Vector3.Distance(worldPosition, cell.transform.position);
            // Nếu khoảng cách nhỏ hơn một nửa kích thước ô cờ thì mới nhận diện
            if (distance < CellSize * 0.8f && distance < minDistance)
            {
                minDistance = distance;
                nearestCell = cell;
            }
        }
        return nearestCell;
    }

    // Kiểm tra xem Block có thể đặt vừa vào vị trí ô cờ đích hay không
    public bool CanPlaceBlock(BlockData blockData, int startRow, int startCol)
    {
        for (int r = 0; r < blockData.Rows; r++)
        {
            for (int c = 0; c < blockData.Columns; c++)
            {
                if (blockData.Shape[r, c])
                {
                    // Tính toán tọa độ ô cờ đích trên Board tương ứng với ma trận Block
                    int targetRow = startRow - r;
                    int targetCol = startCol + c;

                    // Nếu vượt ngoài biên bàn cờ
                    if (targetRow < 0 || targetRow >= rows || targetCol < 0 || targetCol >= columns)
                    {
                        return false;
                    }

                    // Nếu ô đó đã bị một khối khác chiếm chỗ trước đó
                    if (board[targetRow, targetCol].IsOccupied)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    // Thực hiện đặt Block xuống và nhuộm màu bàn cờ
    public void PlaceBlock(BlockData blockData, int startRow, int startCol)
    {
        // Chọn một màu ngẫu nhiên cho khối khi đặt xuống bàn cờ (hoặc dùng màu cố định tùy bạn)
        Color blockColor = new Color(0.12f, 0.73f, 0.91f); // Màu xanh Cyan nổi bật

        for (int r = 0; r < blockData.Rows; r++)
        {
            for (int c = 0; c < blockData.Columns; c++)
            {
                if (blockData.Shape[r, c])
                {
                    int targetRow = startRow - r;
                    int targetCol = startCol + c;

                    // Đánh dấu ô bị chiếm và đổi màu
                    board[targetRow, targetCol].SetOccupied(true, blockColor);
                }
            }
        }
    }
}