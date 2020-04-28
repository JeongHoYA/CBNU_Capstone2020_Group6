using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;


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
	List<int> triangles = new List<int>();						// 복셀의 삼각형 리스트
	List<Vector2> uvs = new List<Vector2>();                    // 복셀의 텍스처 적용에 필요한 리스트

	public int[,,] voxelMap = new int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];	// 청크 내 각각 복셀의 위치와 복셀코드를 저장하는 리스트




	/* 월드 내 청크를 다루는 함수 모음 */

	public Chunk (ChunkCoord _coord, World _world)
	{
		coord = _coord;
		world = _world;
		chunkObject = new GameObject();

		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshRenderer.material = world.material;

		chunkObject.transform.SetParent(world.transform);
		chunkObject.transform.position = new Vector3Int(coord.x * VoxelData.ChunkWidth, 0, coord.z * VoxelData.ChunkWidth);
		chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

		PopulateVoxelMap();
		CreateMeshData();
		CreateMesh();
	}				// 월드 내 _coord 좌표상에 새 청크를 생성하는 함수



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
	}                                      // voxelMap 리스트에 world.GetVoxel이 반환해온 복셀 코드 저장

	void CreateMeshData()
	{
		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{
					if(!world.blockTypes[voxelMap[x, y, z]].isAir)
						AddVoxelDataToChunk(new Vector3Int(x, y, z));
				}
			}
		}
	}										// 청크 내에서 !isAir한 복셀의 좌표(VoxelMap[,,])를 바탕으로 AddVoxelDataToChunk 함수 실행

	void AddVoxelDataToChunk(Vector3 pos)
	{	// p = 복셀의 상하전후좌우 6면
		for (int p = 0; p < 6; p++)
		{	// 인접한 청크들이 없을 시에만 시행
			if(!CheckVoxel(pos + VoxelData.faceChecks[p]))
			{
				int blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

				AddTexture(world.blockTypes[blockID].GetTextureID(p));

				triangles.Add(vertexIndex);
				triangles.Add(vertexIndex + 1);
				triangles.Add(vertexIndex + 2);
				triangles.Add(vertexIndex + 2);
				triangles.Add(vertexIndex + 1);
				triangles.Add(vertexIndex + 3);
				vertexIndex += 4;
			}
		}
	}						// VoxelData 클래스의 복셀 생성 리스트들을 청크 내 pos위치의 복셀 생성 리스트에 삽입하는 함수

	void CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}											// 리스트에 삽입된 내용을 바탕으로 복셀의 매쉬를 생성하는 함수

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
	}                              // textureID를 입력받아 텍스처 아틀라스에서 그에 맞는 텍스처 가져와 복셀에 추가해주는 함수




	/* 좌표or데이터 변환 및 값 설성or반환 관련 함수 모음 */

	bool CheckVoxel(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		// 좌표가 청크의 범위를 벗어날 시 월드좌표로 반환
		if (!isVoxelInChunk(x, y, z))
			return !world.blockTypes[world.GetVoxel(pos + position)].isAir;
		// 아니면 청크 내 복셀맵 좌표로 반환
		return !world.blockTypes[voxelMap[x, y, z]].isAir;
	}									// Vector3 형식의 절대좌표에 위치한 VoxelMap의 !isAir 값 반환
	bool isVoxelInChunk(int x, int y, int z)
	{
		if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
			return false;
		else
			return true;
	}						// 복셀의 청크에 대한 로컬 좌표[x, y, z]가 청크를 벗어나면 false 반환
	public bool isActive
	{
		get { return chunkObject.activeSelf; }
		set { chunkObject.SetActive(value); }
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

	public ChunkCoord (int _x, int _z)
	{
		x = _x;
		z = _z;
	}							// ChunkCoord 좌표 생성자
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