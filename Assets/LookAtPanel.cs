using UnityEngine;

public class LookAtPanel : MonoBehaviour
{
    public float Distance = 2f;

    private void Awake()
    {
        OVRManager.InputFocusAcquired += OVRManagerOnInputFocusAcquired;
        OVRManager.HMDMounted += OVRManagerOnHMDMounted;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.8f)
        {
            ResetPosition();
        }
    }

    private void OVRManagerOnHMDMounted()
    {
        ResetPosition();
    }

    private void OVRManagerOnInputFocusAcquired()
    {
        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * Distance;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up);
    }
}
