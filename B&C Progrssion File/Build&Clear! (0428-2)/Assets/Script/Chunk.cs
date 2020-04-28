﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


/* 복셀과 청크를 만드는 클래스 */
public class Chunk
{
	World world;												// 월드오브젝트
	GameObject chunkObject;                                     // 게임오브젝트 청크

	public ChunkCoord coord;									// 월드에 대한 청크의 상대좌표 (ChunkCoord좌표)

	MeshRenderer meshRenderer;									// 청크 메시 렌더러
	MeshFilter meshFilter;										// 청크 메시 필터

	int vertexIndex = 0;										

	/* 복셀 생성 리스트 */
	List<Vector3> vertices = new List<Vector3>();				// 복셀의 꼭짓점 리스트
	List<int> triangles = new List<int>();                      // 복셀의 삼각형 리스트
	List<Vector2> uvs = new List<Vector2>();                    // 복셀의 텍스처 적용에 필요한 리스트

	List<int> transparentTriangles = new List<int>();           // transparent 복셀의 삼각형 리스트
	Material[] materials = new Material[2];                     // 머터리얼 리스트 (0 : 불투명, 1: 투명)

	public int[,,] voxelMap = new int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
	// 청크 내 각각 복셀의 위치와 복셀 인덱스를 저장하는 리스트

	private bool _isActive;                                     // 청크 오브젝트의 isActive값
	public bool isVoxelMapPopulated = false;					// 복셀맵이 생성되었나





	/* 월드 내 청크를 다루는 함수 모음 */

	public Chunk (ChunkCoord _coord, World _world, bool generateOnLoad)
	{
		coord = _coord;
		world = _world;
		isActive = true;

		if (generateOnLoad)
			Init();

	}	// generateOnLoad가 true일 시 Init 함수 실행

	public void Init()
	{
		chunkObject = new GameObject();

		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();

		materials[0] = world.material;
		materials[1] = world.transparentMaterial;
		meshRenderer.materials = materials;

		chunkObject.transform.SetParent(world.transform);
		chunkObject.transform.position = new Vector3Int(coord.x * VoxelData.ChunkWidth, 0, coord.z * VoxelData.ChunkWidth);
		chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

		PopulateVoxelMap();
		UpdateChunk();
	}                                           // 월드 내 _coord 좌표상에 새 청크를 생성하는 함수





	/* 청크 내 복셀을 다루는 함수 모음 */

