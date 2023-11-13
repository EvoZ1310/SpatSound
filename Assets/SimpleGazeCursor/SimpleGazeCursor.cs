using UnityEngine;
using System.Collections;
using System;
using System.Net.NetworkInformation;

public class SimpleGazeCursor : MonoBehaviour 
{
    public Transform StartPoint;
    public GameObject CursorPrefab;
    public float MaxCursorDistance = 30;
    public Vector3 CurrentCursorPosition;

    public bool IsHitting;

    private GameObject _cursorInstance;

	// Use this for initialization
	void Start () 
    {
        _cursorInstance = Instantiate(CursorPrefab, transform);
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateCursor(); 
    }

    /// <summary>
    /// Updates the cursor based on what the camera is pointed at.
    /// </summary>
    private void UpdateCursor()
    {
        // Create a gaze ray pointing forward from the camera
        Ray ray = new Ray(StartPoint.position, StartPoint.rotation * Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // If the ray hits something, set the position to the hit point and rotate based on the normal vector of the hit
            _cursorInstance.transform.position = hit.point;
            _cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            CurrentCursorPosition = _cursorInstance.transform.position;
            IsHitting = true;
        }
        else
        {
            IsHitting = false;
            CurrentCursorPosition = Vector3.zero;
            // If the ray doesn't hit anything, set the position to the maxCursorDistance and rotate to point away from the camera
            _cursorInstance.transform.position = ray.origin + ray.direction.normalized * MaxCursorDistance;
            _cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
        }
    }
}
