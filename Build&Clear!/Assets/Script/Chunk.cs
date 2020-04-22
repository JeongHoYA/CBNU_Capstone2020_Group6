using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // 청크(1x1x1크기의 큐브)에 관련된 스크립트
public class Chunk : MonoBehaviour
{
	World world; // 월드 클래스 레퍼런스
	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	int vertexIndex = 0;
	List<Vector3> vertices = new List<Vector3>(); // 복셀 꼭짓점 리스트
	List<int> triangles = new List<int>(); // 복셀 면 리스트
	List<Vector2> uvs = new List<Vector2>(); // 복셀 텍스처 리스트

	// [,,] 위치에 존재하는 복셀의 종류를 저장하는 byte 리스트
	byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
	

	void Start()
	{
		world = GameObject.Find("World").GetComponent<World>(); // 월드 클래스에 "World" 할당

		PopulateVoxelMap();
		CreateMeshData();
		CreateMesh();
	}

	// (byte list)voxelMap 에 저장된 해당 위치에 대한 블록의 isSolid여부 반환
	// pos의 x/y/z축의 값이 0 미만, VoxelData.ChunkWidth/ChunkHeight 초과일 시 false 반환
	bool CheckVoxel(Vector3Int pos)
	{
		if (pos.x < 0 || pos.x > VoxelData.ChunkWidth - 1 || pos.y < 0 || pos.y > VoxelData.ChunkHeight - 1 || pos.z < 0 || pos.z > VoxelData.ChunkWidth - 1)
			return false;

		return world.blockTypes[voxelMap[pos.x, pos.y, pos.z]].isSolid;
	}


	// 해당 위치에 대한 (bool list)voxelMap 변수를 true로 설정
	void PopulateVoxelMap()
	{
		for (int y = 0; y < VoxelData.ChunkHeight; y++)
		{
			for (int x = 0; x < VoxelData.ChunkWidth; x++)
			{
				for (int z = 0; z < VoxelData.ChunkWidth; z++)
				{
					voxelMap[x, y, z] = 0;
					// 0 = 베이스 블록 코드넘버
				}
			}
		}
	}
	

	//해당 위치에 대해 AddVoxelDataToChunk를 호출
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
	}


	// 데이터를 복셀 리스트들에 삽입
	// Vector3 pos(위치)를 입력받아 복셀의 꼭짓점들의 위치를 pos만큼 이동시킴
	// Vertices = 꼭짓점, triangles = 면, AddTexture = 텍스처 데이터 삽입 함수
	void AddVoxelDataToChunk(Vector3Int pos)
	{
		for (int p = 0; p < 6; p++)
		{		// 상하전후좌우에 다른 복셀이 존재하지 않을 시에만 데이터 삽입
			if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
			{
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

				byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
				AddTexture(world.blockTypes[blockID].TextureID);

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

	// 리스트에 삽입된 데이터를 바탕으로 메쉬 생성
	void CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray(); // 꼭짓점 리스트
		mesh.triangles = triangles.ToArray(); // 면 리스트
		mesh.uv = uvs.ToArray(); // 텍스처 리스트

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}

	// 텍스처 아틀라스의 각 텍스처를 좌상단부터 0, 1, 2, 3... 순서로 번호를 매긴 후
	// 입력받은 textureID에 걸맞는 텍스처를 텍스처 리스트에 삽입
	void AddTexture (int textureID)
	{
		float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
		float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

		x *= VoxelData.NormallizedBlockTextureSize;
		y *= VoxelData.NormallizedBlockTextureSize;
		y = 1f - y - VoxelData.NormallizedBlockTextureSize;

		uvs.Add(new Vector2(x, y));
		uvs.Add(new Vector2(x, y + VoxelData.NormallizedBlockTextureSize));
		uvs.Add(new Vector2(x + VoxelData.NormallizedBlockTextureSize, y));
		uvs.Add(new Vector2(x + VoxelData.NormallizedBlockTextureSize, y + VoxelData.NormallizedBlockTextureSize));
	}
}