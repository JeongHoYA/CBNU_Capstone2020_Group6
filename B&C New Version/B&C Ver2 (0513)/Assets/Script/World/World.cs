using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class World : MonoBehaviour
{
    /* 오브젝트 및 트랜스폼 모음 */
    public Transform player;                                                                // 플레이어 트랜스폼
    public Vector3 spawnPosition;                                                           // 플레이어 스폰 위치

    public GameObject debugScreen;                                                          // 디버그스크린 오브젝트
    public GameObject inventory;                                                            // 인벤토리 오브젝트
    public GameObject cursorSlot;                                                           // 인벤토리 커서 오브젝트

    public BlockType[] blocks;                                                              // 복셀 타입



    /* 청크 생성 관련 변수들 */
    public Chunk[,] chunks = new Chunk[Chunk.worldSizeInChunks, Chunk.worldSizeInChunks];
    // WorldSizeInChunks 개수만큼 청크를 저장하는 리스트
    // 위 리스트에 저장된 청크는 Chunk 클래스를 거쳐 월드에 생성됨

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();                               // 생성시킬 청크들의 리스트
    private bool isCreatingChunks;                                                          // 현재 청크를 생성시키고 있는지 여부             

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();                                 // isActive한 청크들 리스트
    public ChunkCoord playerChunkCoord;                                                     // 플레이어가 현재 위치한 청크의 ChunkCoord 좌표
    ChunkCoord playerLastChunkCoord;                                                        // 플레이어가 직전에 위치했던 청크의 ChunkCoord 좌표


    /* 기타 변수들 */
    private bool _inUI = false;                                                             // 플레이어의 inUI상태 여부




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
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();

        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");

        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);
    }





    /* 월드를 다루는 함수 모음 */

    private void GenerateWorld()
    {  
        for (int x = Chunk.worldSizeInChunks / 2 - Chunk.viewDistanceInChunks; x < Chunk.worldSizeInChunks / 2 + Chunk.viewDistanceInChunks; x++)
        {
            for (int z = Chunk.worldSizeInChunks / 2 - Chunk.viewDistanceInChunks; z < Chunk.worldSizeInChunks / 2 + Chunk.viewDistanceInChunks ; z++)
            {
                if(IsChunkInWorld(x, z))
                {
                    chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                    activeChunks.Add(new ChunkCoord(x, z));
                }
            }
        }
        spawnPosition = new Vector3(Chunk.worldSizeInBlocks / 2, Chunk.chunkHeight + 2, Chunk.worldSizeInBlocks / 2);
        player.position = spawnPosition;
    }                                                         // 초기 위치에 청크를 생성하고 플레이어 포지션을 설정해주는 함수


    IEnumerator CreateChunks()
    {
        isCreatingChunks = true;

        while(chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }
        isCreatingChunks = false;
    }                                                           // 청크를 생성시켜주는 코루틴 함수


    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        activeChunks.Clear();

        for (int x = coord.x - Chunk.viewDistanceInChunks; x < coord.x + Chunk.viewDistanceInChunks; x++)
        {
            for (int z = coord.z - Chunk.viewDistanceInChunks; z < coord.z + Chunk.viewDistanceInChunks; z++)
            {
                
                if (IsChunkInWorld(x, z))
                {
                    ChunkCoord thisChunk = new ChunkCoord(x, z);

                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
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
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
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
            GameObject voxel = Instantiate(blocks[id].block, posinChunk + chunkPosition, Quaternion.identity, chunk.transform);
        }
    }
    // posinChunk + chunkPosition에 코드번호 id의 복셀을 chunk를 부모로 하여 생성시켜주는 함수

    






    /* 좌표or데이터 변환 및 값 설정-반환 관련 함수 모음 */

    public bool inUI
    {
        get { return _inUI; }
        set
        {
            _inUI = value;

            if (_inUI)
            {
                inventory.SetActive(true);
                cursorSlot.SetActive(true);
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
                
            else
            {
                inventory.SetActive(false);
                cursorSlot.SetActive(false);
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }   
        }
    }                                                                     // inUI 설정

    bool IsChunkInWorld(int x, int z)
    {
        if (x >= 0 && x < Chunk.worldSizeInChunks && z >= 0 && z < Chunk.worldSizeInChunks)
            return true;
        else
            return false;
    }                                                    // Chunkcoord좌표가 월드좌표 안에 존재할 시 true 반환

    public bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < Chunk.worldSizeInBlocks && pos.y >= 0 && pos.y < Chunk.chunkHeight && pos.z >= 0 && pos.z < Chunk.worldSizeInBlocks)
            return true;
        else
            return false;
    }                                              // Vector3좌표가 월드좌표 안에 존재할 시 true 반환

    public ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / Chunk.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / Chunk.chunkWidth);
        return new ChunkCoord(x, z);
    }                              // 월드좌표 pos를 입력받아 ChunkCoord좌표를 반환

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / Chunk.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / Chunk.chunkWidth);
        return chunks[x, z];
    }                                        // 월드좌표 pos에 위치한 Chunk 반환

    public byte CheckVoxel (Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsVoxelInWorld(pos))
            return 0;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return blocks[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].blockType;

        return blocks[GetVoxel(Vector3Int.FloorToInt(pos))].blockType;
    }                                                 // 월드좌표 pos 위치의 블록의 타입을 반환하는 함수
}


[System.Serializable]
public class BlockType
{
    public string blockName;                                                                // 블록의 이름
    public byte blockType;                                                                  // 블록의 타입 (0 = 공기 ...)
    public Sprite icon;                                                                     // 블록 아이콘
    public GameObject block;                                                                // 블록 오브젝트
}