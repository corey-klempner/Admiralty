using UnityEngine;
using System.Collections;

public class ShipwrightScript : MonoBehaviour {

	/*
		Player Scripts
	*/
	PlayerStatistics playerScript; 
	

	public GameObject repairPane; 

	private int repairCostHull, repairCostRudder, repairCostSails;

	private UILabel playerGoldLabel;
	// Use this for initialization
	void Start () {
		playerGoldLabel = GameObject.Find("PlayerGoldValue").GetComponent<UILabel>();

		playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatistics>();
		if (playerScript == null)
			print ("Player not found");

		calculateRepairCosts(); 
		updateLabels();
	}
	
	void calculateRepairCosts(){
		int hullDamage = Mathf.RoundToInt(playerScript.hullHitpoints[1] - playerScript.hullHitpoints[0]);
		int rudderDamage = Mathf.RoundToInt(playerScript.rudderHitpoints[1] - playerScript.rudderHitpoints[0]);
		int sailsDamage = Mathf.RoundToInt(playerScript.sailsHitpoints[1] - playerScript.sailsHitpoints[0]); 

		repairCostHull = hullDamage*10; 
		repairCostRudder = rudderDamage*10; 
		repairCostSails = sailsDamage*10; 
	}
	
	void updateLabels(){
		playerGoldLabel.text = playerScript.playerGold + " gold";

		foreach (Transform t in repairPane.transform){
			switch(t.name){
			
			case "HullButton" :
				t.FindChild("Animation").FindChild("Label").GetComponent<UILabel>().text = "HULL: " + playerScript.hullHitpoints[0] + "/" + playerScript.hullHitpoints[1];
				t.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>().text = repairCostHull + " gold";
				break;
			
			case "RudderButton" :
				t.FindChild("Animation").FindChild("Label").GetComponent<UILabel>().text = "RUDDER: " + playerScript.rudderHitpoints[0] + "/" + playerScript.rudderHitpoints[1];
				t.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>().text = repairCostRudder + " gold";
				break;
			
			case "SailsButton" :
				t.FindChild("Animation").FindChild("Label").GetComponent<UILabel>().text = "SAILS: " + playerScript.sailsHitpoints[0] + "/" + playerScript.sailsHitpoints[1];
				t.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>().text = repairCostSails + " gold";
				break;
			
			case "RecruitButton" :
				t.FindChild("Animation").FindChild("Label").GetComponent<UILabel>().text = "CREW: " + playerScript.crewMembers[0] + "/" + playerScript.crewMembers[1];
				t.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>().text = "XXX gold";
				break;
			}
		}
	}

	void OnRepairHullPress(){
		if (playerScript.playerGold >= repairCostHull){
			playerScript.playerGold -= repairCostHull;
			playerScript.hullHitpoints[0] = playerScript.hullHitpoints[1];
			calculateRepairCosts();
			updateLabels();
		}
	}


	void OnRepairRudderPress(){
		if (playerScript.playerGold >= repairCostRudder){
			playerScript.playerGold -= repairCostRudder;
			playerScript.rudderHitpoints[0] = playerScript.rudderHitpoints[1];
			calculateRepairCosts();
			updateLabels();
		}
	}


	void OnRepairSailsPress(){
		if (playerScript.playerGold >= repairCostSails){
			playerScript.playerGold -= repairCostSails;
			playerScript.sailsHitpoints[0] = playerScript.sailsHitpoints[1];
			calculateRepairCosts();
			updateLabels();
		}
	}


	void OnRecruitCrewPress(){
		print("Hired crew!");
	
		updateLabels();
	}
}
