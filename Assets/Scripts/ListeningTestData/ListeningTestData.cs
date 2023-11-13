using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ListeningTestData
{
    public string Identifier;
    public List<ListeningTestDataPair> Results;

    public ListeningTestData(string identifier)
    {
        Results = new List<ListeningTestDataPair>();
        Identifier = identifier;
    }

    public void AddDataPair(ListeningTestDataPair pair)
    {
        Results.Add(pair);
    }
}
