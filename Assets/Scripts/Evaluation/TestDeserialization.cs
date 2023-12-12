using CSVFile;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

public class SPSSDataSet
{
    public int UserIdentifier;
    public float DistanceMean;
    public float LongDistanceMean;
    public float ShortDistanceMean;
    public float AzimuthMean;
    public float LongAzimuthMean;
    public float ShortAzimuthMean;
    public float Localization_Accuracy;
    public float Voice_Naturalness;
    public float Location_Difficulty;
    public float TimeNeededMean;
    public string Spatializer;
}

public class SPSSPlottingDataSet
{
    public int UserIdentifier;
    public string SampleID;
    public float X_Origin;
    public float Y_Origin;
    public float X_Marker;
    public float Y_Marker;
    public float DistanceDiff;
    public float AzimuthDiff;
    public float TimeNeeded;
    public string Spatializer;
}

public class SPSSPlottingMeanDataSet
{
    public string SampleID;
    public float X;
    public float Y;
    public float DistanceDiff;
    public float AzimuthDiff;
    public float TimeNeeded;
    public string Spatializer;
    public int IsOrigin;
}

public class QuestionaireDataSet
{
    public class SpatializerDataSet
    {
        public float Localization_Accuracy;
        public float Voice_Naturalness;
        public float Location_Difficulty;
    }

    public int UserIdentifier;
    public SpatializerDataSet SteamDataSet;
    public SpatializerDataSet MetaDataSet;
    public SpatializerDataSet MSAcousticDataSet;
    public SpatializerDataSet DearVRDataSet;
}

public static class DataSetContstants
{
    public const string STEAM_SPATIALIZER = "Steam";
    public const string DEARVR_SPATIALIZER = "dearVR";
    public const string META_SPATIALIZER = "Meta";
    public const string MSACOUSTIC_SPATIALIZER = "MSAcoustic";
}

public class TestDeserialization : MonoBehaviour
{
    public Transform OriginPoints;

    public string DearVRFolder;
    public string SteamFolder;
    public string MSAcousticFolder;
    public string MetaFolder;
    public string QuestionnairPath;
    public string OutputPath;

    // Debugging
    //public Transform NewA01;
    //public Transform NewA02;
    //public Transform OldA01;
    //public Transform OldA02;
    //public Transform NewCenter;
    //public Transform NewCenter1;

    // Start is called before the first frame update
    void Start()
    {
        // DearVR
        var dearVRTestData = GetTestData(DearVRFolder);
        var steamData = GetTestData(SteamFolder);
        var msData = GetTestData(MSAcousticFolder);
        var metaData = GetTestData(MetaFolder);
        var qDataSet = GetQuestionairDataSet();

        var spssDataSetList = new List<SPSSDataSet>();
        var spssPlottingDataSet = new List<SPSSPlottingDataSet>();

        foreach (var testEntry in dearVRTestData)
        {
            spssDataSetList.Add(CreateSPSSDataSet(testEntry, qDataSet, DataSetContstants.DEARVR_SPATIALIZER));
            spssPlottingDataSet.AddRange(GenerateSPSSPlottingDataSet(testEntry, DataSetContstants.DEARVR_SPATIALIZER));
        }

        foreach (var testEntry in steamData)
        {
            spssDataSetList.Add(CreateSPSSDataSet(testEntry, qDataSet, DataSetContstants.STEAM_SPATIALIZER));
            spssPlottingDataSet.AddRange(GenerateSPSSPlottingDataSet(testEntry, DataSetContstants.STEAM_SPATIALIZER));
        }

        foreach (var testEntry in msData)
        {
            spssDataSetList.Add(CreateSPSSDataSet(testEntry, qDataSet, DataSetContstants.MSACOUSTIC_SPATIALIZER));
            spssPlottingDataSet.AddRange(GenerateSPSSPlottingDataSet(testEntry, DataSetContstants.MSACOUSTIC_SPATIALIZER));
        }

        foreach (var testEntry in metaData)
        {
            spssDataSetList.Add(CreateSPSSDataSet(testEntry, qDataSet, DataSetContstants.META_SPATIALIZER));
            spssPlottingDataSet.AddRange(GenerateSPSSPlottingDataSet(testEntry, DataSetContstants.META_SPATIALIZER));
        }

        using (var spssDataFile = new StreamWriter(Path.Combine(OutputPath, "spssDataFile.csv")))
        {
            var csvText = CSV.Serialize(spssDataSetList, new CSVSettings { FieldDelimiter = ';' });
            spssDataFile.Write(csvText);
        }

        using (var spssPlottingDataFile = new StreamWriter(Path.Combine(OutputPath, "spssPlottingDataFile.csv")))
        {
            var csvText = CSV.Serialize(spssPlottingDataSet, new CSVSettings { FieldDelimiter = ';' });
            spssPlottingDataFile.Write(csvText);
        }

        var spssPlottingDataMean = GenerateSPSSPlottingDataMean(spssPlottingDataSet);

        using (var spssPlottingDataMeanFile = new StreamWriter(Path.Combine(OutputPath, "spssPlottingDataMeanFile.csv")))
        {
            var csvText = CSV.Serialize(spssPlottingDataMean, new CSVSettings { FieldDelimiter = ';' });
            spssPlottingDataMeanFile.Write(csvText);
        }
    }

