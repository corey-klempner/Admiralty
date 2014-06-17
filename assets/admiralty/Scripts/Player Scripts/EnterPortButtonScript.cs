using UnityEngine;
using System.Collections;

public class enterPortButtonScript : MonoBehaviour {

	// Use this for initialization
	void OnClick() {
		print ("Loading level");
		Application.LoadLevel (1);
	}
}
