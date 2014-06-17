using UnityEngine;
using System.Collections;


public class CannonScript : MonoBehaviour 
{
	private enum Projectile {RoundShot, ChainShot, GrapeShot};

	private int	AIM_OFF = 0;

	// public
	public float projMuzzleVelocity = 450; // in metres per second
	public float RateOfFire = 3.0f;
	public float Inaccuracy = 0.1f;
	public int fireNow = 0;
	
	//public float arc = 
	public float elevationDegrees = 9.0f;   

	private bool loaded; 
	private GameObject instantiatedSmoke;
	private GameObject instantiatedProjectile;
	// Use this for initialization

	PlayerStatistics playerScript; 

	public string bank = "port"; //what side of the ship this gun is on

	private GameObject icon; //This is the game icon cylinder on GUI
	private int iconState; //What color the icon is.  0 - empty(red), 2- loading(orange), 3 - loaded(green), 4 - destroyed (black)

	private Material iconRed;
	private Material iconGreen;

	private GameObject GUIIconAnchor; 

	private bool debugAim = true; 
	public bool isAiming;

	Vector3 pos;

	private LineRenderer lineRenderer; 

	ThirdPersonCameraNET cameraScript; 


	Projectile nextShot; //= Projectile.RoundShot; //What to load next
	Projectile currentShot; // = Projectile.RoundShot;  //What is currently loaded


	void Start()
	{
		cameraScript = gameObject.transform.parent.parent.parent.GetComponent<ThirdPersonCameraNET>();


		GameObject lineRendererObject = new GameObject("Cannon Direction Vector");

		lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
		lineRenderer.transform.parent = transform;

		lineRenderer.material.color = Color.red;
		lineRenderer.SetColors(Color.red, Color.red);
		lineRenderer.SetWidth(0.01F, 0.01F);
		lineRenderer.SetVertexCount(2);
	
		loaded = true;

		//need to go two parents up to get player object
		playerScript = gameObject.transform.parent.parent.parent.GetComponent<PlayerStatistics>();

		playerScript.countGuns (); //Add this gun to list
		icon = (GameObject) Instantiate (Resources.Load("cannonIconPrefab"));
		icon.transform.parent = transform;

		icon.transform.Rotate (new Vector3 (0, 0, 90));
		//Position icon on screen
		pos = new Vector3(playerScript.getIndicatorPosition(bank), 10, 1);


		/*
		 *  To allow scene loading, do not destroy these objects
		 */
		DontDestroyOnLoad(icon);
		DontDestroyOnLoad(lineRenderer);
	}



	/*
	 * Whatever the nextShot is, verify that there is 
	 * enough supplies to load it, and react accordingly
	 * 
	 * 
	 */
	 public void reload(){ 
		bool success = false;

		switch (nextShot){		
			case (Projectile.RoundShot):

			if (playerScript.getCargoByName("Round Shot").getQuantity() > 0)
				{
				print ("Loading round shot");
					Invoke ("loadCannons", 1 / RateOfFire);
					playerScript.getCargoByName("Round Shot").remove(1);
					success=true;
				}else{
					print ("No Round shot ammunition left. Load somethinge else");
				}
				break;
			
			case (Projectile.ChainShot):
			if (playerScript.getCargoByName("Chain Shot").getQuantity() > 0)
				{
				
				print ("Loading chain shot");
					Invoke ("loadCannons", 1 / RateOfFire);
					playerScript.getCargoByName("Chain Shot").remove(1); 
					success = true;
				}else{
					print ("No Chain Shot ammunition left");
				}
				break;

				
			case (Projectile.GrapeShot):
				if (playerScript.getCargoByName("Grape Shot").getQuantity() > 0)
				{
					Invoke ("loadCannons", 1 / RateOfFire);
					playerScript.getCargoByName("Grape Shot").remove(1);
					success = true;
				}else{
					print ("No grape shot ammunition left");
				}
				break;
		}

		if (success)
		{
			currentShot = nextShot;
	
		}
	}
	
	void loadCannons (){
		loaded = true; 

		//Change colour of indicator to
		//Loaded and ready to fire!
		iconState = 3;
		icon.renderer.material.color = Color.green;
	}

	void fireCannon(){

		if (loaded) {
			loaded = false; 

			//Change the HUD icon for this cannon
			iconState = 2; 
			icon.renderer.material.color = Color.red;

			//Assign velocity vector to the projectile
			//Inaccuracy to randomize the direction and speed
			Vector3 muzzlevelocity = transform.forward;
			if (Inaccuracy != 0) {
				Vector3 rand = Random.insideUnitSphere;
				muzzlevelocity += new Vector3 (rand.x, rand.y, rand.z) * Inaccuracy;
			}
			muzzlevelocity = muzzlevelocity.normalized * projMuzzleVelocity; 


			//Fire what is currently loaded
			switch (currentShot){
				case Projectile.RoundShot:
				/*
			 	*  Instantiate Round Shot
			 	*/
					instantiatedProjectile = PhotonNetwork.Instantiate ("CannonballRound", transform.position, transform.rotation, 0);
					instantiatedProjectile.GetComponent<ProjectileScript> ().muzzleVelocity = muzzlevelocity;
					break;
				case Projectile.ChainShot:
				/*
				 *  Instantiate Chain Shot
				 */
					instantiatedProjectile = PhotonNetwork.Instantiate ("CannonballChainShot", transform.position, transform.rotation, 0);
					instantiatedProjectile.GetComponent<ProjectileScript> ().muzzleVelocity = muzzlevelocity;
					break;
				case Projectile.GrapeShot:
					break;
			}

			//Create the muzzle blast 
			instantiatedSmoke = PhotonNetwork.Instantiate ("cannonSmoke", transform.position, transform.rotation, 0);

			//Clean up projectiles
			Invoke ("DestroyObjects", 5.0f);

			reload();
		}
	}

	public void Update(){

		if (debugAim)
			Debug.DrawLine (transform.position, transform.position + transform.forward * 10, Color.green);


		/*
		 * Do what is done for the reload - make FireCannon public
		 */
		if (fireNow == 1)
		{
			fireNow = 0;
			//Create a random delay in firing. 
			//For Man of wars, need more sophisiticated script
			//as cannons often follow in semi-succession 

			float primerdelay = Random.Range (0,3);
			Invoke("fireCannon", primerdelay+1);
		}


	}

	void LateUpdate()
	{	
		/*
		 * Position this cannon's icon on the screen
		 *     Port scenes do not have a main camera, only world scenes
		 */

		 if (cameraScript.camera)
			icon.transform.position = cameraScript.camera.ScreenToWorldPoint (pos);

		/*
		 * Draw the aim lines protruding from the cannon
		 */
		if (cameraScript.selectCamera != AIM_OFF){
			lineRenderer.SetPosition (0, transform.position);
			lineRenderer.SetPosition(1, transform.position + transform.forward * 1000);
		}else{
			//Hide the line when not aiming
			lineRenderer.SetPosition (0, transform.position);
			lineRenderer.SetPosition (1, transform.position);
		}
	}

	/*
	 * Learn interfaces
	 */
	public void setNextShot(int ammoType)
	{
		switch(ammoType){
		case 1: 
			nextShot = Projectile.RoundShot;
			break;
		case 2: 
			nextShot = Projectile.ChainShot;
			break;
		case 3: 
			nextShot = Projectile.GrapeShot;
			break;
		}
	}


	public void DestroyObjects()
	{
		if (instantiatedSmoke)
			PhotonNetwork.Destroy (instantiatedSmoke);
		
		if (instantiatedProjectile)
			PhotonNetwork.Destroy (instantiatedProjectile);
	}
}