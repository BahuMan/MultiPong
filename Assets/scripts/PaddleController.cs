using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {

    public Vector3 left;
    public Vector3 right;
    public float movementSpeed;
    public bool listenToKeyboard = true;

    //position is kept track of as a float between 0.0f(=left) to 1.0f(=right)
    private float curPos;

	// Use this for initialization
	void Start () {
        //move paddle to the middle
        curPos = 0.5f;
        transform.position = left + ((right - left) / 2f);
	}
	
	// Update is called once per frame
	void Update () {
        if (!listenToKeyboard) return;

        float newPos = Mathf.Clamp01(curPos + movementSpeed * Input.GetAxis("Horizontal"));
        transform.position = Vector3.Lerp(left, right, newPos);
        curPos = newPos;
	}
}
