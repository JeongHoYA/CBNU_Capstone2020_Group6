using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform player;                                                                // 플레이어 트랜스폼
    public Vector3 spawnPosition;                                                           // 플레이어 스폰 위치

    public BlockType[] blocks;                                                              // 복셀 타입

    Chunk[,] chunks = new Chunk[Chunk.worldSizeInChunks, Chunk.worldSizeInChunks];
    // WorldSizeInChunks 개수만큼 청크를 저장하는 리스트
    // 위 리스트에 저장된 청크는 Chunk 클래스를 거쳐 월드에 생성됨

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();                                 // isActive한 청크들 리스트

    public ChunkCoord playerChunkCoord;                                                     // 플레이어가 현재 위치한 청크의 ChunkCoord 좌표
    ChunkCoord playerLastChunkCoord;                                                        // 플레이어가 직전에 위치했던 청크의 ChunkCoord 좌표


    private void Awake()
    {
        Application.targetFrameRate = 60;
        // 게임 프레임 60으로 제한
    }

    private void Start()
    {
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);
    }


    void Update()
    {
        if (!GetChunkCoordFromVector3(player.transform.position).Equals(playerLastChunkCoord))
            CheckViewDistance();
    }



    /* 월드를 다루는 함수 모음 */

    private void GenerateWorld()
    {
        
        for (int x = Chunk.worldSizeInChunks / 2 - Chunk.viewDistanceInChunks; x < Chunk.worldSizeInChunks / 2 + Chunk.viewDistanceInChunks; x++)
        {
            for (int z = Chunk.worldSizeInChunks / 2 - Chunk.viewDistanceInChunks; z < Chunk.worldSizeInChunks / 2 + Chunk.viewDistanceInChunks ; z++)
            {
                CreateChunk(new ChunkCoord(x, z));
            }
        }
        spawnPosition = new Vector3(Chunk.worldSizeInBlocks / 2, Chunk.chunkHeight + 2, Chunk.worldSizeInBlocks / 2);
        player.position = spawnPosition;
    }                                                         // 초기 위치에 청크를 생성하고 플레이어 포지션을 설정해주는 함수


    private void CreateChunk(ChunkCoord coord)
    {
        chunks[coord.x, coord.z] = new Chunk(coord, this);
        activeChunks.Add(coord);
    }                                           // 청크를 생성시키고 activeChunk 리스트에 삽입해주는 함수


    void CheckViewDistance()
    {

        int chunkX = Mathf.FloorToInt(player.position.x / Chunk.chunkWidth);
        int chunkZ = Mathf.FloorToInt(player.position.z / Chunk.chunkWidth);

        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        activeChunks.Clear();

        for (int x = chunkX - Chunk.viewDistanceInChunks; x < chunkX + Chunk.viewDistanceInChunks; x++)
        {
            for (int z = chunkZ - Chunk.viewDistanceInChunks; z < chunkZ + Chunk.viewDistanceInChunks; z++)
            {
                
                if (IsChunkInWorld(x, z))
                {
                    ChunkCoord thisChunk = new ChunkCoord(x, z);

                    if (chunks[x, z] == null)
                        CreateChunk(thisChunk);
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        
                    }
                    activeChunks.Add(thisChunk);

                    
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    { 
                        if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                            previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }
        foreach (ChunkCoord coord in previouslyActiveChunks)
            chunks[coord.x, coord.z].isActive = false;
    }                                                             // 플레이어 주변 ViewDistanceInChunks만큼 Chunks를 생성 혹은 active시켜주는 함수


    public int GetVoxel(Vector3Int pos)
    {
        int blocktype = 0;

        if (pos.x < 0 || pos.x > Chunk.worldSizeInBlocks - 1 || pos.y < 0 || pos.y > Chunk.chunkHeight - 1 || pos.z < 0 || pos.z > Chunk.worldSizeInBlocks - 1)
            return 0;
        if (pos.y == 0)
            return 1;

        else return blocktype;
    }                                                  // 월드좌표 pos의 위치에 복셀 코드를 제공하는 함수 (월드를 만드는 함수) 


    public void PlaceVoxel(int id, Vector3Int posinChunk, Vector3Int chunkPosition, Chunk chunk)
    {
        if (id != 0)
        {
            Instantiate(blocks[id].block, posinChunk + chunkPosition, Quaternion.identity, chunk.transform);
        }   
    }
    // posinChunk + chunkPosition에 코드번호 id의 복셀을 chunk를 부모로 하여 생성시켜주는 함수






    /* 좌표or데이터 변환 및 값 설정-반환 관련 함수 모음 */
    bool IsChunkInWorld(int x, int z)
    {
        if (x >= 0 && x < Chunk.worldSizeInChunks && z >= 0 && z < Chunk.worldSizeInChunks)
            return true;
        else
            return false;
    }                                                    // Chunkcoord좌표가 월드좌표 안에 존재하는 좌표이면 true 반환

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / Chunk.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / Chunk.chunkWidth);
        return new ChunkCoord(x, z);
    }                                     // 월드좌표 pos를 입력받아 ChunkCoord좌표를 반환
}


[System.Serializable]
public class BlockType
{
    public string blockName;
    public GameObject block;
}