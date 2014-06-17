using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Needed for List construct
using System.Text.RegularExpressions; //Needed for REGEX string to int


public class PlayerStatistics : MonoBehaviour {
	/*
	 * Constants
	 */


	 /*
	 * 	Ship Data
	 */
	 public string vesselName; 

	

	/*
	 *  Hitpoints & Health  {current, maximum}
	 */
	public List<float> hullHitpoints = new List<float>();
	public List<float> rudderHitpoints = new List<float>();
	public List<float> sailsHitpoints = new List<float>();
	public List<float> crewMembers = new List<float>();
	
	/*
	 * Inventory & Cargo
	 */
	public int playerGold = 12000; 

	
	public int cargoCurrentHold; 
	public int cargoHoldSize = 1000; 
	

	private Vector2 scrollPosition = Vector2.zero;

	private bool displayCargo; 
	

	private string cargoInformationBox  = "No information available"; //Displays additional information of cargo when selected


	/**
	 *  Screen Elements - Placing the gun loading state
	 */
	private int totalGuns; 

	//Probably should load all the guns into an array...
	
	private int nextIndicatorPositionPort = Screen.width / 2;
	private int nextIndicatorPositionStarboard = Screen.width / 2;
	//Determines where the next indicator icon should be placed.


	/*
	*  Vessel Damage vectors and vars
	*/
	private GameObject sinkingStart;
	private GameObject sinkingEnd;
	private float sinkingSpeed = 0.2f; 
	private float sinkingStartTime;
	private bool sinkDoOnce; 


	public List <Cargo> playerCargo;

	private string selectedQuantity;

	private UILabel playerStatisticsLabel; 


	private bool isGameWorld = true;

	void Start(){
		vesselName = "Cutter";

		DontDestroyOnLoad(gameObject);

		hullHitpoints.Add(100);
		hullHitpoints.Add(500);

		rudderHitpoints.Add(100);
		rudderHitpoints.Add(150);
		
		sailsHitpoints.Add(100);
		sailsHitpoints.Add(250);
		
		crewMembers.Add(10);
		crewMembers.Add(120);

		GameObject playerStatsObject = GameObject.Find("PlayerStatsLabel");
		
		playerStatisticsLabel = GameObject.Find("playerStatsLabel").GetComponent<UILabel>();
		DontDestroyOnLoad(playerStatsObject);

		playerCargo = new List<Cargo> ();

		Cargo roundShot = new Cargo ("Round Shot", 30, 1);
		Cargo chainShot = new Cargo ("Chain Shot", 40, 1);
		Cargo grapeShot = new Cargo ("Grape Shot", 20, 1);
		Cargo diamonds = new Cargo ("Diamonds", 1, 0);
		Cargo fish = new Cargo ("Fish", 0, 4);
		Cargo coffee = new Cargo ("Coffee", 10, 5);
		Cargo spices = new Cargo ("Spices", 60, 5);
		Cargo tea = new Cargo ("Tea", 0, 5);
		Cargo porcelain = new Cargo ("Porcelain", 0, 5);
		Cargo tobacco = new Cargo ("Tobacco", 0, 5);

		roundShot.setInfo ("Round cannonballs have the furthest range and highest accuracy. They are effective at damaging the hull and rudder.");
		chainShot.setInfo ("Chain shot has medium range. They are best used to try to destroy enemy masts and rigging to disable enemy ships.");
		grapeShot.setInfo ("Grape shot is a very close range and nasty ammunition.  Grape shot is used to maim and kill enemy crew, which is often the fastest way to capture an enemy ship");


		playerCargo.Add (roundShot);
		playerCargo.Add (chainShot);
		playerCargo.Add (grapeShot);
		playerCargo.Add (diamonds);
		playerCargo.Add (fish);
		playerCargo.Add (coffee);
		playerCargo.Add (spices);
		playerCargo.Add (tea);
		playerCargo.Add (porcelain); 
		playerCargo.Add (tobacco);


		/*
		*  Get weight of all items
		*     This should be updated in the Cargo.Add 
		*/
		foreach (Cargo c in playerCargo)
		{
			cargoCurrentHold += c.getTotalWeight();
		}
	}



