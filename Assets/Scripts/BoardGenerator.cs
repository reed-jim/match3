using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public Vector2[] CreateMap(LevelMap levelMap, int numRow, int numCol, SpriteRenderer blockRenderer, out Tile[] tileProperties)
    {
        int numTile = numRow * numCol;

        tileProperties = new Tile[numTile];

        int index;
        Vector2 position;
        Vector2 blockSize = blockRenderer.bounds.size;
        Vector2 blockGap = 0.0f * blockRenderer.bounds.size;
        Vector2 distance = (Vector2)blockRenderer.bounds.size + blockGap;

        Vector2[] result = new Vector2[numTile];

        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                index = j + numCol * i;

                position.x = -((numCol - 1) / 2f) * distance.x + j * distance.x;
                position.y = -((numCol - 1) / 2f) * distance.x + i * distance.y;

                result[index] = position;

                tileProperties[index] = new Tile
                {
                    Index = index,
                    XIndex = j,
                    YIndex = i,
                    Position = position,
                    TileType = levelMap.TileTypes[index],
                    BlockType = levelMap.BlockTypes[index],
                    BlockIndex = index
                };
            }
        }

        return result;
    }
}