	void PopulateVoxelMap()
	{
		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{
					voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
				}
			}
		}
		isVoxelMapPopulated = true;
	}                                      // voxelMap 리스트에 world.GetVoxel이 반환해온 복셀 인덱스 저장

	public void EditVoxel(Vector3 pos, int newID)
	{
		int xCheck = Mathf.FloorToInt(pos.x);
		int yCheck = Mathf.FloorToInt(pos.y);
		int zCheck = Mathf.FloorToInt(pos.z);

		xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
		zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

		if (isVoxelInChunk(xCheck, yCheck, zCheck))
		{
			voxelMap[xCheck, yCheck, zCheck] = newID;
			UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
		}
		UpdateChunk();
	}				// pos좌표의 복셀 인덱스를 newID로 치환

	void UpdateSurroundingVoxels(int x, int y, int z)
	{
		Vector3 thisVoxel = new Vector3(x, y, z);

		for (int p = 0; p < 6; p++)
		{
			Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

		    if (world.IsVoxelInWorld(currentVoxel + position))
			{
				world.GetChunkFromVector3(currentVoxel + position).UpdateChunk();
			}		
		}
	}			// [x, y, z]좌표와 인접한 복셀의 텍스처를 업데이트
	 
	void UpdateChunk()
	{
		ClearMeshData();

		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{
					if (!world.blockTypes[voxelMap[x, y, z]].isAir)
						UpdateMeshData(new Vector3Int(x, y, z));
				}
			}
		}
		CreateMesh();
	}											// 청크 내에서 !isAir한 복셀의 좌표(VoxelMap[,,])를 바탕으로 청크를 업데이트

	void ClearMeshData()
	{
		vertexIndex = 0;
		vertices.Clear();
		triangles.Clear();
		transparentTriangles.Clear();
		uvs.Clear();
	}											// 복셀의 메시 데이터를 삭제하는 함수

	void UpdateMeshData(Vector3Int pos)
	{
		int blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
		bool isTransparent = world.blockTypes[blockID].isTransparent;
		// 복셀의 투명도 확인
		
		for (int p = 0; p < 6; p++)
		{   // p = 복셀의 상하전후좌우 6면
			if (CheckVoxelisTransparent(pos + VoxelData.faceChecks[p]))
			{   // 인접한 청크들이 없을 시에만 시행
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

				AddTexture(world.blockTypes[blockID].GetTextureID(p));

				if (!isTransparent)
				{
					triangles.Add(vertexIndex);
					triangles.Add(vertexIndex + 1);
					triangles.Add(vertexIndex + 2);
					triangles.Add(vertexIndex + 2);
					triangles.Add(vertexIndex + 1);
					triangles.Add(vertexIndex + 3);
				}
				else
				{
					transparentTriangles.Add(vertexIndex);
					transparentTriangles.Add(vertexIndex + 1);
					transparentTriangles.Add(vertexIndex + 2);
					transparentTriangles.Add(vertexIndex + 2);
					transparentTriangles.Add(vertexIndex + 1);
					transparentTriangles.Add(vertexIndex + 3);
				}
				vertexIndex += 4;
			}
		}
	}							// pos위치에 있는 복셀의 메시 데이터 업데이트

	void CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();

		mesh.subMeshCount = 2;
		mesh.SetTriangles(triangles.ToArray(), 0);
		mesh.SetTriangles(transparentTriangles.ToArray(), 1);

		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}											// 메시 데이터를 바탕으로 복셀의 메시를 생성하는 함수

	void AddTexture (int textureID)
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
	}                              // textureID를 입력받아 텍스처 아틀라스에서 그에 맞는 텍스처를 가져와 복셀에 추가해주는 함수





	/* 좌표or데이터 변환 및 값 설성or반환 관련 함수 모음 */

	bool CheckVoxelisTransparent(Vector3Int pos)
	{
		int x = pos.x;
		int y = pos.y;
		int z = pos.z;

		// 좌표가 청크의 범위를 벗어날 시 월드좌표로 반환
		if (!isVoxelInChunk(x, y, z))
			return world.CheckForVoxelisTransparent(pos + position);
		// 아니면 청크 내 복셀맵 좌표로 반환
		return world.blockTypes[voxelMap[x, y, z]].isTransparent;
	}					// Vector3Int 형식의 절대좌표에 위치한 복셀의 isTransparent 값 반환
	public int GetVoxelFromGlobalVector3(Vector3 pos)
	{
		int xCheck = Mathf.FloorToInt(pos.x);
		int yCheck = Mathf.FloorToInt(pos.y);
		int zCheck = Mathf.FloorToInt(pos.z);

		xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
		zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

		if (isVoxelInChunk(xCheck, yCheck, zCheck))
			return voxelMap[xCheck, yCheck, zCheck];
		else
			return 0;
	}			// Vector3 절대좌표에 위치한 복셀의 인덱스값 반환
	bool isVoxelInChunk(int x, int y, int z)
	{
		if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
			return false;
		else
			return true;
	}						// 복셀의 청크에 대한 로컬 좌표[x, y, z]가 청크를 벗어나면 false 반환
	public bool isActive
	{
		get { return _isActive; }
		set
		{
			_isActive = value;
			if (chunkObject != null)
				chunkObject.SetActive(value);
		}
	}											// chunkObject의 isActive상태값 설정 및 반환
	public Vector3 position
	{
		get { return chunkObject.transform.position; }
	}										// chunkObject의 position값 반환
}


/* 월드에 대한 청크의 상대좌표 */
public class ChunkCoord
{
	public int x;
	public int z;

	public ChunkCoord()
	{
		x = 0;
		z = 0;
	}											// [0, 0]에 대한 ChunkCoord 좌표 생성자
	public ChunkCoord (int _x, int _z)
	{
		x = _x;
		z = _z;
	}							// [x, z]에 대한 ChunkCoord 좌표 생성자
	public ChunkCoord (Vector3 pos)
	{
		int xCheck = Mathf.FloorToInt(pos.x);
		int zCheck = Mathf.FloorToInt(pos.z);

		x = xCheck / VoxelData.ChunkWidth;
		z = zCheck / VoxelData.ChunkWidth;
	}								// Vector3 pos에 대한 ChunkCoord 좌표 생성자
	public bool Equals (ChunkCoord other)
	{
		if (other == null)
			return false;
		else if (other.x == x && other.z == z)
			return true;
		else
			return false;
	}						// ChunkCoord 좌표 비교 함수
}