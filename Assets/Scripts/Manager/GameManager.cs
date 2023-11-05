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

public class GameManager : MonoBehaviour
{
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

    [Header("SPRITE")]
    [SerializeField] private Sprite[] blockSprites;

    [Header("MANAGEMENT")]
    private LevelMap _levelMap;
    private Tile[] tileProperties;

    [Header("REFERENCE")]
    [SerializeField] private BoardGenerator boardGenerator;

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
  
        tileProperties = new Tile[_numTile];

        for (int i = 0; i < _numTile; i++)
        {
            blocks[i] = Instantiate(blockPrefab, blockContainer.transform);
            _blockRenderers[i] = blocks[i].GetComponent<SpriteRenderer>();
            _blockColliders[i] = blocks[i].GetComponent<Collider2D>();

            tiles[i] = Instantiate(tilePrefab, tileContainer.transform);
        }

        Vector2[] blockPositions = boardGenerator.CreateMap(_levelMap, _numRow, _numCol, _blockRenderers[0], out tileProperties);

        for (int i = 0; i < _numTile; i++)
        {
            blocks[i].transform.position = blockPositions[i];
            _blockRenderers[i].sprite = blockSprites[(int)tileProperties[i].BlockType];

            tiles[i].transform.position = new Vector3(blockPositions[i].x, blockPositions[i].y, tiles[i].transform.position.z);
        }
    }

    public void Move(int blockIndex, SwipeDirection swipeDirection)
    {
        int anotherBlockIndex;

        int tileIndex = tileProperties.First(tileProperty => tileProperty.BlockIndex == blockIndex).Index;

        Vector3[] newPositionsAfterSwap = GetNewPositionsAfterSwap(tileIndex, swipeDirection, out anotherBlockIndex);

        Tween.Position(blocks[blockIndex].transform, newPositionsAfterSwap[0], duration: 1f);
    }

    private Vector3[] GetNewPositionsAfterSwap(int tileIndex, SwipeDirection swipeDirection, out int anotherBlockIndex)
    {
        anotherBlockIndex = -1;

        Vector3[] result = new Vector3[2];

        Tile tileProperty = tileProperties[tileIndex];

        if (swipeDirection == SwipeDirection.Up)
        {
            if (tileProperty.YIndex + 1 < _numRow)
            {
                anotherBlockIndex = tileProperties[tileIndex + _numCol].BlockIndex;
                result[0] = tileProperties[tileIndex + _numCol].Position;
            }
        }
        else if (swipeDirection == SwipeDirection.Down)
        {
            if (tileProperty.YIndex - 1 >= 0)
            {
                anotherBlockIndex = tileProperties[tileIndex - _numCol].BlockIndex;
                result[0] = tileProperties[tileIndex - _numCol].Position;
            }
        }
        else if (swipeDirection == SwipeDirection.Right)
        {
            if (tileProperty.XIndex + 1 < _numCol)
            {
                anotherBlockIndex = tileProperties[tileIndex + 1].BlockIndex;
                result[0] = tileProperties[tileIndex + 1].Position;
            }
        }
        else if (swipeDirection == SwipeDirection.Left)
        {
            if (tileProperty.XIndex - 1 >= 0)
            {
                anotherBlockIndex = tileProperties[tileIndex - 1].BlockIndex;
                result[0] = tileProperties[tileIndex - 1].Position;
            }
        }

        result[1] = tileProperty.Position;

        return result;
    }
}
