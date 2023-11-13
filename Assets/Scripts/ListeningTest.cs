using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ListeningTest : MonoBehaviour
{
    public UnityEvent ListeningTestFinished;

    public string ListeningTestName;
    public Transform AudioSources;
    public ListeningTestMarker MarkerPrefab;
    public SimpleGazeCursor GazeCursor;

    private AudioSource[] _audioSources;
    private AudioSource _currentAudioSource;

    private List<ListeningTestMarker> _markers;
    private ListeningTestMarker _currentMarker;
    private ListeningTestData _listeningTestData;

    private float _currentAudioSourceStartedTime;
    private int _listeningId;
    private bool _isRunning;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartListeningTest()
    {
        _listeningTestData = new ListeningTestData(CreateTestIdentifier());
        _markers = new List<ListeningTestMarker>();
        GazeCursor.gameObject.SetActive(true);

        _listeningId = 0;
        _audioSources = ShuffleArray(AudioSources.GetComponentsInChildren<AudioSource>(true).ToArray());
        _currentAudioSourceStartedTime = Time.realtimeSinceStartup;

        _currentAudioSource = _audioSources[_listeningId];
        _currentAudioSource.gameObject.SetActive(true);
        _isRunning = true;
    }

    private void EndListeningTest()
    {
        _isRunning = false;
        ListeningTestFinished.Invoke();

        _currentMarker.gameObject.SetActive(false);
        _currentAudioSource.gameObject.SetActive(false);
        _currentMarker = null;
        foreach (var listeningTestMarker in _markers)
        {
            Destroy(listeningTestMarker.gameObject);
        }

        _markers.Clear();
        _listeningId = 0;
        GazeCursor.gameObject.SetActive(false);
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!_isRunning)
            return;

        if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.8f)
        {
            if (_currentMarker == null)
            {
                if (GazeCursor.IsHitting)
                {
                    _currentMarker = Instantiate(MarkerPrefab);
                }
            }

            if (_currentMarker != null && GazeCursor.IsHitting)
            {
                _currentMarker.transform.position = GazeCursor.CurrentCursorPosition;
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            if (_currentMarker != null)
            {
                NextAudioSource();
            }
        }
    }

    private void NextAudioSource()
    {
        _listeningId++;

        if (_listeningId >= _audioSources.Length)
        {
            EndListeningTest();
            return;
        }

        var time = Time.realtimeSinceStartup - _currentAudioSourceStartedTime;
        _listeningTestData.AddDataPair(new ListeningTestDataPair(_currentAudioSource.name, time, _currentAudioSource.transform.position, _currentMarker.transform.position));
        _markers.Add(_currentMarker);
        _currentMarker.gameObject.SetActive(false);
        _currentMarker = null;

        _currentAudioSource.gameObject.SetActive(false);
        _currentAudioSourceStartedTime = Time.realtimeSinceStartup;

        _currentAudioSource = _audioSources[_listeningId];
        _currentAudioSource.gameObject.SetActive(true);
    }

    private string CreateTestIdentifier()
    {
        var time = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        return $"{ListeningTestName}_{time}";
    }

    private T[] ShuffleArray<T>(T[] array)
    {
        T tempGO;

        for (int i = 0; i < array.Length; i++)
        {
            int rnd = Random.Range(0, array.Length);
            tempGO = array[rnd];
            array[rnd] = array[i];
            array[i] = tempGO;
        }

        return array;
    }

    public void WriteTestResults()
    {
        var success = false;
        try
        {
            var testData = JsonUtility.ToJson(_listeningTestData);
            var fileName = _listeningTestData.Identifier + ".json";
            var path = Path.Combine(Application.persistentDataPath, ListeningTestName);
            var filePath = Path.Combine(path, fileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(fileName))
            {
                Log("File already exists! Not possible..");
                return;
            }

            var file = File.CreateText(filePath);
            file.WriteLine(testData);
            file.Close();

            success = true;
        }
        catch (Exception ex)
        {
            Log(ex.Message);
        }
    }

    private static void Log(string message) => Debug.Log($"[ListeningTestLog]: {message}");
}
