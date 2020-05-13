using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;



    void Start()
    {

        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>(); //사용할 메쉬에 저장할 정점 배열
        List<int> triangles = new List<int> (); // 다른 삼각형 목록
        List<Vector2> uvs = new List<Vector2>();


        for (int p = 0; p < 6; p++)
        {

            for (int i = 0; i < 6; i++)// 4개의 고리
            {

                int triangleIndex = VoxelData.voxelTris[p, i];
                vertices.Add(VoxelData.voxelVerts[triangleIndex]);
                triangles.Add(vertexIndex);

                uvs.Add(VoxelData.voxelUvs [i]);

                vertexIndex++;

            }
        }


        Mesh mesh = new Mesh();     // 삼각형을 수직으로 추가
        mesh.vertices = vertices.ToArray();     //넣을수 있는 메시를 만듬
        mesh.triangles = triangles.ToArray();    //꼭짓점을 삼각형과 동일하게 배열
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals(); //  법선을 계산

        meshFilter.mesh = mesh;


    }

}
