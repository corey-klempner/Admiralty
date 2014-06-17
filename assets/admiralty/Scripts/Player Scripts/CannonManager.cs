using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
	
	public class CannonManager : MonoBehaviour 
	{
	/*
	 *  Constants
	 */
	private int	AIM_OFF = 0;
	private int AIM_PORT = 1;
	private int AIM_STARBOARD = 2;

	/*
	 * Holds each bank of cannons
	 */
	public GameObject portCannonsFolder;
	public GameObject starboardCannonsFolder; 


	/*
	 * Classes
	 */
	ThirdPersonCameraNET cameraScript; 
	//GameScript gameScript;
//	public GameObject GameScripts; 
	Cargo playerCargo; 

	private bool isRemotePlayer = true;
	private PhotonView myPhotonView;
	private int[] reloadPresses = new int[3];

	public void SetIsRemotePlayer(bool val)
	{
		isRemotePlayer = val;
	}

	void Awake()
	{
		cameraScript = GetComponent<ThirdPersonCameraNET>();
		myPhotonView = GetComponent<PhotonView>();

		//Cargo cargoScript = GameObject.Find ("GameScripts").GetComponent<Cargo>();
		/*
		 * Load Cannons from GetComponent calls into arraylist of gameobjects
		 */
	}

	void OnLevelWasLoaded(int level){
		print ("cannonManager loaded level");
		/*
		Load cannon folders if vehicle changed
		*/
		if (level == 0){
			if ((portCannonsFolder == null) || (starboardCannonsFolder == null)){
				print("Assigning cannon folders");
				foreach (Transform vessel in transform){
					foreach (Transform t in vessel){
						if (t.name == "cannonsPort")
							portCannonsFolder = t.gameObject;
						else if(t.name =="cannonsStarboard")
							starboardCannonsFolder = t.gameObject;
					}
				}

			}
		}
	}


	[RPC]
	void firePortBroadside(){
		foreach (Transform trans in portCannonsFolder.transform) {
			trans.gameObject.GetComponent<CannonScript>().fireNow = 1; 
		}
	}

	[RPC]
	void fireStarboardBroadside(){
		foreach (Transform trans in starboardCannonsFolder.transform) {
			trans.gameObject.GetComponent<CannonScript>().fireNow = 1; 
		}
	}
	

	/*
	*	Make each cannon track the mouse within it's own range of motion
	*/
	void aimCannons(string bank)
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit; 


		Vector3 aimTarget; 

		if (Physics.Raycast (ray, out hit, 1000)){
			aimTarget = hit.point;
		}else{
			/*
		 	* No object is found to aim at, so aim far away
		 	*/
			aimTarget = ray.direction * 100000;
		}


		Quaternion originalRotation; 

		if (bank == "port")	{
			foreach (Transform trans in portCannonsFolder.transform) {
				//print("Cannon: " + trans.eulerAngles);
				//0 degrees. 

				//350 degrees: max elevation
				Vector3 originalRot = trans.eulerAngles;

				trans.LookAt(aimTarget);

				float currentElevation;

				if (trans.eulerAngles.x > 180)
					currentElevation = 360 - trans.eulerAngles.x;
				else
					currentElevation = -1 * trans.eulerAngles.x;

				currentElevation = Mathf.Clamp (currentElevation, -5, 11);

				float clampedElevation; 
				if (currentElevation < 0)
					clampedElevation = -1 * currentElevation;
				else
					clampedElevation = 360 - currentElevation;


				/*
				 * Silly rabbit, cannons cannot traverse!  Freeze horizontal arcs
				 * so the cannons may only be elevated/depressed. 
				 */

				trans.eulerAngles = new Vector3(
					clampedElevation,
					originalRot.y,
					originalRot.z
					);
			}
		}
		else if (bank == "starboard"){
			foreach (Transform trans in starboardCannonsFolder.transform){
				trans.LookAt(aimTarget);
			}
		}
	}
	
	void LateUpdate(){
		if (isRemotePlayer) return;

		/*
			 *  AMMO CHANGE LISTENERS
			 */
		if (Input.GetButton ("SelectRoundShot"))
		{
			//This is the first button press
			if (reloadPresses[0] == 0){
				reloadPresses[0]++;
				reloadNextShot(1);

			//The user has pressed the button a 2nd time 
			} else if (reloadPresses[0] == 2){
				reloadNextShot(1);
				reloadNow(1);
				reloadPresses[0] = 0;
			}

			//Only register button presses, not when held
		}else if (reloadPresses[0] == 1){
			reloadPresses[0]++; 

		}

		if (Input.GetButton ("SelectChainShot"))
		{

			print("SELECT CHAIN SHOT");
			if (reloadPresses[1] == 0){
				reloadPresses[1]++;
				reloadNextShot(2);
				
			} else if (reloadPresses[1] == 2){
				reloadNextShot(2);
				reloadNow(1);
				reloadPresses[1] = 0;
			}
		}else if (reloadPresses[1] == 1){
			reloadPresses[1]++; 
		}
	
	
	/*
		 * Handle firing events.  
		 * When the user holds Q or E, it changes camera to port/starboard
		 * camera and allows the player to aim. 
		 * In this mode, when the player clicks, the guns will fire. 
		 */

		if (!Input.GetButton ("AimPortBroadside") && !Input.GetButton("AimStarboardBroadside"))
			cameraScript.selectCamera = AIM_OFF;

		else
		{

			/*
			 *  AIMING LISTENERS
			 */
			if (Input.GetButton ("AimPortBroadside"))
			{
				//Go into port aiming mode. This pans the camera to broadside view
				cameraScript.selectCamera = AIM_PORT;

				aimCannons("port");

				if (Input.GetButton("FireCannons"))
					this.myPhotonView.RPC("firePortBroadside", PhotonTargets.All);
			}

			if (Input.GetButton ("AimStarboardBroadside"))
			{
				cameraScript.selectCamera = AIM_STARBOARD;

				aimCannons("starboard");

				if (Input.GetButton("FireCannons"))
	 				this.myPhotonView.RPC("fireStarboardBroadside", PhotonTargets.All);
			}
		}
	}

	void reloadNextShot(int type){
		foreach (Transform trans in portCannonsFolder.transform) {
			trans.gameObject.GetComponent<CannonScript>().setNextShot(type); 
		}		
		foreach (Transform trans in starboardCannonsFolder.transform) {
			trans.gameObject.GetComponent<CannonScript>().setNextShot(type); 
		}
	}
	
	void reloadNow(int type){
		print ("Reload: " + type);
		foreach (Transform trans in portCannonsFolder.transform) {
			trans.gameObject.GetComponent<CannonScript>().setNextShot(type);
			trans.gameObject.GetComponent<CannonScript>().reload();
		}
		foreach (Transform trans in starboardCannonsFolder.transform) {
			trans.gameObject.GetComponent<CannonScript>().setNextShot(type); 
			trans.gameObject.GetComponent<CannonScript>().reload();
		}
	}
}




