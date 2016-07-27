using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class MPGoalController : NetworkBehaviour {

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "ball") { Debug.Log("collision irrelevant for " + col.gameObject.name); return; }
        Debug.Log("player lost!");
    }

}