    private SPSSDataSet CreateSPSSDataSet(ListeningTestData testEntry, List<QuestionaireDataSet> qDataSet, string spatializer)
    {
        var spssData = CalculateDataSetForUser(testEntry, spatializer);
        AppendQuestionairData(spssData, qDataSet.FirstOrDefault(q => q.UserIdentifier == spssData.UserIdentifier));
        return spssData;
    }

    private void AppendQuestionairData(SPSSDataSet spssData, QuestionaireDataSet qDataSet)
    {
        switch(spssData.Spatializer)
        {
            case DataSetContstants.STEAM_SPATIALIZER:
                spssData.Localization_Accuracy = qDataSet.SteamDataSet.Localization_Accuracy;
                spssData.Location_Difficulty = qDataSet.SteamDataSet.Location_Difficulty;
                spssData.Voice_Naturalness = qDataSet.SteamDataSet.Voice_Naturalness;
                break;
            case DataSetContstants.MSACOUSTIC_SPATIALIZER:
                spssData.Localization_Accuracy = qDataSet.MSAcousticDataSet.Localization_Accuracy;
                spssData.Location_Difficulty = qDataSet.MSAcousticDataSet.Location_Difficulty;
                spssData.Voice_Naturalness = qDataSet.MSAcousticDataSet.Voice_Naturalness;
                break;
            case DataSetContstants.META_SPATIALIZER:
                spssData.Localization_Accuracy = qDataSet.MetaDataSet.Localization_Accuracy;
                spssData.Location_Difficulty = qDataSet.MetaDataSet.Location_Difficulty;
                spssData.Voice_Naturalness = qDataSet.MetaDataSet.Voice_Naturalness;
                break;
            case DataSetContstants.DEARVR_SPATIALIZER:
                spssData.Localization_Accuracy = qDataSet.DearVRDataSet.Localization_Accuracy;
                spssData.Location_Difficulty = qDataSet.DearVRDataSet.Location_Difficulty;
                spssData.Voice_Naturalness = qDataSet.DearVRDataSet.Voice_Naturalness;
                break;
        }
    }

