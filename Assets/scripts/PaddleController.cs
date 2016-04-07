using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {

    public Vector3 left;
    public Vector3 right;
    public float movementSpeed;

    private Vector3 movementUnit;
    //position is kept track of as a float between 0.0f(=left) to 1.0f(=right)
    private float curPos;

	// Use this for initialization
	void Start () {
        movementUnit = right - left;
        //move paddle to the middle
        transform.position = left + (movementUnit / 2f);
        //@TODO: rotate & scale paddle
        movementUnit.Normalize();
	}
	
	// Update is called once per frame
	void Update () {
        float newPos = Mathf.Clamp01(curPos + movementSpeed * Input.GetAxis("Horizontal"));
        transform.position = Vector3.Lerp(left, right, newPos);
        curPos = newPos;
	}
}
