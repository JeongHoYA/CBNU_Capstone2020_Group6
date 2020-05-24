using UnityEngine;

public class Chunk {

    /* 청크와 복셀의 규격에 관한 변수 모음*/
    public static readonly int chunkHeight = 20;                                            // 청크의 복셀에 대한 높이
    public static readonly int chunkWidth = 10;                                             // 청크의 복셀에 대한 가로세로 길이
    public static readonly int worldSizeInChunks = 10;                                      // 월드의 청크에 대한 가로세로 길이
    public static int worldSizeInBlocks {
        get { return chunkWidth * worldSizeInChunks; }
    }                                                 // 월드의 복셀에 대한 가로세로 길이
    public static readonly int viewDistanceInChunks = 4;                                    // 플레이어가 볼 수 있는 청크의 가로세로 길이


    /* 오브젝트 모음 */
    World world;                                                                            // 월드 오브젝트
    GameObject chunkObject;                                                                 // 청크 오브젝트


    public ChunkCoord coord;                                                                // 청크 상대좌표
    public int[,,] voxelMap = new int[chunkWidth, chunkHeight, chunkWidth];
    // 청크 내 복셀의 상대 위치에 복셀의 코드를 저장하는 리스트

    public Transform transform;                                                             // 청크의 트랜스폼


    /* 청크를 다루는 함수 모음 */

    public Chunk(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;

        chunkObject = new GameObject();
        chunkObject.transform.position = new Vector3Int(coord.x * chunkWidth, 0, coord.z * chunkWidth);
        chunkObject.transform.SetParent(world.transform);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

        this.transform = chunkObject.transform;

        GetChunk();
        SetChunk();
    }                                        // 청크의 생성자

    public void GetChunk()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3Int(x, y, z) + position);
                }
            }
        }
    }                                                               // World.Getvoxel을 이용해 voxelMap에 코드를 저장

    public void SetChunk()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    if(voxelMap[x, y, z] != 0)
                        world.PlaceVoxel(voxelMap[x, y, z], new Vector3Int(x, y, z), position, this);
                }
            }
        }
    }                                                               // voxelMap에 저장된 코드를 바탕으로 청크 생성




    /* 좌표or데이터 변환 및 값 설성or반환 관련 함수 모음 */

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
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }                                                                 // 청크의 isActive여부를 반환하는 함수

    public Vector3Int GetPosFromChunkCoord(ChunkCoord coord)
    {
        int x = coord.x * Chunk.chunkWidth;
        int z = coord.z * Chunk.chunkWidth;

        return new Vector3Int(x, 0, z);
    }                             // ChunkCoord좌표를 Vector3Int로 변환해주는 함수
}


/* ChunkCoord(청크의 상대좌표) 클래스 */
public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public ChunkCoord(Vector3Int pos)
    {
        x = pos.x / Chunk.chunkWidth;
        z = pos.z / Chunk.chunkWidth;
    }

    public ChunkCoord(Vector3 pos)
    {
        x = Mathf.FloorToInt(pos.x) / Chunk.chunkWidth;
        z = Mathf.FloorToInt(pos.z) / Chunk.chunkWidth;
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

