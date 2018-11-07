using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBox
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Vector3> normals = new List<Vector3>();
    private List<Vector4> tangents = new List<Vector4>();
    private List<int> triangles = new List<int>();

    public Mesh mesh;

    public int AddVertex(Vector3 vertex, Vector2 uv, Vector3 normal, Vector4 tangent)
    {
        vertices.Add(vertex);
        uvs.Add(uv);
        normals.Add(normal);
        tangents.Add(tangent);
        return vertices.Count - 1;
    }

    public void AddTriangle(int p1, int p2, int p3)
    {
        triangles.Add(p1);
        triangles.Add(p2);
        triangles.Add(p3);
    }

    public void Combine()
    {
        if (this.vertices.Count == 0) return;
        if (mesh != null) return;
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.tangents = tangents.ToArray();
    }
}