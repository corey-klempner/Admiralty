using UnityEngine;
using System.Collections;

public class BatteryScript : MonoBehaviour {

	public int health = 10000;
	public GameObject cannonFolder;


	private int aggression = 0; //Passive
						//	1  Aggressive

	void fireCannon(){
		foreach (Transform trans in cannonFolder.transform) {
			trans.gameObject.GetComponent<BatteryCannonScript>().fireNow = 1; 
		}

	}

	public void Update(){

		if (aggression == 1)
					fireCannon ();
	}



	public void OnCollisionEnter(Collision collision)
	{
		//print ("Fort is Aggressive ");

		aggression = 1;

	}
}