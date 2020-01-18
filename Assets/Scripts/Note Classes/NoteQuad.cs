using StyleStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NoteQuad
{
    public float XOffset { get; private set; }
    public float YOffset { get { return (float)height / 2; } }
    public Transform transform { get { return quad.transform; } }

    GameObject quad;
    Mesh mesh;
    Vector3[] verts;
    double height;
    float width;

    float[] yCoords = { -1f, 1f, -1f, 1f };

    public NoteQuad(GameObject _quad)
    {
        quad = _quad;
        mesh = _quad.GetComponent<MeshFilter>().mesh;
        verts = mesh.vertices;
    }

    public void SetVerts(Note parent, Note prevNote)
    {
        var parentBeat = Globals.GetDistAtBeat(parent.BeatLocation);
        var prevBeat = Globals.GetDistAtBeat(prevNote.BeatLocation);    
        height = parentBeat - prevBeat;
        var topNote = parent.Type == NoteType.Shuffle ? prevNote : parent;
        float[] coords =
        {
            (float)Globals.CalcTransX(prevNote, Side.Left), // Lower Left
            (float)Globals.CalcTransX(topNote, Side.Right), // Upper Right
            (float)Globals.CalcTransX(prevNote, Side.Right), // Lower Right
            (float)Globals.CalcTransX(topNote, Side.Left) // Upper Left
        };
        float max = coords.Max();
        float min = coords.Min();
        width = max - min;
        XOffset = max - (width / 2);

        for (int i = 0; i < coords.Length; i++)
        {
            verts[i] = new Vector3(coords[i] - XOffset, (float)height / 2 * yCoords[i]);
        }

        mesh.vertices = verts;
        mesh.RecalculateBounds();
    }

    public void SetActive(bool active)
    {
        quad.SetActive(active);
    }

}
