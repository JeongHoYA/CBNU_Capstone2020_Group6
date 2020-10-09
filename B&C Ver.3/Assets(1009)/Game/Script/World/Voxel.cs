using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    int vertexIndex = 0;

    void Start()
    {
        AddVoxelData();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddVoxelData()
    {
        for (int p = 0; p < 6; p++)
        {
            vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
            vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
            vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
            vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

            AddTexture(GetTextureID(p));

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);
            vertexIndex += 4;
        }
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

    int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0: // Back
                return 3;
            case 1: // Front
                return 2;
            case 2: // Top
                return 0;
            case 3: // Down
                return 1;
            case 4: // 
                return 3;
            case 5:
                return 2;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }
}