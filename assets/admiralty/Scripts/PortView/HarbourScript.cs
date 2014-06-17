using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Needed for List construct



public class HarbourScript : MonoBehaviour {

private const int CUTTER = 0;
private const int EAST_INDIAMAN = 1;

	public UILabel infoLabel; 

	public UILabel currentVessel;
	
	private PlayerStatistics playerScript; 
	private ThirdPersonCameraNET cameraScript; 
	private GameObject player; 

	private List<GameObject> playerHarbour;

	void Start(){
		playerHarbour = new List<GameObject>();

		/*
			Instantiate all the vessels once, to improve performance when
			changing vessels.  

			This way, changing vessels is just assigning this object to the player heirarchy
		*/
		print ("loading Vessels");

	
	}

	void OnLevelWasLoaded(int level) {
		if (level == 1){
			player = GameObject.FindGameObjectWithTag("Player");
			cameraScript = player.GetComponent<ThirdPersonCameraNET>();
			playerScript = player.GetComponent<PlayerStatistics>();
			currentVessel.text = playerScript.vesselName.ToString();			
		}
	}

	void displayCutterInfo(){
		infoLabel.text = Vessel.vesselList.Find(x => x.Id == CUTTER).info;	
	}	

	void displaySchoonerInfo(){
		infoLabel.text = Vessel.vesselList.Find(x => x.vesselName == "Schooner").info;
	}	


	void displayEastIndiamanInfo(){
		infoLabel.text = Vessel.vesselList.Find(x => x.Id == EAST_INDIAMAN).info;
	}

	void displaySOTLInfo(){
		infoLabel.text = Vessel.vesselList.Find(x => x.vesselName == "SOTL").info;
	}

	
	void selectCutter(){
		GameObject oldVessel = player.transform.Find(playerScript.vesselName.ToLower()).gameObject;
		//Destroy(oldVessel);
		oldVessel.transform.parent = null;

		playerScript.vesselName = "Cutter";
		//Vessel.currentVessel = Vessel.vesselList[0];
		GameObject cutter = (GameObject) Instantiate(Resources.Load("cutter"), player.transform.position, player.transform.rotation);

		playerHarbour.Add(cutter);
		cutter.transform.parent = player.transform; 
	}

	void selectSloop(){
		//currentVessel = vesselList[1];
	}


	void selectSchooner(){
		GameObject oldVessel = player.transform.Find(playerScript.vesselName.ToLower()).gameObject;
		Destroy(oldVessel);

		playerScript.vesselName = "Ship of the Line";
		//currentVessel = vesselList[2];
		GameObject newVessel = (GameObject) Instantiate(Resources.Load("shipoftheline"), player.transform.position + new Vector3(10.03514f, 0.0f, 1.8388f), player.transform.rotation);
		newVessel.transform.parent = player.transform; 

		cameraScript.AssignCameraPositions();
	}

	void selectFrigate6(){
		GameObject oldVessel = player.transform.Find(playerScript.vesselName.ToLower()).gameObject;
		Destroy(oldVessel);

		playerScript.vesselName = "Sixth Rate Frigate";
		//currentVessel = vesselList[3];
		GameObject frigate6 = (GameObject) Instantiate(Resources.Load("frigate6"), player.transform.position + new Vector3(10.03514f, 0.0f, 1.8388f), player.transform.rotation);
		frigate6.transform.parent = player.transform; 

		cameraScript.AssignCameraPositions();
	}
	
	void selectEastIndiaman(){
		GameObject oldVessel = player.transform.Find(playerScript.vesselName.ToLower()).gameObject;
		Destroy(oldVessel);

		playerScript.vesselName = "East Indiaman";
		//currentVessel = vesselList[4];
		GameObject eastIndiaman = (GameObject) Instantiate(Resources.Load("eastIndiaman"), player.transform.position + new Vector3(10.03514f, 0.0f, 1.8388f), player.transform.rotation);

		//frigate.transform.parent = player.transform; 
		//playerHarbour[5].transform.position = player.transform.position + new Vector3(10.03514f, 0.0f, 1.8388f);
		//playerHarbour[5].transform.rotation = player.transform.rotation;

		eastIndiaman.transform.parent = player.transform; 
		
		cameraScript.AssignCameraPositions();
		print("Selected EAst Indiaman");
	}
	

	void selectSOTL(){
		GameObject oldVessel = player.transform.Find(playerScript.vesselName.ToLower()).gameObject;
		Destroy(oldVessel);

		playerScript.vesselName = "Ship of the Line";
		//currentVessel = vesselList[5];
		GameObject newVessel = (GameObject) Instantiate(Resources.Load("shipoftheline"), player.transform.position + new Vector3(10.03514f, 0.0f, 1.8388f), player.transform.rotation);
		newVessel.transform.parent = player.transform; 
	
		cameraScript.AssignCameraPositions();	
	}

}