    private SPSSDataSet CalculateDataSetForUser(ListeningTestData testData, string spatializer)
    {
        var data = new SPSSDataSet();
        data.Spatializer = spatializer;
        data.UserIdentifier = testData.UserIdentifier;

        float sumDistanceDiff = 0;
        float sumShortDistanceDiff = 0;
        float sumLongDistanceDiff = 0;
        float sumAzimuthDiff = 0;
        float sumShortAzimuthDiff = 0;
        float sumLongAzimuthDiff = 0;
        float sumTimeNeeded = 0;
        int count = 0;
        int longCount = 0;
        int shortCount = 0;

        // calculate mean azimuth and distance
        var centerPosition = GetCenterPointFromTestData(testData);
        foreach (var result in testData.Results)
        {
            var markerVector = new Vector2(result.Marker.x, result.Marker.z) - centerPosition;
            var originVector = new Vector2(result.Origin.x, result.Origin.z) - centerPosition;
            var distance = markerVector - originVector; // to get negative result if the marker distance was too short
            var angle = Vector2.SignedAngle(markerVector.normalized, originVector.normalized);

            if (originVector.magnitude > 5)
            {
                sumLongDistanceDiff += distance.magnitude;
                sumLongAzimuthDiff += angle;
                longCount++;
            }
            else
            {
                sumShortDistanceDiff += distance.magnitude;
                sumShortAzimuthDiff += angle;
                shortCount++;
            }

            sumDistanceDiff += distance.magnitude;
            sumAzimuthDiff += sumDistanceDiff;
            sumTimeNeeded += result.TimeNeeded;

            count++;
        }

        data.DistanceMean = sumDistanceDiff / count;
        data.ShortDistanceMean = sumShortDistanceDiff / shortCount;
        data.LongDistanceMean = sumLongDistanceDiff / longCount;
        data.AzimuthMean = sumAzimuthDiff / count;
        data.ShortAzimuthMean = sumShortAzimuthDiff / shortCount;
        data.LongAzimuthMean = sumLongAzimuthDiff / longCount;
        data.TimeNeededMean = sumTimeNeeded / count;

        return data;
    }

    private List<SPSSPlottingDataSet> GenerateSPSSPlottingDataSet(ListeningTestData testData, string spatializer)
    {
        var centerNew2d = GetCenterPointFromTestData(testData);
        var centerNew = new Vector3(centerNew2d.x, 0, centerNew2d.y);
        var pointToMove = new GameObject().transform;
        var centerOld = Vector3.zero;
        //var as02Old = OriginPoints.GetComponentsInChildren<SampleIdentifier>().FirstOrDefault(i => i.Name == "AS02");
        //var as02New = testData.Results.FirstOrDefault(p => p.Name == "AS02").Origin;

        var normalizedVOld = GetNormalizedOriginVector("AS01", "AS02");
        var normalizedVNew = GetNormalizedMarkerVector(testData, "AS01", "AS02");
        var degrees = Vector2.SignedAngle(normalizedVOld, normalizedVNew);
        var spssPlotting = new List<SPSSPlottingDataSet>();

        foreach (var dataPoint in testData.Results)
        {
            var originToMove = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            var markerToMove = new GameObject().transform;

            originToMove.position = dataPoint.Origin - centerNew;
            markerToMove.position = dataPoint.Marker - centerNew;
            originToMove.RotateAround(centerOld, Vector3.up, degrees);
            markerToMove.RotateAround(centerOld, Vector3.up, degrees);
            originToMove.position = originToMove.position + centerOld;
            markerToMove.position = markerToMove.position + centerOld;

            var distance = markerToMove.position.magnitude - originToMove.position.magnitude; // to get negative result if the marker distance was too short
            var angle = Vector3.SignedAngle(markerToMove.position, originToMove.position, Vector3.up); // its left when its negative

            spssPlotting.Add(new SPSSPlottingDataSet()
            {
                SampleID = dataPoint.Name,
                Spatializer = spatializer,
                UserIdentifier = testData.UserIdentifier,
                TimeNeeded = dataPoint.TimeNeeded,
                X_Marker = markerToMove.position.x,
                Y_Marker = markerToMove.position.z,
                X_Origin = originToMove.position.x,
                Y_Origin = originToMove.position.z,
                AzimuthDiff = angle,
                DistanceDiff = distance
            });

            Destroy(originToMove.gameObject);
            Destroy(markerToMove.gameObject);
        }

        return spssPlotting;
    }

