using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomSyncPanel : MonoBehaviour
{
    public Transform RoomParent;

    public Transform SyncRoom;

    public Transform SyncPanel;

    public Transform SpatialAnchorPanel;

    public Text SpatialAnchorLog;

    public OVRManager OVRManager;

    private Vector3 _originRotation;

    void Awake()
    {
        Application.logMessageReceived += ApplicationOnlogMessageReceived;
    }

    // Start is called before the first frame update
    void Start()
    {
        SyncRoom.gameObject.SetActive(true);
        OVRManager.GetComponent<OVRPassthroughLayer>().enabled = false;
        _originRotation = RoomParent.eulerAngles;
    }

    private void OnEnable()
    {
        SyncRoom.gameObject.SetActive(true);
        OVRManager.GetComponent<OVRPassthroughLayer>().enabled = false;
        _originRotation = RoomParent.eulerAngles;
    }

    private void OnDisable()
    {
        SyncRoom.gameObject.SetActive(false);
        OVRManager.GetComponent<OVRPassthroughLayer>().enabled = true;
    }

    public void PositionRoom()
    {
        var newRoomPosition = Camera.main.transform.position;
        RoomParent.position = new Vector3(newRoomPosition.x, 0, newRoomPosition.z);
    }

    public void RotateRoom()
    {
        var newRoomRotation = new Vector3(0, Camera.main.transform.eulerAngles.y + _originRotation.y, 0);
        RoomParent.eulerAngles = newRoomRotation;
    }

    public void SwitchPassthrough()
    {
        SyncRoom.gameObject.SetActive(!SyncRoom.gameObject.activeSelf);
        OVRManager.GetComponent<OVRPassthroughLayer>().enabled = !OVRManager.GetComponent<OVRPassthroughLayer>().enabled;
    }

    public void ShowSyncPanel()
    {
        SyncPanel.gameObject.SetActive(true);
        SpatialAnchorPanel.gameObject.SetActive(false);
    }

    public void ShowSpatialAnchorsPanel()
    {
        SyncPanel.gameObject.SetActive(false);
        SpatialAnchorPanel.gameObject.SetActive(true);
    }

    public void LoadSpatialAnchor()
    {
        SpatialAnchorManager.Instance.LoadAnchor();
    }

    public void SaveSpatialAnchor()
    {
        SpatialAnchorManager.Instance.SaveAnchorLocally();
    }

    public void DeleteSpatialAnchor()
    {
        SpatialAnchorManager.Instance.EraseAnchorLocally();
    }

    private void ApplicationOnlogMessageReceived(string condition, string stacktrace, LogType type)
    {
        if (condition.Contains("[ListeningTestLog]"))
            SpatialAnchorLog.text += condition + "\n\r";
    }
}
