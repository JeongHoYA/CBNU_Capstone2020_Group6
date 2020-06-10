using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using System.IO;
using System.Net;
using TMPro.Examples;

public class World : MonoBehaviour
{
    /* 게임 설정/저장 관련 변수/클래스 모음 */
    public WorldData worldData;                                                             // 월드 데이터 클래스

    public string mapName;                                                                  // 맵 이름
    public string mapFileLocation;                                                          // 맵 이름에 해당하는 파일 위치
    public bool isBuildMode;                                                                // 맵의 빌드모드 여부                                                        

    public Settings settings;                                                               // 세팅 클래스
    public bool isSettingsChanged = false;                                                  // 세팅이 바뀌었는지 여부(바뀔 시 세팅 재설정)

    public Material daySky;                                                                 // 낮 머터리얼        
    public Material nightSky;                                                               // 밤 머터리얼

    /* 오브젝트 및 트랜스폼 모음 */
    public Transform player;                                                                // 플레이어 트랜스폼
    Vector3 spawnPosition;                                                                  // 플레이어 스폰 위치
    public GameObject buildPlayer;                                                          // 빌드 플레이어 오브젝트
    public GameObject clearPlayer;                                                          // 클리어 플레이어 오브젝트

    public GameObject debugScreen;                                                          // 디버그스크린 오브젝트
    public GameObject inventory;                                                            // 인벤토리 오브젝트
    public GameObject cursorSlot;                                                           // 인벤토리 커서 오브젝트
    public GameObject pauseMenu;                                                            // 일시정지 메뉴 오브젝트

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


    /* 플레이어 상태 변수들 */
    private bool _inUI = false;                                                             // 플레이어의 inUI상태 여부
    private bool _inPause = false;                                                          // 플레이어의 Pause상태 여부


    /* 월드의 싱글톤화 관련 변수들 */
    private static World _instance;                                                         // 프라이빗 월드
    public static World Instance { get { return _instance; } }                              // 프라이빗 월드의 반환자
    




    private void Awake()
    {
        // 게임 프레임 60으로 제한
        Application.targetFrameRate = 60;
        
        
        /* 메인 메뉴에서 파일을 받아오는 상황 */
        /*
        mapName = MainMenu.nameFromMainScene;
        worldData.worldName = mapName;
        Debug.Log("Map name : " + mapName);

        mapFileLocation = MainMenu.locationFromMainScene;
        Debug.Log("Map File Location : " + mapFileLocation);
        // mapName과 mapFileLocation을 MainMenu로부터 받아옴

        isBuildMode = MainMenu.isOpendinBuildMode;
        */
        /* 따로 파일을 만드는 상황(테스트 목적) */
        mapName = "testing";
        mapFileLocation = Application.dataPath + "/MapFolder/";
        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(mapFileLocation + mapName + " settings.cfg", jsonExport);

        isBuildMode = false;

        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
        // 현재 생성한 월드를 유일한 월드(싱글톤)으로 만들어줌
    }

    private void Start()
    {
        worldData = SaveSystem.LoadWorld(mapName);

        ImportSetting();
        SetGlobalLightValue();

        if(isBuildMode)
        {
            Debug.Log(mapName + " Build Mode");
            clearPlayer.gameObject.SetActive(false);
            spawnPosition = new Vector3(-5, 5, -5);
            buildPlayer.transform.position = spawnPosition;
            player.position = buildPlayer.transform.position;
            playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
        }
        else
        {
            Debug.Log(mapName + " Clear Mode");
            buildPlayer.gameObject.SetActive(false);
            spawnPosition = new Vector3(settings.cpX, settings.cpY, settings.cpZ);
            buildPlayer.transform.position = spawnPosition;
            player.position = buildPlayer.transform.position;
            playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
        }
        
        LoadWorld();
        GenerateWorld();
    }

    void Update()
    {
        if (isBuildMode)
            player.position = buildPlayer.transform.position;
        else
            player.position = clearPlayer.transform.position;

        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();
        // 플레이어의 ChunkCoord위치가 변경될 시 CheckViewDistance함수 실행

        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");
        // 생성해야 할 청크 큐가 존재할 시 CreateChunks 코루틴 실행

        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);
        // F3키를 눌렀을 시 디버그스크린 활성화/비활성화

        if (Input.GetKeyDown(KeyCode.F1))
            SaveWorld();
        // F1키를 눌렀을 시 월드 데이터 저장

