using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Map : LevelMap
{
    public override int NumRow => 4;
    public override int NumCol => 4;

    public override TileType[] TileTypes { get => GetBasicMap(); }

    public override BlockType[] BlockTypes
    {
        get => GetBlockTypes();
    }

    private BlockType[] GetBlockTypes()
    {
        BlockType[] result = {
            basic1, basic2, basic3, basic4,
            basic5, basic1, basic1, basic2,
            basic5, basic1, basic1, basic2,
            basic5, basic1, basic1, basic2
        };

        return result;
    }
}
