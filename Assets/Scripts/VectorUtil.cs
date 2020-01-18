using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Util
{
    public static void SetY(this Vector3 vect, float y)
    {
        vect.Set(vect.x, y, vect.z);
    }

    public static Vector3 ModY(this Vector3 vect, float y)
    {
        return new Vector3(vect.x, y, vect.z);
    }

    public static Vector3 ModZ(this Vector3 vect, float z)
    {
        return new Vector3(vect.x, vect.y, z);
    }
}
