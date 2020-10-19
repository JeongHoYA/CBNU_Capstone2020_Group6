using UnityEngine;

public class Chunk {

    /* 월드, 청크, 복셀의 규격에 관한 변수 모음*/
    public static readonly int chunkHeight = 20;                                            // 청크의 복셀에 대한 높이
    public static readonly int chunkWidth = 10;                                             // 청크의 복셀에 대한 가로세로 길이
    public static readonly int worldSizeInChunks = 10;                                      // 월드의 청크에 대한 가로세로 길이
    public static int worldSizeInBlocks {
        get { return chunkWidth * worldSizeInChunks; }
    }                                                 // 월드의 복셀에 대한 가로세로 길이
    //public static readonly int viewDistanceInChunks = 4;                                    // 플레이어가 볼 수 있는 청크의 가로세로 길이


    /* 청크 오브젝트 관련 모음 */
    GameObject chunkObject;                                                                 // 청크 오브젝트

    public ChunkCoord coord;                                                                // 청크 상대좌표

    ChunkData chunkdata;
    //public int[,,] voxelMap = new int[chunkWidth, chunkHeight, chunkWidth];
    // 청크 내 복셀의 상대 위치에 복셀의 코드를 저장하는 리스트
    public Transform transform;                                                             // 청크의 트랜스폼

    private bool _isActive;                                                                 // 청크의 isActive 여부
    public bool isVoxelMapPopulated = false;                                                // 청크의 생성됨 여부

    /* 청크를 다루는 함수 모음 */

    public Chunk(ChunkCoord _coord, bool generateOnLoad)
    {
        coord = _coord;
        isActive = true;

        if (generateOnLoad)
            Init();
    }                                        // 청크의 생성자

    public void Init()
    {
        chunkObject = new GameObject();
        chunkObject.transform.SetParent(World.Instance.transform);
        chunkObject.transform.position = new Vector3Int(coord.x * chunkWidth, 0, coord.z * chunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

        this.transform = chunkObject.transform;

        chunkdata = World.Instance.worldData.RequestChunk(new Vector2Int((int)position.x, (int)position.z), true);

        SetChunk();
    }                                                                   // 청크의 생성 함수


    public void SetChunk()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    if(chunkdata.map[x, y, z] != 0)
                        World.Instance.PlaceVoxel(chunkdata.map[x, y, z], new Vector3Int(x, y, z), position, this);
                }
            }
        }
    }                                                               // voxelMap에 저장된 코드를 바탕으로 청크 생성


    public void EditVoxelMap (Vector3 pos, int newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        if(IsVoxelInChunk(xCheck, yCheck, zCheck))
        {
            chunkdata.map[xCheck, yCheck, zCheck] = newID;
            World.Instance.worldData.AddToModifiedChunkList(chunkdata);
        }
    }                                                       // 해당 위치의 복셀데이터에 새 복셀코드 삽입
        



    /* 좌표or데이터 변환 및 값 설정-반환 관련 함수 모음 */

    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > chunkWidth - 1 || y < 0 || y > chunkHeight - 1 || z < 0 || z > chunkWidth - 1)
            return false;
        else return true;
    }                                             // 복셀이 청크 내에 존재하는지 확인하는 함수

    Vector3Int position
    {
        get { return Vector3Int.FloorToInt(chunkObject.transform.position); }
    }                                                                  // 청크의 위치를 반환하는 함수

    public bool isActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);
        }
    }                                                                 // 청크의 isActive여부를 반환하는 함수

    public Vector3Int GetPosFromChunkCoord(ChunkCoord coord)
    {
        int x = coord.x * Chunk.chunkWidth;
        int z = coord.z * Chunk.chunkWidth;

        return new Vector3Int(x, 0, z);
    }                             // ChunkCoord좌표를 Vector3Int로 변환해주는 함수

    public int GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        return chunkdata.map[xCheck, yCheck, zCheck];
    }                                    // Vector3 좌표로 해당 위치의 VoxelMap 값을 반환해주는 함수
}


/* ChunkCoord(청크의 상대좌표) 클래스 */
public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord()
    {
        x = 0; z = 0;
    }

    public ChunkCoord(int _x, int _z)
    {
        x = _x; z = _z;
    }

    public ChunkCoord(Vector3Int pos)
    {
        x = pos.x / Chunk.chunkWidth;
        z = pos.z / Chunk.chunkWidth;
    }

    public ChunkCoord(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);

        x = xCheck / Chunk.chunkWidth;
        z = zCheck / Chunk.chunkWidth;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}

