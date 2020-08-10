using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ChunkData
{
    // 청크의 글로벌 포지션(Vector2Int)로 청크를 불러올 것
    // Vector2Int는 저장이 안 되는 형식이므로 Int형으로 저장할 것
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

    [HideInInspector]
    public VoxelState[,,] map = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

     
}
