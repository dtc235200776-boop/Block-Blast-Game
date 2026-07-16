using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                cellObject.transform.localScale = Vector3.one * CellSize;

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

    // ĐÃ SỬA: Thực hiện đặt Block xuống và gán chỉ số đá quý lên bàn cờ
    public void PlaceBlock(BlockData blockData, int startRow, int startCol)
    {
        int gemIndex = blockData.gemIndex;

        for (int r = 0; r < blockData.Rows; r++)
        {
            for (int c = 0; c < blockData.Columns; c++)
            {
                if (blockData.Shape[r, c])
                {
                    int targetRow = startRow - r;
                    int targetCol = startCol + c;

                    // Đánh dấu ô bị chiếm và đổi Sprite đá quý theo gemIndex
                    board[targetRow, targetCol].SetOccupied(true, gemIndex);
                }
            }
        }
        CheckAndClearLines();
    }

    // ĐÃ SỬA: Hàm dọn dẹp hàng/cột đầy sẽ truyền vào -1 khi dọn ô trống
    private void CheckAndClearLines()
    {
        List<int> fullRows = new List<int>();
        List<int> fullCols = new List<int>();

        // 1. Kiểm tra các hàng đầy
        for (int r = 0; r < rows; r++)
        {
            bool isRowFull = true;
            for (int c = 0; c < columns; c++)
            {
                if (!board[r, c].IsOccupied)
                {
                    isRowFull = false;
                    break;
                }
            }
            if (isRowFull) fullRows.Add(r);
        }

        // 2. Kiểm tra các cột đầy (Rất quan trọng trong Block Blast)
        for (int c = 0; c < columns; c++)
        {
            bool isColFull = true;
            for (int r = 0; r < rows; r++)
            {
                if (!board[r, c].IsOccupied)
                {
                    isColFull = false;
                    break;
                }
            }
            if (isColFull) fullCols.Add(c);
        }

        // 3. Tiến hành dọn dẹp (Clear) các hàng và cột đầy
        int totalCleared = fullRows.Count + fullCols.Count;
        if (totalCleared > 0)
        {
            // Reset trạng thái các ô thuộc hàng bị đầy (truyền -1 để dọn dẹp đá quý)
            foreach (int r in fullRows)
            {
                for (int c = 0; c < columns; c++)
                {
                    board[r, c].SetOccupied(false, -1);
                }
            }

            // Reset trạng thái các ô thuộc cột bị đầy (truyền -1 để dọn dẹp đá quý)
            foreach (int c in fullCols)
            {
                for (int r = 0; r < rows; r++)
                {
                    board[r, c].SetOccupied(false, -1);
                }
            }

            // Cộng điểm số dựa trên số hàng/cột ăn được
            int pointsEarned = totalCleared * 100 * (totalCleared);
            ScoreManager.Instance?.AddScore(pointsEarned);
        }
    }

    // Kiểm tra xem một khối gạch cụ thể có thể đặt vào bất kỳ vị trí nào trên bàn cờ không
    public bool HasAnyValidMove(BlockData blockData)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // Thử ướm khối gạch vào tọa độ (r, c) này
                if (CanPlaceBlock(blockData, r, c))
                {
                    return true; // Chỉ cần có ít nhất 1 chỗ trống vừa vặn
                }
            }
        }
        return false; // Không tìm thấy bất kỳ chỗ nào chứa vừa khối gạch này
    }
}