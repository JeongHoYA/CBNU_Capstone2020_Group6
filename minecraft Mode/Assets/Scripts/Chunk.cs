using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Chunk
{

    public ChunkCoord coord;

    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>(); //사용할 메쉬에 저장할 정점 배열
    List<int> triangles = new List<int>(); // 다른 삼각형 목록
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth]; // 박스를 확인하는 방법

    World world;

    public Chunk (ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>(); // 새로운 게임 오브젝트와 동시에 이 값에 대한 참조를 적용
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform); // 실제로 설정된 부모를 사용
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);  // 이것으로 부터 부모를 설정하면 청크 객체 변환을 수행한다.
        chunkObject.name = "Chunk" + coord.x + "," + coord.z; //인스펙터에서 더 쉽게 따라 할수 있도록 자신의 이름을 말한다.
         
        PopulateVoxelMap();

        CreateMeshData();

        CreateMesh();
    }


    void PopulateVoxelMap() //상자 채우기 함수
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)    // 아래서부터 위로 쌓아올리는 형태
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z)+position);

                }
            }
        }
    }

    void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)    // 아래서부터 위로 쌓아올리는 형태
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    AddVoxelDataToChunk(new Vector3(x, y, z));


                }
            }
        }
    }


    public bool isActive  //청크를 켜고 끌수 있게 하는 함수
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }

    }

    public Vector3 position// 청크 객체 변환 위치로 이동하지 않음
    {
        get { return chunkObject.transform.position; }
    }


    bool IsVoxelInChunk(int x,int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;
                
    }


    bool CheckVoxel(Vector3 pos) // 복셀을 확인하는 함수 방법: 주어진 위치를 나타내는 복셀을 보고 복셀이 있는지 여부에 따라 참거짓을 반환
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);


        if (!IsVoxelInChunk(x,y,z))  // chunk의 범위내에 있는지 확인한다.
            return world.blocktypes[world.GetVoxel(pos + position)].isSolid;

        return world.blocktypes[voxelMap[x, y, z]].isSolid;

    }

    void AddVoxelDataToChunk(Vector3 pos)
    {

        for (int p = 0; p < 6; p++)
        {

            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {

                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blocktypes[blockID].GetTextureID(p));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;

            }
        }

    }

    void CreateMesh()
    {

        Mesh mesh = new Mesh();     // 삼각형을 수직으로 추가
        mesh.vertices = vertices.ToArray();     //넣을수 있는 메시를 만듬
        mesh.triangles = triangles.ToArray();    //꼭짓점을 삼각형과 동일하게 배열
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals(); //  법선을 계산

        meshFilter.mesh = mesh;

    }

    void AddTexture(int textureID)
    {

        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));


    }


}


public class ChunkCoord
{

    public int x;  // 그리는 공간이지만 월드공간에는 없는 청크
    public int z;   // 그리는 공간이지만 월드공간에는 없는 청크

    public ChunkCoord(int _x, int _z)
    {

        x = _x;
        z = _z;

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