using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 복셀과 청크의 단위 및 수 등을 모아놓은 스크립트
public static class VoxelData
{
	public static readonly int ChunkWidth = 10; // 청크(복셀의 덩어리)의 너비
	public static readonly int ChunkHeight = 1; // 청크(복셀의 덩어리)의 깊이

	public static readonly int TextureAtlasSizeInBlocks = 8; // 텍스처 아틀라스의 한 줄에 들어가는 텍스처의 개수
	public static float NormallizedBlockTextureSize // 아틀라스 내 텍스처의 상대적 크기 (1 / 텍스처 개수)
	{
		get { return 1f / (float)TextureAtlasSizeInBlocks; }
	}


	// 복셀의 꼭짓점들을 모아놓은 리스트
	public static readonly Vector3Int[] voxelVerts = new Vector3Int[8] {

		new Vector3Int(0, 0, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(1, 1, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, 0, 1),
		new Vector3Int(1, 0, 1),
		new Vector3Int(1, 1, 1),
		new Vector3Int(0, 1, 1)
	};

	// 복셀끼리의 인접 여부를 파악하는 데 사용하는 리스트
	public static readonly Vector3Int[] faceChecks = new Vector3Int[6]
	{
		new Vector3Int(0, 0, -1), // 뒷면
		new Vector3Int(0, 0, 1), // 앞면
		new Vector3Int(0, 1, 0), // 윗면
		new Vector3Int(0, -1, 0), // 아랫면
		new Vector3Int(-1, 0, 0), // 왼쪽면
		new Vector3Int(1, 0, 0) // 오른쪽면
	};

	// 복셀의 면들을 모아놓은 리스트
	public static readonly int[,] voxelTris = new int[6, 4] {
		
		//패턴 : 0 1 2 2 1 3
		{0, 3, 1, 2}, // 뒷면
		{5, 6, 4, 7}, // 앞면
		{3, 7, 2, 6}, // 윗면
		{1, 5, 0, 4}, // 아랫면
		{4, 7, 0, 3}, // 왼쪽면
		{1, 2, 5, 6} // 오른쪽면
	};

	// 북셀에 텍스처를 적용하는 데 사용하는 리스트
	public static readonly Vector2Int[] voxelUvs = new Vector2Int[4] {

		//패턴 0 1 2 2 1 3
		new Vector2Int (0, 0),
		new Vector2Int (0, 1),
		new Vector2Int (1, 0),
		new Vector2Int (1, 1)
	};
}