        if(isSettingsChanged)
        {
            ImportSetting();
            SetGlobalLightValue();
            Debug.Log("Setting Changed");
            isSettingsChanged = false;
        }
        // 설정이 변경되었을 시 실행하는 함수들
    }



    /* 맵 이외 설정을 다루는 함수 */

    void ImportSetting()
    {
        string jsonImport = File.ReadAllText(mapFileLocation + mapName + " settings.cfg");
        settings = JsonUtility.FromJson<Settings>(jsonImport);
    }                                                                 // 변경된 설정을 Json파일로 Import해주는 함수

    public void SetGlobalLightValue()
    {
        if(settings.isDay)
        {
            Shader.SetGlobalFloat("GlobalLightLevel", 0);
            RenderSettings.skybox = daySky;
        }
        else
        {
            Shader.SetGlobalFloat("GlobalLightLevel", 0.7f);
            RenderSettings.skybox = nightSky;
        } 
    }                                                    // 낮/밤에 대한 라이트와 스카이 머터리얼을 설정해주는 함수

    public void SaveWorld()
    {
        SaveSystem.SaveWorld(worldData);
    }                                                              // 월드 데이터를 저장하는 함수



    /* 월드를 다루는 함수 모음 */

    private void GenerateWorld()
    {
        for (int x = playerLastChunkCoord.x - settings.viewDistance; x < playerLastChunkCoord.x + settings.viewDistance; x++)
        {
            for (int z = playerLastChunkCoord.z - settings.viewDistance; z < playerLastChunkCoord.z + settings.viewDistance; z++)
            {
                if(IsChunkInWorld(x, z))
                {
                    chunks[x, z] = new Chunk(new ChunkCoord(x, z), true);
                    activeChunks.Add(new ChunkCoord(x, z));
                }
            }
        }
    }                                                         // 초기 위치에 청크를 생성하고 플레이어 포지션을 설정해주는 함수


    private void LoadWorld()
    {
        for (int x = playerLastChunkCoord.x - settings.viewDistance - 1; x < playerLastChunkCoord.x + settings.viewDistance + 1; x++)
        {
            for (int z = playerLastChunkCoord.z - settings.viewDistance - 1; z < playerLastChunkCoord.z + settings.viewDistance + 1; z++)
            {
                if (IsChunkInWorld(x, z))
                {
                    worldData.LoadChunk(new Vector2Int(x, z));
                }
            }
        }
    }                                                             // 저장된 청크 데이터를 불러오는 함수


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

        for (int x = coord.x - settings.viewDistance; x < coord.x + settings.viewDistance; x++)
        {
            for (int z = coord.z - settings.viewDistance; z < coord.z + settings.viewDistance; z++)
            {
                ChunkCoord thisChunkCoord = new ChunkCoord(x, z);

                if (IsChunkInWorld(x, z))
                { 
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(thisChunkCoord, false);
                        chunksToCreate.Add(thisChunkCoord);
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                    }
                    activeChunks.Add(thisChunkCoord);

                    
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    { 
                        if (previouslyActiveChunks[i].Equals(thisChunkCoord))
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
                Cursor.lockState = CursorLockMode.None;
            }
                
            else
            {
                inventory.SetActive(false);
                cursorSlot.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }   
        }
    }                                                                     // inUI 설정

    public bool inPause
    {
        get { return _inPause; }
        set
        {
            _inPause = value;

            if (_inPause)
            {
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }                                                                  // inPause 설정
        
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

    public int CheckVoxel (Vector3 pos)
    {
        return worldData.GetVoxel(pos);
    }                                                  // 월드좌표 pos 위치의 블록의 타입을 반환하는 함수
}


[System.Serializable]
public class BlockType
{
    public string blockName;                                                                // 블록의 이름
    public byte blockType;                                                                  // 블록의 타입 (0 = 공기 ...)
    public Sprite icon;                                                                     // 블록 아이콘
    public GameObject block;                                                                // 블록 오브젝트
}

[System.Serializable]
public class Settings
{
    public string mapName = "";                                                             // 맵 이름
    public bool isCanBeCleared = false;                                                     // 클리어모드 진입 가능 맵 여부
    public int cpX = 5, cpY = 5, cpZ = 5;                                                   // 클리어모드 플레이어의 스폰좌표

    [Range(2, 4)]
    public int viewDistance = 2;                                                            // 가시거리

    public bool isDay = true;                                                               // 낮/밤 여부

    [Range(0.1f, 2.0f)]
    public float mouseSensitivity = 1f;                                                     // 마우스 민감도
}
