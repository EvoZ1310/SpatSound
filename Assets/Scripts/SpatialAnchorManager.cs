using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialAnchorManager : MonoBehaviour
{
    public static SpatialAnchorManager Instance;
    
    public string UuidKey;
    
    public OVRSpatialAnchor SpatialAnchorPrefab;
    public Transform RoomParent;

    private OVRSpatialAnchor _spatialAnchor;

    void Awake()
    {
        if (!PlayerPrefs.HasKey(UuidKey))
        {
            PlayerPrefs.SetString(UuidKey, "");
        }

        Instance = this;
        //OVRManager.InputFocusAcquired += OVRManagerOnInputFocusAcquired;
        OVRManager.HMDMounted += OVRManagerOnHMDMounted;
    }

    public void SaveAnchorLocally()
    {
        if (IsAnchorAvailable())
        {
            Log("Save: Anchor already saved.. Please erase before saving again!");
            return;
        }

        Log("Save: Instantiate SpatialAnchorPrefab");
        // create new spatial anchor and set it to objects transform
        _spatialAnchor = Instantiate(SpatialAnchorPrefab, RoomParent.position, RoomParent.rotation);

        StartCoroutine(WaitForAnchorCreation());
    }

    private IEnumerator WaitForAnchorCreation()
    {
        while (_spatialAnchor != null && !_spatialAnchor.Created)
        {
            yield return null;
        }

        if (IsAnchorAvailable())
        {
            Log($"Save: Anchor with {UuidKey} will be saved");
            _spatialAnchor.Save((anchor, success) =>
            {
                if (!success)
                {
                    Log($"Save: Saving Anchor wasnt successful");
                    return;
                }

                Log($"Save: Saving was successful with id: {anchor.Uuid}");
                PlayerPrefs.SetString(UuidKey, anchor.Uuid.ToString());
            });
        }
        else
        {
            Log($"Save: Destroying Anchor while saving");
            // Creation must have failed
            Destroy(_spatialAnchor);
        }
    }

    public void EraseAnchorLocally()
    {
        if (IsAnchorAvailable())
        {
            _spatialAnchor.Erase((anchor, success) =>
            {
                if (success)
                {
                    Log("Erase: Erase successful");
                    Destroy(_spatialAnchor);
                }
            });
        }
        else
        {
            Log("Erase: No Anchor erased");
        }
    }

    public bool IsAnchorSaved()
    {
        return PlayerPrefs.GetString(UuidKey) != string.Empty;
    }

    public bool IsAnchorAvailable()
    {
        return _spatialAnchor != null;
    }

    public void LoadAnchor()
    {
        if (IsAnchorAvailable())
        {
            ApplyPosition();
            return;
        }

        if (IsAnchorSaved())
        {
            var currentUuid = new Guid(PlayerPrefs.GetString(UuidKey));
            Log("Load: QueryAnchorByUuid: " + currentUuid);
            var uuids = new List<Guid> { currentUuid };

            Load(new OVRSpatialAnchor.LoadOptions
            {
                MaxAnchorCount = 1,
                Timeout = 0,
                StorageLocation = OVRSpace.StorageLocation.Local,
                Uuids = uuids
            });
        }
        else
        {
            Log($"Load: No anchor id found in player prefs. Save a new anchor first.");
        }
    }

    private void Load(OVRSpatialAnchor.LoadOptions options) => OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
    {
        if (anchors == null || anchors.Length == 0)
        {
            Log("Load: Query failed.");
            return;
        }

        foreach (var anchor in anchors)
        {
            if (anchor.Localized)
            {
                OnLocalized(anchor, true);
            }
            else if (!anchor.Localizing)
            {
                anchor.Localize(OnLocalized);
            }
        }
    });
    
    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success)
        {
            Log($"Load: {unboundAnchor} Localization failed!");
            return;
        }
        
        Log("Load: Instantiate SpatialAnchorPrefab");
        var pose = unboundAnchor.Pose;
        _spatialAnchor = Instantiate(SpatialAnchorPrefab, pose.position, pose.rotation);
        unboundAnchor.BindTo(_spatialAnchor);
        RoomParent.position = pose.position;
        RoomParent.rotation = pose.rotation;
    }

    private void ApplyPosition()
    {
        RoomParent.position = _spatialAnchor.transform.position;
        RoomParent.rotation = _spatialAnchor.transform.rotation;
    }

    private void OnEnable()
    {
        LoadAnchor();
    }

    private void OnApplicationFocus()
    {
        LoadAnchor();
    }

    private void OVRManagerOnInputFocusAcquired()
    {
        LoadAnchor();
    }

    private void OVRManagerOnHMDMounted()
    {
        LoadAnchor();
    }

    private static void Log(string message) => Debug.Log($"[ListeningTestLog]: {message}");
}
