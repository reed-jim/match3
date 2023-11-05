using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelMap : MonoBehaviour
{
    public abstract int NumRow
    {
        get;
    }

    public abstract int NumCol
    {
        get;
    }

    public abstract TileType[] TileTypes
    {
        get;
    }

    public abstract BlockType[] BlockTypes
    {
        get;
    }

    protected static BlockType basic1 = BlockType.Basic1;
    protected static BlockType basic2 = BlockType.Basic2;
    protected static BlockType basic3 = BlockType.Basic3;
    protected static BlockType basic4 = BlockType.Basic4;
    protected static BlockType basic5 = BlockType.Basic5;

    protected TileType[] GetBasicMap()
    {
        TileType[] result = new TileType[NumRow * NumCol];

        Array.Fill(result, TileType.HaveBlock);

        return result;
    }
}