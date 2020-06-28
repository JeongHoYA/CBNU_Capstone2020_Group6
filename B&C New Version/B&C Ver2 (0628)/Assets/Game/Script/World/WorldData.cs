using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public string worldName;                                                                        // 맵의 이름

    [System.NonSerialized]
    public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();
    // Vector2Int 형의 키와 ChunkData형의 데이터를 갖는 딕셔너리

    [System.NonSerialized]
    public List<ChunkData> modifiedChunks = new List<ChunkData>();
    // 변경된 청크들의 리스트



    


    public WorldData (string _worldName)
    {
        worldName = _worldName;
    }                                                         // 생성자

    public WorldData (WorldData wd)
    {
        worldName = wd.worldName;
    }                                                              // 생성자


    /* 청크 관련 함수 */
    public void SetVoxel(Vector3 pos, int value)
    {
        if (!IsVoxelInWorld(pos))
            return;
        // 복셀이 월드 내에 존재하지 않을 시 무효

        int x = Mathf.FloorToInt(pos.x / Chunk.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / Chunk.chunkWidth);
        x *= Chunk.chunkWidth;
        z *= Chunk.chunkWidth;
        // 청크의 월드 좌표(Vector2Int) 생성

        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);
        // 청크 생성

        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

        chunk.map[voxel.x, voxel.y, voxel.z] = value;
        AddToModifiedChunkList(chunk);
        // 청크 내 복셀 설정, 변경된 청크 리스트에 삽입
    }                                                 // 복셀 설정


    public int GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos))
            return 0;
        // 복셀이 월드 내에 존재하지 않을 시 0 반환

        int x = Mathf.FloorToInt(pos.x / Chunk.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / Chunk.chunkWidth);
        x *= Chunk.chunkWidth;
        z *= Chunk.chunkWidth;
        // 청크의 월드 좌표(Vector2Int) 생성

        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);
        // 청크 확인

        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

        return chunk.map[voxel.x, voxel.y, voxel.z];
        // 청크 내 복셀 반환
    }                                                             // 청크 내 pos에 위치한 Voxel 반환


    public ChunkData RequestChunk(Vector2Int coord, bool create)
    {
        ChunkData c;

        if (chunks.ContainsKey(coord))
            c = chunks[coord];
        else if (!create)
            c = null;
        else
        {
            LoadChunk(coord);
            c = chunks[coord];
        }

        return c;
    }                                 // Vector2Int형의 월드위치에 존재하는 ChunkData를 반환하는 함수
    

    public void LoadChunk(Vector2Int coord)
    {
        if (chunks.ContainsKey(coord))
            return;

        ChunkData chunk = SaveSystem.LoadChunk(worldName, coord);
        if(chunk != null)
        {
            chunks.Add(coord, chunk);
            return;
        }

        chunks.Add(coord, new ChunkData(coord));
        chunks[coord].Populate();
    }                                                      // Vector2Int형의 월드 위치에 청크를 생성하는 함수


    public void AddToModifiedChunkList(ChunkData chunk)
    {
        if (!modifiedChunks.Contains(chunk))
            modifiedChunks.Add(chunk);
    }                                          // modifiedChunkList에 청크 삽입 함수




    /* 좌표or데이터 변환 및 값 설정-반환 관련 함수 모음 */

    public bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < Chunk.worldSizeInBlocks && pos.y >= 0 && pos.y < Chunk.chunkHeight && pos.z >= 0 && pos.z < Chunk.worldSizeInBlocks)
            return true;
        else
            return false;
    }
}
