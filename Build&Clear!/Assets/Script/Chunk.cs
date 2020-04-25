using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Chunk : MonoBehaviour
{
	World world;

	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	int vertexIndex = 0;

	/* 복셀 생성 리스트 */
	List<Vector3> vertices = new List<Vector3>();				// 복셀의 꼭짓점 리스트
	List<int> triangles = new List<int>();						// 복셀의 삼각형 리스트
	List<Vector2> uvs = new List<Vector2>();                    // 복셀의 텍스처 적용에 필요한 리스트

	byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];	// 청크 내 각각 복셀의 코드를 저장하는 리스트

	void Start()
	{
		world = GameObject.Find("World").GetComponent<World>();

		PopulateVoxelMap();
		CreateMeshData();
		CreateMesh();
	}



	/* 복셀, 청크 관련 함수 모음 */

	void PopulateVoxelMap()
	{
		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{
					if (y < 1)
						voxelMap[x, y, z] = 1;
					else if (y == VoxelData.ChunkHeight - 1)
						voxelMap[x, y, z] = 5;
					else
						voxelMap[x, y, z] = 2;
				}
			}
		}
	}                                      // voxelMap 리스트에 일정한 규칙으로 복셀 코드 저장

	void CreateMeshData()
	{
		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{
					AddVoxelDataToChunk(new Vector3Int(x, y, z));
				}
			}
		}
	}										// 청크 크기만큼 AddVoxelDataToChunk 함수 실행

	void AddVoxelDataToChunk(Vector3Int pos)
	{	// p = 복셀의 상하전후좌우 6면
		for (int p = 0; p < 6; p++)
		{	// 인접한 청크들이 없을 시에만 시행
			if(!CheckVoxel(pos + VoxelData.faceChecks[p]))
			{
				byte blockID = voxelMap[pos.x, pos.y, pos.z];

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
	}								// textureID를 입력받아 텍스처 아틀라스에서 그에 맞는 텍스처 가져와 복셀에 추가해주는 함수





	/* 좌표or데이터 변환 관련 함수 모음 */

	bool CheckVoxel(Vector3Int pos)
	{
		int x = pos.x; int y = pos.y; int z = pos.z;

		// 좌표가 청크의 범위를 벗어날 시 false 반환
		if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
			return false;

		return world.blockTypes[voxelMap[x, y, z]].isSolid;
	}								// Vector3Int 형식의 절대좌표에 위치한 VoxelMap의 isSolid 값 반환
	bool CheckVoxel(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		// 좌표가 청크의 범위를 벗어날 시 false 반환
		if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight || z < 0 || z > VoxelData.ChunkWidth)
			return false;

		return world.blockTypes[voxelMap[x, y, z]].isSolid;
	}									// Vector3 형식의 절대좌표에 위치한 VoxelMap의 isSolid 값 반환

}