    private List<SPSSPlottingMeanDataSet> GenerateSPSSPlottingDataMean(List<SPSSPlottingDataSet> full)
    {
        var meanList = new List<SPSSPlottingMeanDataSet>();

        var spatializers = full.GroupBy(e => e.Spatializer);
        foreach (var spat in spatializers)
        {
            var sampleIDs = spat.GroupBy(e => e.SampleID);
            foreach (var sample in sampleIDs)
            {
                var azimuthMean = sample.Sum(e => e.AzimuthDiff) / sample.Count();
                var distanceMean = sample.Sum(e => e.DistanceDiff) / sample.Count();
                var timeNeededMean = sample.Sum(e => e.TimeNeeded) / sample.Count();
                var xMarkerMean = sample.Sum(e => e.X_Marker) / sample.Count();
                var yMarkerMean = sample.Sum(e => e.Y_Marker) / sample.Count();
                var xOriginMean = sample.Sum(e => e.X_Origin) / sample.Count();
                var yOriginMean = sample.Sum(e => e.Y_Origin) / sample.Count();

                meanList.Add(new SPSSPlottingMeanDataSet()
                {
                    Spatializer = spat.Key,
                    SampleID = sample.Key,
                    TimeNeeded = timeNeededMean,
                    X = xMarkerMean,
                    Y = yMarkerMean,
                    AzimuthDiff = azimuthMean,
                    DistanceDiff = distanceMean,
                    IsOrigin = 0
                });
                meanList.Add(new SPSSPlottingMeanDataSet()
                {
                    Spatializer = spat.Key,
                    SampleID = sample.Key,
                    TimeNeeded = timeNeededMean,
                    X = xOriginMean,
                    Y = yOriginMean,
                    AzimuthDiff = azimuthMean,
                    DistanceDiff = distanceMean,
                    IsOrigin = 1
                });
            }
        }

        return meanList;
    }

    private Vector2 GetCenterPointFromTestData(ListeningTestData testData)
    {
        // get all data
        var centerOld = Vector3.zero;
        var centerNew = new GameObject().transform;
        var as01Old = OriginPoints.GetComponentsInChildren<SampleIdentifier>().FirstOrDefault(i => i.Name == "AS01");
        var as01New = testData.Results.FirstOrDefault(p => p.Name == "AS01").Origin;

        // set AS01 as new center
        centerNew.position = centerOld - as01Old.transform.position;
        //NewCenter1.position = centerNew.position;
        //OldA01.position = OldA01.position - as01Old.transform.position;
        //OldA02.position = OldA02.position - as01Old.transform.position;

        // rotate
        var normalizedVOld = GetNormalizedOriginVector("AS01", "AS02");
        var normalizedVNew = GetNormalizedMarkerVector(testData, "AS01", "AS02");
        var degrees = Vector2.SignedAngle(normalizedVNew, normalizedVOld);
        centerNew.RotateAround(centerOld, Vector3.up, degrees);
        //NewCenter1.RotateAround(centerOld, Vector3.up, degrees);
        //OldA01.RotateAround(centerOld, Vector3.up, degrees);
        //OldA02.RotateAround(centerOld, Vector3.up, degrees);

        // add new AS01 position
        centerNew.position = centerNew.position + as01New;
        //NewCenter.position = centerNew.position;
        //Debug.Log("New distance: " + (NewCenter.position - NewA01.position).magnitude);
        //Debug.Log("Old distance: " + (centerOld - as01Old.transform.position).magnitude);

        var center = new Vector2(centerNew.position.x, centerNew.position.z);
        Destroy(centerNew.gameObject);
        return center;
    }

    private List<QuestionaireDataSet> GetQuestionairDataSet()
    {
        var questionaireData = new List<QuestionaireDataSet>();

        var settings = new CSVSettings()
        {
            FieldDelimiter = ';',
            TextQualifier = '\"',
            ForceQualifiers = true,
            LineSeparator = "\n"
        };

        using StreamReader sr = new StreamReader(QuestionnairPath);
        {
            using (CSVReader cr = new CSVReader(sr, settings))
            {
                foreach (string[] line in cr)
                {
                    // 7 = MsAcoustic Localization_Accuracy
                    // 8 = MsAcoustic Voice_Naturalness
                    // 9 = MsAcoustic Location_Difficulty
                    // 11 = Meta Localization_Accuracy
                    // 12 = Meta Voice_Naturalness
                    // 13 = Meta Location_Difficulty
                    // 15 = Steam Localization_Accuracy
                    // 16 = Steam Voice_Naturalness
                    // 17 = Steam Location_Difficulty
                    // 19 = dearVR Localization_Accuracy
                    // 20 = dearVR Voice_Naturalness
                    // 21 = dearVR Location_Difficulty
                    var qDataSet = new QuestionaireDataSet()
                    {
                        UserIdentifier = int.Parse(line[1]),
                        MSAcousticDataSet = new QuestionaireDataSet.SpatializerDataSet
                        {
                            Localization_Accuracy = float.Parse(line[7]),
                            Voice_Naturalness = float.Parse(line[8]),
                            Location_Difficulty = float.Parse(line[9]),
                        },
                        MetaDataSet = new QuestionaireDataSet.SpatializerDataSet
                        {
                            Localization_Accuracy = float.Parse(line[11]),
                            Voice_Naturalness = float.Parse(line[12]),
                            Location_Difficulty = float.Parse(line[13]),
                        },
                        SteamDataSet = new QuestionaireDataSet.SpatializerDataSet
                        {
                            Localization_Accuracy = float.Parse(line[15]),
                            Voice_Naturalness = float.Parse(line[16]),
                            Location_Difficulty = float.Parse(line[17]),
                        },
                        DearVRDataSet = new QuestionaireDataSet.SpatializerDataSet
                        {
                            Localization_Accuracy = float.Parse(line[19]),
                            Voice_Naturalness = float.Parse(line[20]),
                            Location_Difficulty = float.Parse(line[21]),
                        }
                    };
                    questionaireData.Add(qDataSet);
                }
            }
        }

        return questionaireData;
    }

