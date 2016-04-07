using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class DrawGrid : MonoBehaviour {

    public Material lineColor;
    public float nrLines;

    private Camera cam;

    public float wave;
    public float waveSpeed;
    public float ywave;
    public float ywaveSpeed;
    public float zoom;
    public float zoomSpeed;

    private Vector3 startPos;
    private float startZoom;


    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
        startPos = transform.position;
        startZoom = cam.fieldOfView;
	}

    void LateUpdate()
    {
        float newx = startPos.x + wave * Mathf.Sin(Time.time * waveSpeed);
        float newz = startPos.z + wave * Mathf.Cos(Time.time * waveSpeed);
        float newy = startPos.y + ywave * Mathf.Cos(Time.time * ywaveSpeed);
        transform.position = new Vector3(newx, newy, newz);

        cam.fieldOfView = startZoom + zoom * Mathf.Cos(Time.time * zoomSpeed);

        //cam.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, -3f)-transform.position);
    }
	
	// Update is called once per frame
	void OnPostRender () {
        //Debug.Log("drawing line");
        lineColor.SetPass(0);
        GL.PushMatrix();
        GL.LoadProjectionMatrix(cam.projectionMatrix);
        GL.Begin(GL.LINES);
        for (float x=-nrLines; x<nrLines; x+= 1f)
        {
            //Debug.DrawLine(new Vector3(x, 0f, -nrLines), new Vector3(x, 0f, nrLines), lineColor.color);
            //Debug.DrawLine(new Vector3(-nrLines, 0f, x), new Vector3(nrLines, 0f, x), lineColor.color);
            GL.Vertex3(x, 0f, -nrLines);
            GL.Vertex3(x, 0f, nrLines);
            GL.Vertex3(-nrLines, 0f, x);
            GL.Vertex3(nrLines, 0f, x);
        }
        GL.PopMatrix();

    }
}
