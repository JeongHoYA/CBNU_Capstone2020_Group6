﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public string worldName = "Prototype";
    public int seed;

    [System.NonSerialized]
    public Dictionary<Vector2, ChunkData> chunks = new Dictionary<Vector2, ChunkData>();

    [System.NonSerialized]
    public List<ChunkData> modifiedChunks = new List<ChunkData>();

    public void AddToModifiedChunkList (ChunkData chunk)
    {
        if (!modifiedChunks.Contains(chunk))
            modifiedChunks.Add(chunk);
    }

    public WorldData (string _worldName, int _seed)
    {
        worldName = _worldName;
        seed = _seed;
    }

    public WorldData (WorldData wD)
    {
        worldName = wD.worldName;
        seed = wD.seed;
    }

    public ChunkData RequestChunk(Vector2Int coord, bool create)
    {
        ChunkData c;

        lock (World.Instance.ChunkListThreadLock)
        {
            if (chunks.ContainsKey(coord))
                c = chunks[coord];
            else if (!create)
                c = null;
            else
            {
                LoadChunk(coord);
                c = chunks[coord];
            }
        }
        return c;
    }

    public void LoadChunk (Vector2Int coord)
    {
        if (chunks.ContainsKey(coord))
            return;

        ChunkData chunk = SaveSystem.LoadChunk(worldName, coord);
        if (chunk != null)
        {
            chunks.Add(coord, chunk);
            return;
        }

        chunks.Add(coord, new ChunkData(coord));
        chunks[coord].Populate();
    }

    bool IsVoxleInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeigth && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;
    }

    public void SetVoxel (Vector3 pos, byte value)
    {
        // If the voxel is outside of the world we don't need to do anything with it
        if (!IsVoxleInWorld(pos))
            return;

        // Find out the ChunkCoord value of our voxel's chunk.
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        // Then reverse that to get the position of the chunk.
        x *= VoxelData.ChunkWidth;
        z *= VoxelData.ChunkWidth;

        // Check if the chunk exists. if nt, create it.
        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

        // Then create a Vector3Int with the position of our voxel *within* the chunk.
        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

        // Then set the voxel in our chunk.
        chunk.map[voxel.x, voxel.y, voxel.z].id = value;
        AddToModifiedChunkList(chunk);
    }

    public VoxelState GetVoxel (Vector3 pos)
    {
        // If the voxel is outside of the world we don't need to do anything with it
        if (!IsVoxleInWorld(pos))
            return null;

        // Find out the ChunkCoord value of our voxel's chunk.
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        // Then reverse that to get the position of the chunk.
        x *= VoxelData.ChunkWidth;
        z *= VoxelData.ChunkWidth;

        // Check if the chunk exists. if nt, create it.
        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

        // Then create a Vector3Int with the position of our voxel *within* the chunk.
        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

        // Then set the voxel in our chunk.
        return chunk.map[voxel.x, voxel.y, voxel.z];
    }
}
