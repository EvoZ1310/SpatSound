using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ListeningTestDataPair
{
    public string Name;
    public float TimeNeeded;
    public Vector3 Origin;
    public Vector3 Marker;

    public ListeningTestDataPair(string name, float timeNeeded, Vector3 origin, Vector3 marker)
    {
        Name = name;
        TimeNeeded = timeNeeded;
        Origin = origin;
        Marker = marker;
    }
}
