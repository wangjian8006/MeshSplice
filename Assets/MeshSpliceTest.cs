using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshSpliceTest : MonoBehaviour
{
    private Vector3 start;

    private Ray startRay;

    private Ray endRay;

    public void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            start = Input.mousePosition;
            startRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            endRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 v0 = startRay.origin;
            Vector3 v1 = endRay.origin;
            Vector3 v2 = endRay.origin + 100 * endRay.direction;
            Plane plane = new Plane(v0, v1, v2);

            MeshFilter mf = this.GetComponent<MeshFilter>();
            MeshSplice ms = new MeshSplice();
            ms.Splite(this.transform, mf.mesh, plane);

            GameObject g1 = null;
            GameObject g2 = null;
            if (ms.mesh1.mesh != null)
            {
                g1 = new GameObject();
                g1.AddComponent<MeshFilter>().mesh = ms.mesh1.mesh;
                g1.AddComponent<MeshRenderer>().sharedMaterials = this.GetComponent<MeshRenderer>().sharedMaterials;
                g1.AddComponent<MeshSpliceTest>();
            }
            if (ms.mesh2.mesh != null)
            {
                g2 = new GameObject();
                g2.AddComponent<MeshFilter>().mesh = ms.mesh2.mesh;
                g2.AddComponent<MeshRenderer>().sharedMaterials = this.GetComponent<MeshRenderer>().sharedMaterials;
                g2.AddComponent<MeshSpliceTest>();
            }

            if (g1 != null && g2 != null)
            {
                g1.transform.position = g1.transform.position + plane.normal * 1;
                g2.transform.position = g2.transform.position - plane.normal * 1;
            }

            this.gameObject.SetActive(false);
        }
    }
}