using UnityEngine;
using System.Collections;

public class PongCoordinator : MonoBehaviour {

    public int numPlayers = 5;
    public float radius = 20f;

    public GameObject DynamicWall;
    public Rigidbody ball;

	// Use this for initialization
	void Start () {
        Vector3[] points = CreateEllipse(radius, radius, new Vector3(0f, 0f, 0f), numPlayers*2);
        for (int i=0; i<points.Length-1; ++i)
        {
            createWall(points[i], points[i+1]);
        }
        createWall(points[points.Length-1], points[0]);


    }

    // Update is called once per frame
    void Update () {
	
	}

    public GameObject createWall(Vector3 from, Vector3 to)
    {
        GameObject newWall = Instantiate<GameObject>(DynamicWall);
        newWall.transform.position = (from + to) / 2f;
        newWall.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(from, to));
        newWall.transform.rotation = Quaternion.LookRotation(from-to);
        newWall.name = from.ToString() + " - " + to.ToString();
        //Quaternion.FromToRotation(from, to);
        return newWall;
    }

    private static Vector3[] CreateEllipse(float a, float b, Vector3 center, int resolution)
    {

        Vector3[] positions = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++)
        {
            float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
            positions[i] = new Vector3(a * Mathf.Cos(angle), center.y, b * Mathf.Sin(angle));
            positions[i] = positions[i] + center;
        }

        return positions;
    }
}
