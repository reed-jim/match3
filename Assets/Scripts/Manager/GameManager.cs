using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using System.Linq;

public enum BlockType
{
    Basic1,
    Basic2,
    Basic3,
    Basic4,
    Basic5,
    None
}

public enum TileType
{
    Empty,
    None,
    HaveBlock,
    Obstacle
}

public enum TilePriority
{
    PlayerSwapChoice,
    JustSpawn,
    Last
}

public class GameManager : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject blockContainer;
    [SerializeField] private GameObject[] blocks;
    private SpriteRenderer[] _blockRenderers;
    private Collider2D[] _blockColliders;

    private int _numRow;
    private int _numCol;
    private int _numTile;

    [Header("TILE")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileContainer;
    [SerializeField] private GameObject[] tiles;
    private SpriteRenderer[] _tileRenderers;

    [Header("SPRITE")]
    [SerializeField] private Sprite[] blockSprites;

    [Header("MANAGEMENT")]
    private LevelMap _levelMap;
    private Tile[] _tileProperties;
    private List<MatchBlocksCollection> matchBlocksCollections = new List<MatchBlocksCollection>();

    [Header("REFERENCE")]
    [SerializeField] private BoardGenerator boardGenerator;

    [Header("DEBUG")]
    [SerializeField] private bool isDebug;

    public GameObject[] Blocks
    {
        get => blocks;
    }

    public Collider2D[] BlockColliders
    {
        get => _blockColliders;
    }

    public int NumBlock
    {
        get => _numTile;
    }
    #endregion

    public void Init()
    {
        _levelMap = GameObject.Find("Level 1").GetComponent<LevelMap>();

        _numRow = _levelMap.NumRow;
        _numCol = _levelMap.NumCol;
        _numTile = _numRow * _numCol;

        blocks = new GameObject[_numTile];
        _blockRenderers = new SpriteRenderer[_numTile];
        _blockColliders = new Collider2D[_numTile];
        tiles = new GameObject[_numTile];
        _tileRenderers = new SpriteRenderer[_numTile];
        _tileProperties = new Tile[_numTile];

        for (int i = 0; i < _numTile; i++)
        {
            blocks[i] = Instantiate(blockPrefab, blockContainer.transform);
            _blockRenderers[i] = blocks[i].GetComponent<SpriteRenderer>();
            _blockColliders[i] = blocks[i].GetComponent<Collider2D>();

            tiles[i] = Instantiate(tilePrefab, tileContainer.transform);
            _tileRenderers[i] = tiles[i].GetComponent<SpriteRenderer>();
        }

        Vector2[] blockPositions = boardGenerator.CreateMap(_levelMap, _numRow, _numCol, _blockRenderers[0], out _tileProperties);

        for (int i = 0; i < _numTile; i++)
        {
            blocks[i].transform.position = blockPositions[i];
            _blockRenderers[i].sprite = blockSprites[(int)_tileProperties[i].BlockType];

            tiles[i].transform.position = new Vector3(blockPositions[i].x, blockPositions[i].y, tiles[i].transform.position.z);
        }

        for (int i = 0; i < _numRow; i++)
        {
            for (int j = 0; j < _numCol; j++)
            {
                int index = j + _numCol * i;

                if (i % 2 == 0)
                {
                    if (j % 2 == 0) _tileRenderers[index].color = new Color(1, 1, 1, 5f / 255);
                    else _tileRenderers[index].color = new Color(1, 1, 1, 10f / 255);
                }
                else
                {
                    if (j % 2 == 0) _tileRenderers[index].color = new Color(1, 1, 1, 10f / 255);
                    else _tileRenderers[index].color = new Color(1, 1, 1, 5f / 255);
                }
            }
        }
    }

    public void Move(int blockIndex, SwipeDirection swipeDirection)
    {
        int anotherBlockIndex;

        int startTileIndex = _tileProperties.First(tileProperty => tileProperty.BlockIndex == blockIndex).Index;
        int endTileIndex;

        Vector3[] newPositionsAfterSwap = GetNewPositionsAfterSwap(startTileIndex, swipeDirection, out anotherBlockIndex, out endTileIndex);

        if (anotherBlockIndex == -1) return;

        BlockType startBlockType = _tileProperties[startTileIndex].BlockType;
        BlockType endBlockType = _tileProperties[endTileIndex].BlockType;

        _tileProperties[startTileIndex].Priority = TilePriority.PlayerSwapChoice;
        _tileProperties[endTileIndex].Priority = TilePriority.PlayerSwapChoice;

        Tween.Position(blocks[blockIndex].transform, newPositionsAfterSwap[0], duration: 0.4f)
            .OnComplete(() => OnBlockMoved(blockIndex, startBlockType, endTileIndex, isLast: true));
        Tween.Position(blocks[anotherBlockIndex].transform, newPositionsAfterSwap[1], duration: 0.4f)
            .OnComplete(() => OnBlockMoved(anotherBlockIndex, endBlockType, startTileIndex));
    }

    private Vector3[] GetNewPositionsAfterSwap(int tileIndex, SwipeDirection swipeDirection, out int anotherBlockIndex, out int endTileIndex)
    {
        anotherBlockIndex = -1;
        endTileIndex = -1;

        Vector3[] result = new Vector3[2];

        Tile tileProperty = _tileProperties[tileIndex];

        if (swipeDirection == SwipeDirection.Up)
        {
            if (tileProperty.YIndex + 1 < _numRow)
            {
                endTileIndex = tileIndex + _numCol;
                anotherBlockIndex = _tileProperties[endTileIndex].BlockIndex;
                result[0] = _tileProperties[endTileIndex].Position;
            }
        }
        else if (swipeDirection == SwipeDirection.Down)
        {
            if (tileProperty.YIndex - 1 >= 0)
            {
                endTileIndex = tileIndex - _numCol;
                anotherBlockIndex = _tileProperties[endTileIndex].BlockIndex;
                result[0] = _tileProperties[endTileIndex].Position;
            }
        }
        else if (swipeDirection == SwipeDirection.Right)
        {
            if (tileProperty.XIndex + 1 < _numCol)
            {
                endTileIndex = tileIndex + 1;
                anotherBlockIndex = _tileProperties[endTileIndex].BlockIndex;
                result[0] = _tileProperties[endTileIndex].Position;
            }
        }
        else if (swipeDirection == SwipeDirection.Left)
        {
            if (tileProperty.XIndex - 1 >= 0)
            {
                endTileIndex = tileIndex - 1;
                anotherBlockIndex = _tileProperties[endTileIndex].BlockIndex;
                result[0] = _tileProperties[endTileIndex].Position;
            }
        }

        result[1] = tileProperty.Position;

        return result;
    }

    private void OnBlockMoved(int blockIndex, BlockType blockType, int endTileIndex, bool isLast = false)
    {
        _tileProperties[endTileIndex].BlockType = blockType;
        _tileProperties[endTileIndex].BlockIndex = blockIndex;

        if (isLast) CheckMatchAll();
    }

    private void CheckMatchAll()
    {
        CheckMatchHorizontal();
        CheckMatchVertical();

        if (matchBlocksCollections.Count == 0)
        {
            EndTurn();
        }

        HandleMatchBlocks();
        if (isDebug)
        {
            for (int i = 0; i < matchBlocksCollections.Count; i++)
            {
                MatchBlocksCollection o = matchBlocksCollections[i];

                Debug.Log("matchBlocksCollections Type:" + o.Type);

                for (int j = 0; j < o.TileIndexes.Count; j++)
                {
                    Debug.Log("matchBlocksCollections Tile Index: " + o.TileIndexes[j]);
                }
            }
        }
    }

    private void HandleMatchBlocks()
    {
        int numMatchBlock;
        bool isLastCollection = false;

        for (int i = 0; i < matchBlocksCollections.Count; i++)
        {
            isLastCollection = i == matchBlocksCollections.Count - 1;

            MatchBlocksCollection o = matchBlocksCollections[i];

            numMatchBlock = o.TileIndexes.Count;

            if (numMatchBlock == 3)
            {
                Handle3MatchBlocks(o.TileIndexes.ToArray(), isLastCollection);
            }
            else if (numMatchBlock == 4)
            {
                SpawnRowRocket(o.TileIndexes.ToArray());
            }
        }
    }

    private void Handle3MatchBlocks(int[] tileIndexes, bool isLastMatchBlockCollection)
    {
        bool isLast;

        for (int i = 0; i < tileIndexes.Length; i++)
        {
            if (!isLastMatchBlockCollection) isLast = false;
            else isLast = i == tileIndexes.Length - 1;

            int tileIndex = tileIndexes[i];
            Tile tileProperty = _tileProperties[tileIndex];

            Tween.Scale(blocks[tileProperty.BlockIndex].transform, 0, duration: 0.4f)
            .OnComplete(() => OnBlockDestroyed(_tileProperties[tileIndex].BlockIndex, tileIndex, isLast));
        }
    }

    private void OnBlockDestroyed(int blockIndex, int tileIndex, bool isLast)
    {
        _tileProperties[tileIndex].TileType = TileType.Empty;
        _tileProperties[tileIndex].BlockType = BlockType.None;
        _tileProperties[tileIndex].BlockIndex = -1;

        blocks[blockIndex].SetActive(false);

        if (isLast) SpawnNewBlocks();
    }

    private void SpawnRowRocket(int[] tileIndexes)
    {
        int mostPriorityTileIndex = tileIndexes.OrderBy(index => _tileProperties[index].Priority).First();
        int tileIndex;
        Tile tileProperty;

        for (int i = 0; i < tileIndexes.Length; i++)
        {
            tileIndex = tileIndexes[i];
            tileProperty = _tileProperties[tileIndex];
            int blockIndex = tileProperty.BlockIndex;

            if (blockIndex != _tileProperties[mostPriorityTileIndex].BlockIndex)
            {
                Tween.Position(blocks[blockIndex].transform, _tileProperties[mostPriorityTileIndex].Position, duration: 0.4f);
                Tween.Scale(blocks[blockIndex].transform, 0, duration: 0.4f);
            }
        }
    }

    private void CheckMatchHorizontal()
    {
        int numSameBlock = 0;
        BlockType currentCheckBlockType = BlockType.None;

        for (int i = 0; i < _numRow; i++)
        {
            numSameBlock = 0;

            for (int j = 0; j < _numCol; j++)
            {
                int index = j + _numCol * i;

                if (numSameBlock == 0)
                {
                    currentCheckBlockType = _tileProperties[index].BlockType;
                }

                if (_tileProperties[index].BlockType == currentCheckBlockType)
                {
                    numSameBlock++;

                    if (j == _numCol - 1 && numSameBlock >= 3)
                    {
                        AddMatchBlocksCollectionRow(i, j, numSameBlock);
                    }
                }
                else
                {
                    if (numSameBlock >= 3)
                    {
                        AddMatchBlocksCollectionRow(i, j - 1, numSameBlock);
                    }
                    else
                    {
                        numSameBlock = 0;

                        if (j > _numCol - 3)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void CheckMatchVertical()
    {
        int numSameBlock = 0;
        BlockType currentCheckBlockType = BlockType.None;

        for (int i = 0; i < _numCol; i++)
        {
            numSameBlock = 0;

            for (int j = 0; j < _numRow; j++)
            {
                int index = i + _numCol * j;

                if (numSameBlock == 0)
                {
                    currentCheckBlockType = _tileProperties[index].BlockType;
                }

                if (_tileProperties[index].BlockType == currentCheckBlockType)
                {
                    numSameBlock++;

                    if (j == _numRow - 1 && numSameBlock >= 3)
                    {
                        AddMatchBlocksCollectionColumn(i, j, numSameBlock);
                    }
                }
                else
                {
                    if (numSameBlock >= 3)
                    {
                        AddMatchBlocksCollectionColumn(i, j - 1, numSameBlock);
                    }
                    else
                    {
                        numSameBlock = 0;

                        if (j > _numRow - 3)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void AddMatchBlocksCollectionRow(int rowIndex, int endColumnIndex, int numSameBlock)
    {
        MatchBlocksCollection matchBlocksCollection = new MatchBlocksCollection();
        matchBlocksCollection.Type = MatchBlocksCollectionType.Row;
        matchBlocksCollection.TileIndexes = new List<int>();

        for (int i = endColumnIndex; i > endColumnIndex - numSameBlock; i--)
        {
            matchBlocksCollection.TileIndexes.Add(i + _numCol * rowIndex);
        }

        matchBlocksCollections.Add(matchBlocksCollection);
    }

    private void AddMatchBlocksCollectionColumn(int columnIndex, int endRowIndex, int numSameBlock)
    {
        MatchBlocksCollection matchBlocksCollection = new MatchBlocksCollection();
        matchBlocksCollection.Type = MatchBlocksCollectionType.Column;
        matchBlocksCollection.TileIndexes = new List<int>();

        for (int i = endRowIndex; i > endRowIndex - numSameBlock; i--)
        {
            matchBlocksCollection.TileIndexes.Add(endRowIndex + i * _numCol);
        }

        matchBlocksCollections.Add(matchBlocksCollection);
    }

    private void SpawnNewBlocks()
    {
        int currentCheckBlockIndex = blocks.Length - 1;
        int index;

        for (int i = 0; i < _numCol; i++)
        {
            for (int j = 0; j < _numRow; j++)
            {
                index = i + _numCol * j;

                if (_tileProperties[index].TileType == TileType.Empty)
                {
                    FindBlockToFillGap(i, j + 1);
                }
            }
        }

        void FindBlockToFillGap(int xIndex, int startYIndex)
        {
            int tileIndex;
            int destinationTileIndex = xIndex + (startYIndex - 1) * _numCol;

            for (int i = startYIndex; i < _numRow; i++)
            {
                tileIndex = xIndex + i * _numCol;
                // Debug.Log("test0: " + xIndex + "/" + i + "/"+)
                if (_tileProperties[tileIndex].TileType == TileType.HaveBlock)
                {
                    MoveBlock(_tileProperties[tileIndex].BlockIndex, destinationTileIndex);

                    _tileProperties[tileIndex].TileType = TileType.Empty;

                    return;
                }
            }

            if (currentCheckBlockIndex < 0) return;

            while (true)
            {
                if (!blocks[currentCheckBlockIndex].activeSelf)
                {
                    MoveNewSpawnBlock(currentCheckBlockIndex, destinationTileIndex);
                    currentCheckBlockIndex--;

                    break;
                }

                currentCheckBlockIndex--;

                if (currentCheckBlockIndex < 0) return;
            }
        }









        // List<ColumnGap> columnGaps = new List<ColumnGap>();

        // ColumnGap columnGap;
        // int index;

        // for (int i = 0; i < _numCol; i++)
        // {
        //     columnGap = new ColumnGap();
        //     columnGap.XIndex = i;

        //     for (int j = 0; j < _numRow; j++)
        //     {
        //         index = i + _numCol * j;

        //         if (_tileProperties[index].TileType == TileType.Empty)
        //         {
        //             if (columnGap.NumTile == 0) columnGap.StartYIndex = j;

        //             columnGap.NumTile++;
        //         }
        //     }

        //     if (columnGap.NumTile > 0) columnGaps.Add(columnGap);
        // }


        // int tileIndex;
        // int destinationTileIndex;
        // int numGapFill;

        // for (int i = 0; i < columnGaps.Count; i++)
        // {
        //     destinationTileIndex = columnGaps[i].StartYIndex;
        //     numGapFill = 0;

        //     for (int j = columnGaps[i].StartYIndex + 1; j < _numCol; j++)
        //     {
        //         tileIndex = columnGaps[i].XIndex + j * _numCol;

        //         if (_tileProperties[tileIndex].TileType == TileType.HaveBlock)
        //         {
        //             MoveNewSpawnBlock(_tileProperties[tileIndex].BlockIndex, destinationTileIndex);

        //             destinationTileIndex += _numCol;
        //             numGapFill++;
        //         }


        //         if (columnGaps[i].StartYIndex + j < _numCol)
        //         {
        //             tileIndex = columnGaps[i].XIndex + (columnGaps[i].StartYIndex + j) * _numCol;

        //             if (_tileProperties[tileIndex].TileType == TileType.HaveBlock)
        //             {
        //                 MoveNewSpawnBlock(_tileProperties[tileIndex].BlockIndex, destinationTileIndex);

        //                 destinationTileIndex += _numCol;
        //             }
        //         }
        //         else
        //         {
        //             while (true)
        //             {
        //                 if (!blocks[currentCheckBlockIndex].activeSelf)
        //                 {
        //                     MoveNewSpawnBlock(currentCheckBlockIndex, destinationTileIndex);

        //                     break;
        //                 }

        //                 currentCheckBlockIndex++;

        //                 if (currentCheckBlockIndex >= blocks.Length) return;
        //             }
        //         }
        //     }
        // }
    }

    private void MoveBlock(int blockIndex, int tileIndex)
    {
        Tween.PositionY(blocks[blockIndex].transform, _tileProperties[tileIndex].Position.y, duration: 0.5f).OnComplete(() => OnMoved());

        void OnMoved()
        {
            _tileProperties[tileIndex].TileType = TileType.HaveBlock;
            _tileProperties[tileIndex].BlockType = BlockType.Basic1;
            _tileProperties[tileIndex].BlockIndex = blockIndex;
        }
    }

    private void MoveNewSpawnBlock(int blockIndex, int tileIndex)
    {
        blocks[blockIndex].transform.position =
            new Vector3(_tileProperties[tileIndex].Position.x, _tileProperties.Last().Position.y + 6, 0);
        blocks[blockIndex].transform.localScale = new Vector3(0, 0, 1);
        blocks[blockIndex].SetActive(true);

        Tween.Scale(blocks[blockIndex].transform, 1, duration: 0.5f)
        .OnComplete(
            () => OnScaledUp()
        );

        void OnScaledUp()
        {
            Tween.PositionY(blocks[blockIndex].transform, _tileProperties[tileIndex].Position.y, duration: 0.5f).OnComplete(() => OnMoved());
        }

        void OnMoved()
        {
            _tileProperties[tileIndex].TileType = TileType.HaveBlock;
            _tileProperties[tileIndex].BlockType = BlockType.Basic1;
            _tileProperties[tileIndex].BlockIndex = blockIndex;
        }
    }

    private void EndTurn()
    {

    }
}

class SpawnBlock
{

}

class ColumnGap
{
    private int _xIndex;
    private int _startYIndex;
    private int _numTile;

    public int XIndex
    {
        get => _xIndex; set => _xIndex = value;
    }

    public int StartYIndex
    {
        get => _startYIndex; set => _startYIndex = value;
    }

    public int NumTile
    {
        get => _numTile; set => _numTile = value;
    }
}