using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttribute biome;

    public Settings settings;                                           // 세팅 클래스

    public GameObject debugScreen;                                      // 디버그스크린 오브젝트
    public GameObject creativeInventory;                                // 인벤토리 오브젝트
    public GameObject cursorSlot;                                       // 아이템을 클릭하는 커서 슬롯 오브젝트

    public Transform player;                                            // 플레이어 위치
    public Vector3 spawnPosition;                                       // 플레이어 시작 시 스폰 위치

    public Material material;                                           // 텍스처 아틀라스 머터리얼
    public Material transparentMaterial;                                // 배경이 투명한 텍스처 아틀라스 머터리얼
    public BlockType[] blockTypes;                                      // 복셀 타입

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    // WorldSizeInChunks 개수만큼 청크를 저장하는 리스트
    // 위 리스트에 저장된 청크는 Chunk 클래스를 거쳐 월드에 생성됨

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();             // isActive한 청크들 리스트

    public ChunkCoord playerChunkCoord;                                 // 플레이어가 현재 위치한 청크의 ChunkCoord 좌표
    ChunkCoord playerLastChunkCoord;                                    // 플레이어가 직전에 위치했던 청크의 ChunkCoord 좌표

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();           // start, update도중 즉시 생성할 청크들을 모아놓은 리스트
    private bool isCreatingChunks;                                      // 현재 청크가 생성되고 있는지 여부

    private bool _inUI = false;                                         // 플레이어가 인벤토리를 열었는지 여부



    private void Awake()
    {
        Application.targetFrameRate = 60;
        // 게임 프레임 60으로 제한
    }

    private void Start()
    {
        //string jsonExport = JsonUtility.ToJson(settings);
        //Debug.Log(jsonExport);
        //File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);

        string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
        settings = JsonUtility.FromJson<Settings>(jsonImport);

        Random.InitState(seed);

        debugScreen.SetActive(false);
        creativeInventory.SetActive(false);
        cursorSlot.SetActive(false);

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 3f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);

        GenerateWorld();

        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();
        // 플레이어의 ChunkCoord좌표가 바뀌면 CheckViewDistance 실행

        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");
        // 생성시켜야 할 청크가 있으면 CreateChunks 코루틴 실행

        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);
        // F3키를 눌러 디버그 스크린 창 on/off
    }





    /* 월드 내 청크를 다루는 함수 모음 */

    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2) - settings.viewDistance; x < (VoxelData.WorldSizeInChunks / 2) + settings.viewDistance; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - settings.viewDistance; z < (VoxelData.WorldSizeInChunks / 2) + settings.viewDistance; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                    activeChunks.Add(new ChunkCoord(x, z));
                }
            }
        }
        player.position = spawnPosition;
    }                                             // 게임을 시작할 때 특정 좌표에 청크를 생성한 후 player.position을 설정하는 함수

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        activeChunks.Clear();

        for (int x = coord.x - settings.viewDistance; x < coord.x + settings.viewDistance; x++)
        {
            for (int z = coord.z - settings.viewDistance; z < coord.z + settings.viewDistance; z++)
            {
                if (IsChunkInWorld(new ChunkCoord (x, z)))
                {
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;    
                    }
                    activeChunks.Add(new ChunkCoord(x, z));
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
        /*
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
        }*/
        int voxelValue = 0;

        return voxelValue;
    }                                // 월드좌표 pos의 위치에 복셀 인덱스를 제공하는 함수 (월드를 만드는 함수)

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
    }                                       // 한 프레임에 한 청크만 생성시키게 해주는 코루틴





    /* 좌표or데이터 변환 및 값 설정-반환 관련 함수 모음 */

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x >= 0 && coord.x < VoxelData.WorldSizeInChunks && coord.z >= 0 && coord.z < VoxelData.WorldSizeInChunks)
            return true;
        else
            return false;
    }                            // Chunkcoord좌표가 월드좌표 안에 존재하는 좌표이면 true 반환
    public bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;
    }                          // 월드좌표 pos의 voxel위치가 월드 내이면 true

    ChunkCoord GetChunkCoordFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }                // 월드좌표 pos를 입력받아 ChunkCoord좌표를 반환
    public Chunk GetChunkFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        if (IsChunkInWorld(new ChunkCoord(x, z)))
            return chunks[x, z];
        else
            return null;
    }                   // 월드좌표 pos를 입력받아 해당 좌표의 청크를 반환

    public bool CheckForVoxelisAir(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return true;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isAir;

        return blockTypes[GetVoxel(pos)].isAir;
    }                      // 월드좌표 pos를 입력받아 해당 청크 내 복셀의 isAir 여부 반환
    public bool CheckForVoxelisSolid(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;

        return blockTypes[GetVoxel(pos)].isSolid;
    }                    // 월드좌표 pos를 입력받아 해당 청크 내 복셀의 isSolid 여부 반환
    public bool CheckForVoxelisTransparent(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return true;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isTransparent;

        return blockTypes[GetVoxel(pos)].isTransparent;
    }              // 월드좌표 pos를 입력받아 해당 청크 내 복셀의 isTransparent 여부 반환

    public bool inUI
    {
        get { return _inUI; }
        set
        {
            _inUI = value;
            if (_inUI)
            {
                Cursor.lockState = CursorLockMode.None;
                creativeInventory.SetActive(true);
                cursorSlot.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                creativeInventory.SetActive(false);
                cursorSlot.SetActive(false);
            }
        }
    }                                                 // 플레이어의 inUI상태 반환, 마우스 커서 on/off, 인벤토리 on/off

}


/* 복셀의 타입을 정하는 클래스 */
[System.Serializable]
public class BlockType
{
    public string blockName;                            // 복셀의 이름
    public Sprite icon;                                 // 복셀의 아이콘
    public bool isAir;                                  // 에어블록 여부
    public bool isSolid;                                // 블록의 통과 불가능 여부
    public bool isTransparent;                          // 블록의 투명 여부

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



[System.Serializable]
public class Settings
{
    [Range(1, 8)]
    public int viewDistance = 4;

    public int loadDistance = 16;

    [Range(0.1f, 8f)]
    public float mouseSensitivity = 3f;
}