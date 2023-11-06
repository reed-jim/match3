using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private int _index;
    private int _xIndex;
    private int _yIndex;
    private Vector3 _position;

    private TileType _tileType;
    private BlockType _blockType;
    private int _blockIndex;
    private TilePriority _priority;

    public int Index
    {
        get => _index; set => _index = value;
    }

    public int XIndex
    {
        get => _xIndex; set => _xIndex = value;
    }

    public int YIndex
    {
        get => _yIndex; set => _yIndex = value;
    }

    public Vector2 Position
    {
        get => _position; set => _position = value;
    }

    public TileType TileType
    {
        get => _tileType; set => _tileType = value;
    }

    public BlockType BlockType
    {
        get => _blockType; set => _blockType = value;
    }

    public int BlockIndex
    {
        get => _blockIndex; set => _blockIndex = value;
    }

    public TilePriority Priority
    {
        get => _priority; set => _priority = value;
    }
}
