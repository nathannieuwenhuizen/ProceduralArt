using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Globals
{
    static public float Cap(float val, float min, float max)
    {
        return Mathf.Max(Mathf.Min(max, val), min);
    }
}
