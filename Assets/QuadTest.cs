using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTest : MonoBehaviour
{
    Mesh mesh;
    Vector3[] verts;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        verts = mesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] *= 1.5f;
            }

            mesh.vertices = verts;
            mesh.RecalculateBounds();
        }
    }
}
