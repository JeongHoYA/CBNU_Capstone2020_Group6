﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

[System.Serializable]
public class ChunkData
{
    // The global position of the chunk. We Want to be able tu access it as a Vector2Int,
    // but Vector2Int's are not serialized so we won't be able to save it.
    // So we'll store them as ints.

    int x;
    int y;

    public Vector2Int position
    {
        get { return new Vector2Int(x, y); }
        set
        {
            x = value.x;
            y = value.y;
        }
    }

    public ChunkData (Vector2Int pos) { position = pos; }
    public ChunkData (int _x, int _y) { x = _x; y = _y; }

    [HideInInspector] // Display lots of data in the inspector slows it down even more so hide this one 
    public VoxelState[,,] map = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeigth, VoxelData.ChunkWidth];

    public void Populate()
    {
        for (int y = 0; y < VoxelData.ChunkHeigth; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    map[x, y, z] = new VoxelState(World.Instance.GetVoxel(new Vector3(x + position.x, y, z + position.y)));
                }
            }
        }
        World.Instance.worldData.AddToModifiedChunkList(this);
    }

}
