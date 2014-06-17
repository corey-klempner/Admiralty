using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Needed for List construct


public class Vessel : MonoBehaviour {
	public static List<Vessel> vesselList = new List<Vessel>();

	public string vesselName {get; set;} 
	public int Id {get; set;} 
	public int cargoHold; 
	public int gunPorts; 
	public int topSpeed; 

	public string prefabName; 
// 	public GameObject prefab; 

	public string info; 

	//TODO: Determine navigation factors:
	//  	rudderFactor
	//		sailsSurfaceArea
	//		crewFactor
	private static int inc = 0;
	public Vessel (string name, int w, int guns, int speed, string i, string prefab){
		this.vesselName = name;
		this.cargoHold = w;
		this.gunPorts = guns;
		this.topSpeed = speed;
		this.info = i; 
		this.prefabName = prefab; 
		this.Id = inc++;

		print(name + " id: " + this.Id);
//		this.prefab = (GameObject) Instantiate

		//print ("list: " + vesselList.Count);
	}



	public Vessel currentVessel;
	
	public Vessel cutter, sloop, schooner, eastIndiaman, frigate6, sotl;


	private PlayerStatistics playerScript; 
	private ThirdPersonCameraNET cameraScript; 
	private GameObject player; 

	void OnLevelWasLoaded(int level) {
		if (level == 1){
			player = GameObject.FindGameObjectWithTag("Player");
			cameraScript = player.GetComponent<ThirdPersonCameraNET>();
			playerScript = player.GetComponent<PlayerStatistics>();		
		}
	}

	//Assign to player character

	void Start(){
		DontDestroyOnLoad(transform);

		/* 
		CUTTER
		*/
		string text = "The Cutter is a small, 10-gun vessel. Like the sloop, it has a single fore-and-aft sail that allows it sail efficiently at beam and broad reach.  The cutter is favoured by pirates for its balance of maneavourability and armament, as well as its ability to sail in shallows and shoals. \n\nBest point of sail: Reaching";
		cutter = new Vessel("Cutter", 40, 10, 16, text, "cutter");
		vesselList.Add(cutter);
		
		/*
		SLOOP
		*/
		text = "What the Sloop lacks in armament, it makes up for in speed and maneavourability. The sloop is one of the fastest fore-and-aft rigged ships, and it's shallow draft allows it to navigate shallows. \n\nBest point of sail: Reaching";
		//sloop = new Vessel("Sloop", 40, 4, 18, text, "sloop");
		

		/*
		SCHOONER
		*/
		text = "The schooner is a popular merchant vessel because of it's versatility. Schooners are fore-and-aft rigged with two masts. Schooners can navigate shallow rivers and are often speedy enough to avoid trouble.\n\nBest point of sail: Reaching";
		//schooner = new Vessel("Schooner", 80, 10, 14, text, "schooner");

		/*
		EAST INDIAMAN
		*/
		text = "The East Indiaman is a large merchant vessel that resembles a highly armed warship. They have the largest cargo hold and a modest armament to defend it.  These vessels are highly prized targets, but should not be taken lightly. \n\nBest point of sail: Down wind";
		eastIndiaman = new Vessel("East Indiaman", 400, 40, 6, text, "eastIndiaman");
		vesselList.Add(eastIndiaman);

		/*
		FRIGATE 6
		*/
		text = "The Frigate is a large, shallow-draft vessel originating from the 1750s. A Sixth Rate Frigate typically carries from 30 to 40 guns on a single deck. The full-rigged sails allow it to achieve a high speed. \n\nBest point of sail: Down wind";
		//frigate6 = new Vessel("Sixth Rate Frigate", 140, 30, 9, text, "frigate6");

		/*
		SHIP OF THE LINE 
		*/
		text = "The Ship of the Line is the king of the ocean. These behemoths are marvels of technology and oak several feet thick, sheilding hundreds of large cannons. Due to it's monstrous size, this vessel is very slow and must be wary of shoals and shallows. \n\nBest point of sail: Down wind"; 
		//sotl = new Vessel("Ship of the Line", 280, 120, 4, text, "shipoftheline");
	}

	/*

	public static void selectVessel(string type){
		switch(type){
			case "cutter":
				selectCutter();
				break;
			case "sloop":
				selectSloop();
				break;
			case "schooner":
				selectSchooner();
				break;
			case "eastIndiaman":
				selectEastIndiaman();
				break;
			case "frigate6":
				selectFrigate6();
				break;
			case "sotl":
				selectSOTL();
				break;
		}

	}
*/


}
//TODO: Fix the LIST FIND method

	
