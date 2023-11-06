using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchBlocksCollectionType
{
    Row,
    Column,
    Cross
}

public class MatchBlocksCollection
{
    private MatchBlocksCollectionType _type;
    private List<int> _tileIndexes;
    private int _mostPriorityTileIndex;

    public MatchBlocksCollectionType Type
    {
        get => _type; set => _type = value;
    }

    public List<int> TileIndexes
    {
        get => _tileIndexes; set => _tileIndexes = value;
    }

    public int MostPriorityTileIndex
    {
        get => _mostPriorityTileIndex; set => _mostPriorityTileIndex = value;
    }
}
