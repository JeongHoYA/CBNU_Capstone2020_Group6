using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public static class VoxelData
{

    public static readonly int ChunkWidth = 5; // 박스의 너비 5
    public static readonly int ChunkHeight = 15; // 박스의 높이 5

    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {

        get { return 1f / (float)TextureAtlasSizeInBlocks; }

    }

    public static readonly Vector3[] voxelVerts = new Vector3[8]{ // 편집을 원하지 않기 때문에 읽기 전용이고 8개의 


        new Vector3(0.0f,0.0f, 0.0f),        //8개의 벡터를 3개 넣음
        new Vector3(1.0f,0.0f, 0.0f),
        new Vector3(1.0f,1.0f, 0.0f),
        new Vector3(0.0f,1.0f, 0.0f),
        new Vector3(0.0f,0.0f, 1.0f),
        new Vector3(1.0f,0.0f, 1.0f),
        new Vector3(1.0f,1.0f, 1.0f),
        new Vector3(0.0f,1.0f, 1.0f)

    };

    public static readonly Vector3[] faceChecks = new Vector3[6] // 안의 큐브가 표현될 필요가 없기 때문에 하는 작업
    {
         new Vector3(0.0f,0.0f, -1.0f),
         new Vector3(0.0f,0.0f, 1.0f),
         new Vector3(0.0f,1.0f, 0.0f),
         new Vector3(0.0f,-1.0f, 0.0f),
         new Vector3(-1.0f,0.0f, 0.0f),
         new Vector3(1.0f,0.0f, 0.0f)


    };


    public static readonly int[,] voxelTris = new int[6, 4] {


        {0, 3, 1, 2}, // Back Face
        {5, 6, 4, 7}, // Front Face
        {3, 7, 2, 6}, //Top Face
        {1, 5, 0, 4}, // Bottom Face
        {4, 7, 0, 3}, // Left Face
        {1, 2, 5, 6} // Right Face

    };

    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2(0.0f,0.0f),
        new Vector2(0.0f,1.0f),
        new Vector2(1.0f,0.0f),
        new Vector2(1.0f,1.0f)

    };

}
