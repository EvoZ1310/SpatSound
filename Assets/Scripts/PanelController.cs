using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public RoomSyncPanel RoomSyncPanel;
    public ListeningTestPanel ListeningTestPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.RawButton.Y) && OVRInput.GetDown(OVRInput.RawButton.B))
        {
            RoomSyncPanel.gameObject.SetActive(true);
            ListeningTestPanel.gameObject.SetActive(false);
        }
    }

    public void OnRoomSyncPanelClose()
    {
        RoomSyncPanel.gameObject.SetActive(false);
        ListeningTestPanel.gameObject.SetActive(true);
    }

    public void OnListeningTestStart()
    {
        ListeningTestPanel.gameObject.SetActive(false);
    }

    public void OnListenteningTestEnded()
    {
        ListeningTestPanel.gameObject.SetActive(true);
        ListeningTestPanel.ShowEnding();
    }
}