    private List<ListeningTestData> GetTestData(string path)
    {
        var testData = new List<ListeningTestData>();
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, @"*.json", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                var testDataEntry = (ListeningTestData)JsonUtility.FromJson(File.ReadAllText(file), typeof(ListeningTestData));
                testDataEntry.UserIdentifier = int.Parse(filename.Substring(0, filename.IndexOf("_")));
                testData.Add(testDataEntry);
            }
        }
        return testData;
    }

    private Vector2 GetNormalizedOriginVector(string sampleIdFrom, string sampleIdTo)
    {
        var fromPoint = OriginPoints.GetComponentsInChildren<SampleIdentifier>().FirstOrDefault(i => i.Name == sampleIdFrom);
        var toPoint = OriginPoints.GetComponentsInChildren<SampleIdentifier>().FirstOrDefault(i => i.Name == sampleIdTo);
        Debug.DrawLine(fromPoint.transform.position, toPoint.transform.position);
        var normalizedVector = (toPoint.transform.position - fromPoint.transform.position).normalized;

        return new Vector2(normalizedVector.x, normalizedVector.z);
    }

    private Vector2 GetNormalizedMarkerVector(ListeningTestData testData, string sampleIdFrom, string sampleIdTo)
    {
        var fromPoint = testData.Results.FirstOrDefault(p => p.Name == sampleIdFrom);
        var toPoint = testData.Results.FirstOrDefault(p => p.Name == sampleIdTo);
        //NewA01.position = fromPoint.Origin;
        //NewA02.position = toPoint.Origin;
        Debug.DrawLine(fromPoint.Origin, toPoint.Origin);

        var normalizedVector = (toPoint.Origin - fromPoint.Origin).normalized;

        return new Vector2(normalizedVector.x, normalizedVector.z);
    }

    // Update is called once per frame
    void Update()
    {
        //var normalizedVOld = OriginPoints.GetChild(2).position - OriginPoints.GetChild(1).position;
        //var normalizedVNew = NewPoints.GetChild(2).position - NewPoints.GetChild(1).position;

        //var centerOld = Vector3.zero;
        //TestCube.position = centerOld - OriginPoints.GetChild(1).position;

        //var degrees = Vector3.SignedAngle(normalizedVOld.normalized, normalizedVNew.normalized, Vector3.up);
        //TestCube.eulerAngles = Vector3.zero;
        //TestCube.RotateAround(centerOld, Vector3.up, degrees);

        //TestCube.position = TestCube.position + NewPoints.GetChild(1).position;
        //Debug.DrawLine(OriginPoints.GetChild(1).position, OriginPoints.GetChild(0).position, Color.red);
        //Debug.DrawLine(NewA01.position, NewA02.position, Color.blue);
        //Debug.DrawLine(NewCenter1.position, NewCenter.position, Color.green);
        //Debug.DrawLine(Vector3.zero, OriginPoints.GetChild(0).position, Color.red);
        //Debug.DrawLine(NewA01.position, NewCenter.position, Color.blue);
    }

}
