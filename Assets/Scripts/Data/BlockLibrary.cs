using UnityEngine;

public static class BlockLibrary
{
    // 1. Khối đơn lẻ 1x1
    public static BlockData CreateDotBlock()
    {
        bool[,] shape = { { true } };
        return new BlockData(shape, Color.cyan);
    }

    // 2. Khối Square 2x2 (Đã có)
    public static BlockData CreateSquare2x2()
    {
        bool[,] shape = {
            { true, true },
            { true, true }
        };
        return new BlockData(shape, Color.green);
    }

    // 3. Khối Square lớn 3x3
    public static BlockData CreateSquare3x3()
    {
        bool[,] shape = {
            { true, true, true },
            { true, true, true },
            { true, true, true }
        };
        return new BlockData(shape, Color.red);
    }

    // 4. Các thanh thẳng (Line) đủ kích cỡ
    public static BlockData CreateLine2()
    {
        bool[,] shape = { { true, true } };
        return new BlockData(shape, Color.yellow);
    }

    public static BlockData CreateLine3()
    {
        bool[,] shape = { { true, true, true } };
        return new BlockData(shape, Color.white);
    }

    public static BlockData CreateLine4()
    {
        bool[,] shape = { { true, true, true, true } };
        return new BlockData(shape, new Color(1f, 0.41f, 0.38f));
    }

    public static BlockData CreateLineVertical3()
    {
        bool[,] shape = {
            { true },
            { true },
            { true }
        };
        return new BlockData(shape, Color.blue);
    }

    // 5. Khối chữ L và J (Kích cỡ 2x3 và 3x2)
    public static BlockData CreateLNormal()
    {
        bool[,] shape = {
            { true, false },
            { true, false },
            { true, true }
        };
        return new BlockData(shape, new Color(1f, 0.76f, 0.03f));
    }

    public static BlockData CreateLReverse()
    {
        bool[,] shape = {
            { false, true },
            { false, true },
            { true, true }
        };
        return new BlockData(shape, new Color(0.96f, 0.49f, 0.15f));
    }

    // 6. Khối chữ T
    public static BlockData CreateTBlock()
    {
        bool[,] shape = {
            { true, true, true },
            { false, true, false }
        };
        return new BlockData(shape, new Color(0.01f, 0.66f, 0.96f));
    }

    // 7. Khối chữ Z và S
    public static BlockData CreateZBlock()
    {
        bool[,] shape = {
            { true, true, false },
            { false, true, true }
        };
        return new BlockData(shape, new Color(0.12f, 0.73f, 0.84f));
    }

    public static BlockData CreateSBlock()
    {
        bool[,] shape = {
            { false, true, true },
            { true, true, false }
        };
        return new BlockData(shape, new Color(0.12f, 0.81f, 0.43f));
    }

    // 8. Khối góc nhỏ 2x2 (Hình chữ L mini)
    public static BlockData CreateMiniCorner()
    {
        bool[,] shape = {
            { true, true },
            { true, false }
        };
        return new BlockData(shape, new Color(1f, 0.43f, 0.54f));
    }
}