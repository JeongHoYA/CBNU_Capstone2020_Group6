using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData
{
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeigth = 128;
    public static readonly int WorldSizeInChunks = 100;

    //Lighting Values
    public static float minLightLevel = 0.1f;
    public static float maxLightLevel = 0.9f;
    public static float lightFalloff = 0.08f;

    public static int seed;

    public static int WorldCenter
    {
        get { return (WorldSizeInChunks * ChunkWidth) / 2; }
    }

    public static int WorldSizeInVoxels
    {
        get { return WorldSizeInChunks * ChunkWidth; }
    }

    public static readonly int TextureAtlasSizeInBlock = 16;

    public static float NormalizedBlockTextureSize
    {
        get { return 1f / (float)TextureAtlasSizeInBlock; }
    }

    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f),
    }; //큐브의 모든 점 벡터
    
    public static readonly Vector3Int[] faceChecks = new Vector3Int[6]
    {
        new Vector3Int(0, 0, -1), // Back Check
        new Vector3Int(0, 0, 1), // Front Check
        new Vector3Int(0, 1, 0), // Top Check
        new Vector3Int(0, -1, 0), // Bottom Check
        new Vector3Int(-1, 0, 0), // Left Check
        new Vector3Int(1, 0, 0) // Right Check
    }; //타 큐브면과의 접촉 여부
    
    public static readonly int[,] voxelTris = new int[6, 4]
    {
        //Back, Front, Top, Bottom, Left, Right

        //패턴 : 0 1 2 2 1 3
        {0, 3, 1, 2}, // Back Face
        {5, 6, 4, 7}, // Front Face
        {3, 7, 2, 6}, // Top Face
        {1, 5, 0, 4}, // Bottom Face
        {4, 7, 0, 3}, // Left Face
        {1, 2, 5, 6} // Right Face
    }; //큐브의 면 렌더링을 위한 배열

    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        //패턴 : 0 1 2 2 1 3
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(1.0f, 1.0f)
    }; //텍스처링용 벡터 모음
}
