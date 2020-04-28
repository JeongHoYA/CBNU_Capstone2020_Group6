using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttribute biome;

    public Transform player;                                            // 플레이어 위치
    public Vector3 spawnPosition;                                       // 플레이어 스폰 위치

    public Material material;                                           // 텍스처 아틀라스 머터리얼
    public BlockType[] blockTypes;                                      // 복셀 타입

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    // WorldSizeInChunks 개수만큼 청크를 저장하는 리스트
    // 위 리스트에 저장된 청크는 Chunk 클래스를 거쳐 월드에 생성됨

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();             // isActive한 청크들 리스트

    ChunkCoord playerChunkCoord;                                        // 플레이어가 현재 위치한 청크의 ChunkCoord 좌표
    ChunkCoord playerLastChunkCoord;                                    // 플레이어가 직전에 위치했던 청크의 ChunkCoord 좌표

    private void Start()
    {
        Random.InitState(seed);

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 3f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
        {
            CheckViewDistance();
            playerLastChunkCoord = playerChunkCoord;
        }   // 플레이어의 ChunkCoord좌표가 바뀌면 CheckViewDistance 실행
    }


    /* 월드 내 청크를 다루는 함수 모음 */

    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
        player.position = spawnPosition;
    }                                             // 게임을 시작할 때 특정 좌표에 CreateNewChunk(x, z)를 호출한 후 player.position을 설정하는 함수

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x- VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if (IsChunkInWorld(new ChunkCoord (x, z)))
                {
                    if (chunks[x, z] == null)
                        CreateNewChunk(x, z);
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
                }   // 청크가 존재하지 않으면 생성하고 존재하면 isActive를 True로 설정

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
                }
            }
        }
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }                                         // 플레이어 주변 ViewDistanceInChunks 반경 내 청크가 없을 시 CreateNewChunk(x, z)를 호출하는 함수

    public int GetVoxel (Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);

        /* 바닥과 공기 부분 */
        // 월드 외부 좌표에는 에어복셀 반환
        if (!IsVoxelInWorld(pos))
            return 0;
        // 바닥에는 흑요석 복셀 반환
        if (yPos == 0)
            return 1;

        /* 그 외 부분 */
        int terrainHeight = Mathf.FloorToInt(VoxelData.ChunkHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        int voxelValue = 0;

        if (yPos == terrainHeight)
            voxelValue = 6;
        else if (yPos < terrainHeight && yPos > terrainHeight - 3)
            voxelValue = 7;
        else if (yPos > terrainHeight)
            return 0;
        else
            voxelValue = 2;

        if (voxelValue == 2)
        {
            foreach (Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxheight)
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;
            }
        }

        return voxelValue;
    }                                // Vector3Int pos 위치에 복셀 코드를 제공하는 함수

    void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunks.Add(new ChunkCoord(x, z));
    }                                // [x, z]좌표에 새 청크를 생성시키고 activeChunk 리스트에 좌표 저장





    /* 좌표or데이터 변환 및 값 설성or반환 관련 함수 모음 */

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x >= 0 && coord.x < VoxelData.WorldSizeInChunks && coord.z >= 0 && coord.z < VoxelData.WorldSizeInChunks)
            return true;
        else
            return false;
    }                            // Chunkcoord좌표가 월드좌표 안에 존재하는 좌표이면 true 반환
    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;
    }                                 // 월드좌표 Vector3 pos의 voxel위치가 월드 내이면 true
    ChunkCoord GetChunkCoordFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }                // 월드좌표 Vector3 pos를 입력받아 ChunkCoord좌표를 반환하는 함수
    public bool CheckForVoxel(float _x, float _y, float _z)
    {
        int xCheck = Mathf.FloorToInt(_x);
        int yCheck = Mathf.FloorToInt(_y);
        int zCheck = Mathf.FloorToInt(_z);

        int xChunk = xCheck / VoxelData.ChunkWidth;
        int zChunk = zCheck / VoxelData.ChunkWidth;

        xCheck -= (xChunk * VoxelData.ChunkWidth);
        zCheck -= (zChunk * VoxelData.ChunkWidth);

        if (IsVoxelInWorld(new Vector3(_x, _y, _z)))
            return blockTypes[chunks[xChunk, zChunk].voxelMap[xCheck, yCheck, zCheck]].isSolid;
        else
            return false;
    }          // 월드좌표 [x, y, z]를 입력받아 해당 chunk 내 voxelmap의 isSolid 여부 반환
    
}


/* 복셀의 타입을 정하는 클래스 */
[System.Serializable]
public class BlockType
{
    public string blockName;                            // 복셀의 이름
    public bool isAir;                                  // 에어블록 여부
    public bool isSolid;                                // 블록의 통과 불가능 여부

    [Header("Texture Values")]                          // 복셀에 사용되는 텍스처
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    public int GetTextureID (int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }         // 각 면에 따른 텍스처를 적용시키는 함수
}
