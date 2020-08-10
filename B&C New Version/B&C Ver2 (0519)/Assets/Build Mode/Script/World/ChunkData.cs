using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    // 청크를 글로벌 좌표(Vector2Int)로 접근할 예정
    // Vector2Int형식은 저장이 안되므로 Int형 2가지를 합쳐 저장할 예정
    int x;                                                                                  // 청크의 글로벌 좌표.x
    int y;                                                                                  // 청크의 글로벌 좌표.y

    [HideInInspector]
    public int[,,] map = new int[Chunk.chunkWidth, Chunk.chunkHeight, Chunk.chunkWidth];    // 청크 내 복셀맵



    public ChunkData(Vector2Int pos) { position = pos; }                                    // 생성자
    public ChunkData(int _x, int _y) { x = _x; y = _y; }                                    // 생성자


    public Vector2Int position
    {
        get { return new Vector2Int(x, y); }
        set
        {
            x = value.x;
            y = value.y;
        }
    }


    public void Populate()
    {
        for (int y = 0; y < Chunk.chunkHeight; y++)
        {
            for (int x = 0; x < Chunk.chunkWidth; x++)
            {
                for (int z = 0; z < Chunk.chunkWidth; z++)
                {
                    map[x, y, z] = World.Instance.GetVoxel(new Vector3Int(x + position.x, y, z + position.y));
                }
            }
        }
        World.Instance.worldData.AddToModifiedChunkList(this);
    }                                                               // World.Getvoxel을 이용해 voxelMap에 복셀을 저장
}
