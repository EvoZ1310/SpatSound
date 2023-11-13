using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;

public class WarmUpController : MonoBehaviour
{
    public UnityEvent Warmup1Completed;
    public UnityEvent Warmup2Completed;
    public UnityEvent Warmup3Completed;

    private int _currentWarmUpPhase;
    private bool _warmUpActive;

    // Start is called before the first frame update
    void Start()
    {
        _currentWarmUpPhase = 0;
        _warmUpActive = false;
    }

    private void OnEnable()
    {
        _currentWarmUpPhase = 0;
        _warmUpActive = false;
    }

    public void ActivateWarmup()
    {
        _warmUpActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_warmUpActive)
        {
            switch (_currentWarmUpPhase)
            {
                case 0:
                    if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.8f)
                    {
                        Warmup1Completed.Invoke();
                        _currentWarmUpPhase = 1;
                    }
                    break;
                case 1:
                    if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.8f)
                    {
                        Warmup2Completed.Invoke();
                        _currentWarmUpPhase = 2;
                    }
                    break;
                case 2:
                    if (OVRInput.GetDown(OVRInput.Button.One))
                    {
                        Warmup3Completed.Invoke();
                        _currentWarmUpPhase = 3;
                    }
                    break;
            }
        }
    }
}
