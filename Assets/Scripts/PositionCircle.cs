using UnityEngine;

[ExecuteInEditMode]
public class PositionCircle : MonoBehaviour
{
    public float Radius = 2;
    public int Segments = 360;

    // Start is called before the first frame update
    void Start()
    {
        ReloadCircle();
    }

    public void ReloadCircle()
    {
        var line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = Segments+1;

        var pointCount = line.positionCount + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / Segments);
            points[i] = new Vector3(Mathf.Sin(rad) * Radius, 0, Mathf.Cos(rad) * Radius);
        }

        line.SetPositions(points);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

