using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 복셀(1x1x1 크기 큐브)과 청크(복셀의 덩어리 묶음)를 만드는 데 필요한 리스트*/
public class VoxelData
{
	public static readonly int ChunkWidth = 10;										// 청크의 복셀에 대한 가로세로 길이
	public static readonly int ChunkHeight = 20;                                    // 청크의 복셀에 대한 높이
	public static readonly int WorldSizeInChunks = 100;                             // 월드의 청크에 대한 가로세로 길이

	public static int WorldSizeInVoxels
	{
		get { return WorldSizeInChunks * ChunkWidth; }
	}											// 복셀 단위로 본 월드의 크기

	public static readonly int TextureAtlasSizeInBlocks = 8;						// 텍스처 아틀라스 한 줄에 들어가는 텍스처의 개수
	public static float NormalizedBlockTextureSize
	{
		get { return 1f / (float)TextureAtlasSizeInBlocks; }
	}								// 텍스처의 텍스처 아틀라스에 대한 상대적 크기



	public static readonly Vector3Int[] voxelVerts = new Vector3Int[8]
	{
		new Vector3Int (0, 0, 0),
		new Vector3Int (1, 0, 0),
		new Vector3Int (1, 1, 0),
		new Vector3Int (0, 1, 0),
		new Vector3Int (0, 0, 1),
		new Vector3Int (1, 0, 1),
		new Vector3Int (1, 1, 1),
		new Vector3Int (0, 1, 1),
	};			// 복셀의 꼭짓점 리스트

	public static readonly int[,] voxelTris = new int[6, 4]
	{	// 대입 순서 : 0 1 2 2 1 3
		{0, 3, 1, 2},	// 후면
		{5, 6, 4, 7},	// 전면
		{3, 7, 2, 6},	// 상면
		{1, 5, 0, 4},	// 하면
		{4, 7, 0, 3},	// 좌면
		{1, 2, 5, 6}	// 우면
	};						// 복셀의 면 리스트

	public static readonly Vector2Int[] voxelUvs = new Vector2Int[4]
	{	// 대입 순서 : 0 1 2 2 1 3
		new Vector2Int (0, 0),		
		new Vector2Int (0, 1),
		new Vector2Int (1, 0),
		new Vector2Int (1, 1)
	};			// 복셀의 면에 텍스처를 넣을 때 사용되는 리스트

	public static readonly Vector3Int[] faceChecks = new Vector3Int[6]
	{
		new Vector3Int (0, 0, -1),		// 후면
		new Vector3Int (0, 0, 1),		// 전면
		new Vector3Int (0, 1, 0),		// 상면
		new Vector3Int (0, -1, 0),		// 하면
		new Vector3Int (-1, 0, 0),		// 좌면
		new Vector3Int (1, 0, 0)		// 우면
	};			// 인접 복셀을 체크하는데 사용되는 리스트
}
