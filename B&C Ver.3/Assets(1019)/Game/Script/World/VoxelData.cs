using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
	public static readonly int TextureAtlasSizeInBlocks = 2;
	public static float NormalizedBlockTextureSize
	{

		get { return 1f / (float)TextureAtlasSizeInBlocks; }

	}

	public static readonly Vector3Int[] voxelVerts = new Vector3Int[8] {

		new Vector3Int(0, 0, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(1, 1, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, 0, 1),
		new Vector3Int(1, 0, 1),
		new Vector3Int(1, 1, 1),
		new Vector3Int(0, 1, 1),

	};

	public static readonly int[,] voxelTris = new int[6, 4] {

        // Back, Front, Top, Bottom, Left, Right

		// 0 1 2 2 1 3
		{0, 3, 1, 2}, // Back Face
		{5, 6, 4, 7}, // Front Face
		{3, 7, 2, 6}, // Top Face
		{1, 5, 0, 4}, // Bottom Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6} // Right Face

	};

	public static readonly Vector2Int[] voxelUvs = new Vector2Int[4] {

		new Vector2Int (0, 0),
		new Vector2Int (0, 1),
		new Vector2Int (1, 0),
		new Vector2Int (1, 1),

	};
}
