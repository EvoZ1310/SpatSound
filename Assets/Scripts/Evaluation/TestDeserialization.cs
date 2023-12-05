using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestDeserialization : MonoBehaviour
{
    public Transform OriginPoints;

    public string Folder; 

    // Start is called before the first frame update
    void Start()
    {
        var testData = new List<ListeningTestData>();
        if (Directory.Exists(Folder))
        {
            var files = Directory.GetFiles(Folder, @"*.json", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                testData.Add((ListeningTestData)JsonUtility.FromJson(File.ReadAllText(file), typeof(ListeningTestData)));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        var normalizedVOld = OriginPoints.GetChild(2).position - OriginPoints.GetChild(1).position;
        var normalizedVNew = NewPoints.GetChild(2).position - NewPoints.GetChild(1).position;

        var centerOld = Vector3.zero;
        TestCube.position = centerOld - OriginPoints.GetChild(1).position;

        var degrees = Vector3.SignedAngle(normalizedVOld.normalized, normalizedVNew.normalized, Vector3.up);
        TestCube.eulerAngles = Vector3.zero;
        TestCube.RotateAround(centerOld, Vector3.up, degrees);

        TestCube.position = TestCube.position + NewPoints.GetChild(1).position;
    }

}
