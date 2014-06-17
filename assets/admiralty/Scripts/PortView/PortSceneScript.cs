using UnityEngine;
using System.Collections;

public class PortSceneScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		print("Begin Port Scene!");
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButton ("Interact"))
		{
			LeavePort();
		}
	}

	void LeavePort(){
			PhotonNetwork.LoadLevel(0);
	}
}
