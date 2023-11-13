using UnityEngine;

public class RingRotator : MonoBehaviour
{
    public Transform Ring;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ring.Rotate(Vector3.up, 0.5f);
    }
}
