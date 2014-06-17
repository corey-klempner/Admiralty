using UnityEngine;
using System.Collections;

public class BatteryCannonScript : MonoBehaviour 
{
	// public
	public float projMuzzleVelocity; // in metres per second
	public float RateOfFire = 3.0f;
	public float Inaccuracy = 0.1f;
	public int fireNow = 0; 
	
	private bool loaded; 
	private GameObject instantiatedSmoke;
	private GameObject instantiatedProjectile;
	// Use this for initialization

	void Start () 
	{
		loaded = true;

		if (RateOfFire == 0)
						print ("INVALID RATE OF FIRE FOR BATTERY");

	}
	
	void loadCannons (){
		loaded = true; 
	}
	
	void fireCannon(){
		//projMuzzleVelocity = 30; 
		if (loaded) {
			loaded = false; 

			Invoke ("loadCannons", 1 / RateOfFire);
	
			Vector3 muzzlevelocity = transform.forward;
			
			if (Inaccuracy != 0) {
				Vector3 rand = Random.insideUnitSphere;
				muzzlevelocity += new Vector3 (rand.x, rand.y, rand.z) * Inaccuracy;
			}
			
			muzzlevelocity = muzzlevelocity.normalized * projMuzzleVelocity; 

			//instantiatedProjectile = new GameObject();
			instantiatedProjectile = PhotonNetwork.Instantiate ("CannonballRound", transform.position, transform.rotation, 0);
			instantiatedProjectile.GetComponent<ProjectileScript> ().muzzleVelocity = muzzlevelocity;

			instantiatedSmoke = PhotonNetwork.Instantiate ("cannonSmoke", transform.position, transform.rotation, 0);

			Invoke ("DestroyObjects", 5.0f);
			//Cannonball has it's own deletion process
		}
	}
	
	public void DestroyObjects()
	{
		PhotonNetwork.Destroy (instantiatedSmoke);
		PhotonNetwork.Destroy (instantiatedProjectile);
	}
	
	public void Update(){
		
		Debug.DrawLine (transform.position, transform.position, Color.blue);
		
		if (fireNow == 1)
		{
			fireNow = 0;
			//Create a random delay in firing. 
			//For Man of wars, need more sophisiticated script
			//as cannons often follow in semi-succession 
			
			float primerdelay = Random.Range (0,3);
			//print ("delay: " + primerdelay);
			Invoke("fireCannon", primerdelay+1);
		}
	}
}