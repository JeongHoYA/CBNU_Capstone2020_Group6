using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WorldData
{
    public string worldName = "Prototype";                              // 월드의 이름
    public int worldNum;                                                // 월드의 번호

    public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();
    // 위치와 청크데이터를 포함하는 딕셔너리

    public ChunkData RequestChunk (Vector2Int coord)
    {
        if (chunks.ContainsKey(coord))
            return chunks[coord];
        else
            return null;
    }                                                                // 월드 내 청크를 불러오는 함수


}
