using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSplice
{
    public struct VertexInfo
    {
        public bool above;
        public int index;
    }

    public MeshBox mesh1;

    public MeshBox mesh2;

    public void Splite(Transform meshTransform, Mesh mesh, Plane plane)
    {
        mesh1 = new MeshBox();
        mesh2 = new MeshBox();

        Vector3 localPanelPoint = meshTransform.InverseTransformPoint(plane.normal * -plane.distance);
        Vector3 localPanelNormal = meshTransform.InverseTransformDirection(plane.normal);
        localPanelNormal.Scale(meshTransform.localScale);
        localPanelNormal.Normalize();

        int vecLength = mesh.vertexCount;
        VertexInfo[] newVertexInfos = new VertexInfo[vecLength];
        for (int i = 0; i < vecLength; ++i)
        {
            Vector3 vec = mesh.vertices[i];
            bool above = Vector3.Dot(vec - localPanelPoint, localPanelNormal) >= 0.0f;
            if (above == true)
            {
                newVertexInfos[i].index = mesh1.AddVertex(vec, mesh.uv[i], mesh.normals[i], mesh.tangents[i]);
            }
            else
            {
                newVertexInfos[i].index = mesh2.AddVertex(vec, mesh.uv[i], mesh.normals[i], mesh.tangents[i]);
            }
            newVertexInfos[i].above = above;
        }

        int triangleCount = mesh.triangles.Length / 3;

        for (int i = 0; i < triangleCount; ++i)
        {
            int index0 = mesh.triangles[i * 3];
            int index1 = mesh.triangles[i * 3 + 1];
            int index2 = mesh.triangles[i * 3 + 2];

            VertexInfo vertex0 = newVertexInfos[index0];
            VertexInfo vertex1 = newVertexInfos[index1];
            VertexInfo vertex2 = newVertexInfos[index2];

            if (vertex0.above && vertex1.above && vertex2.above)
            {
                mesh1.AddTriangle(vertex0.index, vertex1.index, vertex2.index);
            }else if (!vertex0.above && !vertex1.above && !vertex2.above)
            {
                mesh2.AddTriangle(vertex0.index, vertex1.index, vertex2.index);
            }
            else
            {
                //切三角形
                if (vertex1.above == vertex2.above)
                {
                    if (vertex0.above == true)
                    {
                        SplitTriangle(mesh, mesh1, mesh2, ref localPanelNormal, ref localPanelPoint, newVertexInfos, index0, index1, index2);
                    }
                    else
                    {
                        SplitTriangle(mesh, mesh2, mesh1, ref localPanelNormal, ref localPanelPoint, newVertexInfos, index0, index1, index2);
                    }
                }
                else if (vertex0.above == vertex2.above)
                {
                    if (vertex1.above == true)
                    {
                        SplitTriangle(mesh, mesh1, mesh2, ref localPanelNormal, ref localPanelPoint, newVertexInfos, index1, index2, index0);
                    }
                    else
                    {
                        SplitTriangle(mesh, mesh2, mesh1, ref localPanelNormal, ref localPanelPoint, newVertexInfos, index1, index2, index0);
                    }
                }
                else
                {
                    if (vertex2.above == true)
                    {
                        SplitTriangle(mesh, mesh1, mesh2, ref localPanelNormal, ref localPanelPoint, newVertexInfos, index2, index0, index1);
                    }
                    else
                    {
                        SplitTriangle(mesh, mesh2, mesh1, ref localPanelNormal, ref localPanelPoint, newVertexInfos, index2, index0, index1);
                    }
                }
            }
        }

        mesh1.Combine();
        mesh2.Combine();
    }

    private void SplitTriangle(Mesh mesh,
                            MeshBox meshUp, 
                            MeshBox meshDown, 
                            ref Vector3 localPanelNormal, 
                            ref Vector3 localPanelPoint, 
                            VertexInfo[] newVertexInfos, 
                            int indexUp, 
                            int indexDown0, 
                            int indexDown1)
    {
        Vector3 v0 = mesh.vertices[indexUp];
        Vector3 v1 = mesh.vertices[indexDown0];
        Vector3 v2 = mesh.vertices[indexDown1];

        float svtop = Vector3.Dot(localPanelPoint - v0, localPanelNormal);
        float sv01 = Mathf.Clamp01(svtop / Vector3.Dot(v1 - v0, localPanelNormal));
        float sv02 = Mathf.Clamp01(svtop / Vector3.Dot(v2 - v0, localPanelNormal));

        Vector3 v01 = v0 + (v1 - v0) * sv01;
        Vector3 v02 = v0 + (v2 - v0) * sv02;

        Vector2 uv0 = mesh.uv[indexUp];
        Vector2 uv1 = mesh.uv[indexDown0];
        Vector2 uv2 = mesh.uv[indexDown1];

        Vector3 uv01 = uv0 + (uv1 - uv0) * sv01;
        Vector3 uv02 = uv0 + (uv2 - uv0) * sv02;

        Vector3 n0 = mesh.normals[indexUp];
        Vector3 n1 = mesh.normals[indexDown0];
        Vector3 n2 = mesh.normals[indexDown1];

        Vector3 normal01 = (n0 + (n1 - n0) * sv01).normalized;
        Vector3 normal02 = (n0 + (n2 - n0) * sv02).normalized;

        Vector4 t0 = mesh.tangents[indexUp];
        Vector4 t1 = mesh.tangents[indexDown0];
        Vector4 t2 = mesh.tangents[indexDown1];

        Vector4 tangent1 = (t0 + (t1 - t0) * sv01).normalized;
        Vector4 tangent2 = (t0 + (t2 - t0) * sv02).normalized;
        tangent1.w = t1.w;
        tangent2.w = t2.w;

        int index01 = meshUp.AddVertex(v01, uv01, normal01, tangent1);
        int index02 = meshUp.AddVertex(v02, uv02, normal02, tangent2);
        meshUp.AddTriangle(newVertexInfos[indexUp].index, index01, index02);

        index01 = meshDown.AddVertex(v01, uv01, normal01, tangent1);
        index02 = meshDown.AddVertex(v02, uv02, normal02, tangent2);
        meshDown.AddTriangle(newVertexInfos[indexDown0].index, newVertexInfos[indexDown1].index, index02);
        meshDown.AddTriangle(newVertexInfos[indexDown0].index, index02, index01);
    }
}