	public Cargo getCargoByName(string itemName)
	{
		for (int i=0; i<playerCargo.Count; i++)
		{
			if (playerCargo[i].name.Equals(itemName))
			{
				return playerCargo[i];
			}
		}
		return null;
	}




	void Update()
	{
		/*
		 * While this ignores the Model-View-Controller design this is temporary
		 * Need to have one script to handle all user input and delegate
		 * responses to subscripts. 
		 * 
		 * This is just to test and is part of the working prototype
		 */
		if (Input.GetButtonDown ("Inventory")){
			if (displayCargo)
				displayCargo= false;
			else
				displayCargo= true; 
		}

		vesselHealthCheck();
	
	

		/*
		 * Update Player GUIs  

		 TODO: MOVE OUT OF UPDATE()
 		 */ 
		if (isGameWorld)
			playerStatisticsLabel.text = "Hull: " + hullHitpoints[0] + "/" + hullHitpoints[1] + "\nSails: " + sailsHitpoints[0] + "/"+ sailsHitpoints[1] + "\nRudder: " 
				+ rudderHitpoints[0] + "/" + rudderHitpoints[1] + "\n\nRound Shot: " + getCargoByName ("Round Shot").getQuantity() + "\nChain Shot: " + getCargoByName ("Chain Shot").getQuantity() + "\nGrape Shot: " + getCargoByName ("Grape Shot").getQuantity() + "\n\nCargo: " + cargoCurrentHold + "/" + cargoHoldSize;

	}
	
	void OnLevelWasLoaded(int level) {

		if (level == 0)
			isGameWorld=true;
		else
			isGameWorld = false;
		
	}






/*
*	Monitor the health of the vessel - hull, mast, rudder etc.
*	Sink the vessel when hull damaged to 0.
*
*/
	public void vesselHealthCheck(){
		if (hullHitpoints[0] <= 0)
		{
			if (!sinkDoOnce)
			{
				sinkDoOnce = true; 
				sinkingStartTime = Time.time; 
				sinkingStart = new GameObject();
				sinkingEnd = new GameObject();
				sinkingStart.transform.position = gameObject.transform.position; 
				sinkingEnd.transform.position = gameObject.transform.position - new Vector3(0,50,0);

				print ("Set up sinking params: " + sinkingStartTime + ", " + sinkingStart.transform.position + ", " + sinkingEnd.transform.position);
			}
			sinkVessel();
		}

		// Sails Hitpoints

		// Rudder Hitpoints

		// Crew hit points

	}

	private void sinkVessel(){
		print (gameObject.name + " IS SINKING!");
		float distCovered = (Time.time - sinkingStartTime) * sinkingSpeed;
		float sinkingFractional = distCovered / (20);
		gameObject.transform.position = Vector3.Lerp(sinkingStart.transform.position, sinkingEnd.transform.position, sinkingFractional);
	}


	public void showCargo(int id)
	{
		
	
	}

	public void damageHull(float damage)
	{
		hullHitpoints[0] -= damage;
	}

	public void damageSails(float damage)
	{
		sailsHitpoints[0] -= damage;
	}

	public void damageMasts(float damage)
	{
		sailsHitpoints[0] -= damage;
	}

	public void damageRudder(float damage)
	{
		rudderHitpoints[0] -= damage;
	}


	public void countGuns()
	{ //This is called by each cannon object on load. 
		totalGuns++;
	}



	/*
	 * For the cannon loaded state icons at the bottom of the screen, this positions them properly. 
	 */
	public int getIndicatorPosition(string bank)
	{
		int spacing = 25; 

		int nextPosition = 0; 

		if (bank == "port") 
		{
			nextPosition = nextIndicatorPositionPort - spacing;
			nextIndicatorPositionPort = nextPosition;
		}
		else if (bank == "starboard")
		{
			nextPosition = nextIndicatorPositionStarboard + spacing; 
			nextIndicatorPositionStarboard = nextPosition;
		}

		return nextPosition;
	}
}
