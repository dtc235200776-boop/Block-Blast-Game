using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//LUU HINH DANG CUA BLOCK
public class BlockData
{
    //Ma tran mo ta hinh dang
    public bool[,] Shape { get; private set; }
    public int gemIndex;
    //So hang
    public int Rows => Shape.GetLength(0);
    //So cot
    public int Columns => Shape.GetLength(1);
    public BlockData(bool [,] shape, int gemIndex)
    {
        Shape = shape;
        this.gemIndex = gemIndex;
    }
}
