using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Needed for List construct

public class TradeScript : MonoBehaviour {

	/* List of items in the port's cargo hold */
	public List <Cargo> portCargo;
	/* List of items in the player's cargo hold. Imported from PlayerStatistics */
	public List <Cargo> playerCargo; 

	/* List of tentatively traded items between port and player. */
	public List <Cargo> buyCargo;
	public List <Cargo> sellCargo; 
	private int netProfit, netWeight;  
	


	/*
		Player Scripts
	*/
	PlayerStatistics playerScript; 
	
	/*
		Trade GameObjects
	*/
	public UILabel playerGoldLabel;

	public GameObject tradeBuyPanel;
	public GameObject tradeSellPanel;
	
	public UILabel totalCostLabel;
	public UILabel totalWeightLabel; 

	public GameObject[] buySliders; 
	public GameObject[] sellSliders; //NOT ASSIGNED 


	void Start(){

		playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatistics>();
		if (playerScript == null)
			print ("Player not found");


		/* Player trade items */	
		playerCargo = playerScript.playerCargo; 


		/* Port trade items */
		portCargo = new List<Cargo>();

		Cargo roundShot = new Cargo ("Round Shot", 30, 1);
		Cargo chainShot = new Cargo ("Chain Shot", 40, 1);
		Cargo grapeShot = new Cargo ("Grape Shot", 20, 1);
		Cargo diamonds = new Cargo ("Diamonds", 1, 0);
		Cargo fish = new Cargo ("Fish", 10, 4);
		Cargo coffee = new Cargo ("Coffee", 120, 5);
		Cargo spices = new Cargo ("Spices", 600, 5);
		Cargo tea = new Cargo ("Tea", 20, 5);
		Cargo porcelain = new Cargo ("Porcelain", 30, 5);
		Cargo tobacco = new Cargo ("Tobacco", 50, 5);

		roundShot.setInfo ("Round cannonballs have the furthest range and highest accuracy. They are effective at damaging the hull and rudder.");
		chainShot.setInfo ("Chain shot has medium range. They are best used to try to destroy enemy masts and rigging to disable enemy ships.");
		grapeShot.setInfo ("Grape shot is a very close range and nasty ammunition.  Grape shot is used to maim and kill enemy crew, which is often the fastest way to capture an enemy ship");

		portCargo.Add (roundShot);
		portCargo.Add (chainShot);
		portCargo.Add (grapeShot);
		portCargo.Add (diamonds);
		portCargo.Add (fish);
		portCargo.Add (coffee);
		portCargo.Add (spices);
		portCargo.Add (tea);
		portCargo.Add (porcelain); 
		portCargo.Add (tobacco);


		Cargo roundShot2 = new Cargo ("Round Shot", 0, 1);
		Cargo chainShot2 = new Cargo ("Chain Shot", 0, 1);
		Cargo grapeShot2 = new Cargo ("Grape Shot", 0, 1);
		Cargo diamonds2 = new Cargo ("Diamonds", 0, 1);
		Cargo fish2 = new Cargo ("Fish", 0, 4);
		Cargo coffee2 = new Cargo ("Coffee", 0, 5);
		Cargo spices2 = new Cargo ("Spices", 0, 5);
		Cargo tea2 = new Cargo ("Tea", 0, 5);
		Cargo porcelain2 = new Cargo ("Porcelain", 0, 5);
		Cargo tobacco2 = new Cargo ("Tobacco", 0, 5);

		Cargo roundShot3 = new Cargo ("Round Shot", 0, 1);
		Cargo chainShot3 = new Cargo ("Chain Shot", 0, 1);
		Cargo grapeShot3 = new Cargo ("Grape Shot", 0, 1);
		Cargo diamonds3 = new Cargo ("Diamonds", 0, 1);
		Cargo fish3 = new Cargo ("Fish", 0, 4);
		Cargo coffee3 = new Cargo ("Coffee", 0, 5);
		Cargo spices3 = new Cargo ("Spices", 0, 5);
		Cargo tea3 = new Cargo ("Tea", 0, 5);
		Cargo porcelain3 = new Cargo ("Porcelain", 0, 5);
		Cargo tobacco3 = new Cargo ("Tobacco", 0, 5);

		buyCargo = new List<Cargo>();
		sellCargo = new List<Cargo>();


		buyCargo.Add (roundShot2);
		buyCargo.Add (chainShot2);
		buyCargo.Add (grapeShot2);
		buyCargo.Add (diamonds2);
		buyCargo.Add (fish2);
		buyCargo.Add (coffee2);
		buyCargo.Add (spices2);
		buyCargo.Add (tea2);
		buyCargo.Add (porcelain2); 
		buyCargo.Add (tobacco2); 

		sellCargo.Add (roundShot3);
		sellCargo.Add (chainShot3);
		sellCargo.Add (grapeShot3);
		sellCargo.Add (diamonds3);
		sellCargo.Add (fish3);
		sellCargo.Add (coffee3);
		sellCargo.Add (spices3);
		sellCargo.Add (tea3);
		sellCargo.Add (porcelain3); 
		sellCargo.Add (tobacco3); 

		updateTotals();

		print("All lists are initialized");
		/* Get all the UISliders */


	}


	void updateLabels(){
		totalCostLabel.text = netProfit + " gold";
		if (netProfit>=0) totalCostLabel.color = Color.green;
		else totalCostLabel.color = Color.red;

		totalWeightLabel.text = netWeight + "/" + playerScript.cargoHoldSize + " tons"; 
		playerGoldLabel.text = playerScript.playerGold.ToString();

		foreach (GameObject item in buySliders) {
			string objectName = item.transform.name.Substring(0,item.transform.name.Length - 6);

			Cargo cargoData = getCargoItemByName(buyCargo, objectName);

			if (getCargoItemByName(portCargo, objectName).getQuantity() == 0) {
				item.SetActive(false);
			}else{
				item.SetActive(true);
			}
			//Refresh the grid
			item.transform.parent.GetComponent<UIGrid>().repositionNow = true; 
			

			foreach(Transform t in item.transform) {
				if (t.name.Equals("Cost")){
					t.GetComponent<UILabel>().text = cargoData.quantity*cargoData.value + " gold";

				}else if (t.name.Substring(0, 4).Equals("Port")){
					t.GetComponent<UILabel>().text = (getCargoItemByName(portCargo, objectName).quantity-cargoData.quantity)*cargoData.weight + " tons";
				
				}else if (t.name.Equals("Label")){
					//no op. Bad hack so Substring below doesn't go out of bounds 

				}else if (t.name.Substring(0, 6).Equals("Player")){
					t.GetComponent<UILabel>().text = cargoData.quantity*cargoData.weight + " tons";
				}
		
			}
		}


		foreach (GameObject item in sellSliders) {
			//Get the name (without "Slider")
			string objectName = item.transform.name.Substring(0,item.transform.name.Length - 6);
			Cargo sellCargoData = getCargoItemByName(sellCargo, objectName);

			if (getCargoItemByName(playerCargo, objectName).getQuantity() == 0) {
				item.SetActive(false);
			}else{
				item.SetActive(true);
			}
			//Refresh the grid
			item.transform.parent.GetComponent<UIGrid>().repositionNow = true; 
			

			foreach(Transform t in item.transform) {
				if (t.name.Equals("Cost")){
					t.GetComponent<UILabel>().text = sellCargoData.quantity*sellCargoData.value + " gold";

				}else if (t.name.Equals("PlayerAmount")){
					t.GetComponent<UILabel>().text = (getCargoItemByName(playerCargo, objectName).quantity-sellCargoData.quantity)*sellCargoData.weight + " tons";
			
				}else if (t.name.Equals("TradeAmount")){
					t.GetComponent<UILabel>().text = sellCargoData.quantity*sellCargoData.weight + " tons";
				}
		
			}
		}			
	}

	void updateTotals(){
		netProfit = 0; 
		netWeight = playerScript.cargoCurrentHold; 

		foreach (Cargo i in buyCargo) {
			netProfit -= i.value * i.getQuantity();
			netWeight += i.weight * i.getQuantity();
		}
		foreach (Cargo i in sellCargo) {
			netProfit += i.value * i.getQuantity();
			netWeight -= i.weight * i.getQuantity();
		}

		updateLabels();
	}



	/*
		Action Listeners on BUY sliders
	*/

	void OnBuySliderChangeCoffee(double x){ purchaseGoods("Coffee", x); }
	void OnBuySliderChangeDiamonds(double x){ purchaseGoods("Diamonds", x); }
	void OnBuySliderChangeTea(double x){ purchaseGoods("Tea", x); }
	void OnBuySliderChangePorcelain(double x){ purchaseGoods("Porcelain", x); }
	void OnBuySliderChangeSpice(double x){ purchaseGoods("Spices", x); }
	void OnBuySliderChangeTobacco(double x){ purchaseGoods("Tobacco", x); }
	void OnBuySliderChangeFish(double x){ purchaseGoods("Fish", x); }



	void purchaseGoods(string cargoName, double sv){
		Cargo item = getCargoItemByName(buyCargo, cargoName); 

		Cargo portItem = getCargoItemByName(portCargo, cargoName);

		if (portItem == null) return; 

		int portItemCount = portItem.getQuantity();

		/* Bound the purchasing quantity. Can't buy less than 0, and can't buy 
			  more than player can: a. Afford, b. Carry, c. what port can sell */
		int maxItemByWeight = (playerScript.cargoHoldSize - playerScript.cargoCurrentHold)/item.weight;
		int maxItemByCost = (playerScript.playerGold + netProfit)/item.value; 
	
		
		float limit = 0.0f; 
		if (portItemCount != 0) {
			limit = (float) Mathf.Min(portItemCount, maxItemByWeight, maxItemByCost)/portItemCount; 
		}else{
			print ("ERROR: TRADING ITEMS THAT PORT DOESN'T HAVE");
		}
		
		/* Limit the range of the slider */
		if ((sv <= limit)) {
			/* Determine how much is to be traded. Can't buy less than 0, or more than available number of items
				Because we are buying, we set a negative value for value. */
			item.setQuantity(Mathf.RoundToInt(Mathf.Clamp((float) sv*portItemCount, 0.0f, (float) portItemCount)));

			updateTotals(); 
		}else {
			float sliderLimit = Mathf.Lerp(0.0f, (float) limit, (float) sv);
			getBuyUISlider(item.name).sliderValue = sliderLimit; 
		}
	}



	/*
		Action Listeners on SELL sliders
	*/

	void OnSellSliderChangeCoffee(double x){ sellGoods("Coffee", x); }
	void OnSellSliderChangeDiamonds(double x){ sellGoods("Diamonds", x); }
	void OnSellSliderChangeTea(double x){ sellGoods("Tea", x); }
	void OnSellSliderChangePorcelain(double x){ sellGoods("Porcelain", x); }
	void OnSellSliderChangeSpices(double x){ sellGoods("Spices", x); }
	void OnSellSliderChangeTobacco(double x){ sellGoods("Tobacco", x); }
	void OnSellSliderChangeFish(double x){ sellGoods("Fish", x); }

	void sellGoods(string cargoName, double sv) {
		
		Cargo item = getCargoItemByName(sellCargo, cargoName); 
		Cargo playerItem = getCargoItemByName(playerCargo, cargoName);
		
		if (item == null) return; 
		
		int playerItemCount = playerItem.getQuantity();

		item.setQuantity(Mathf.RoundToInt(Mathf.Clamp((float) sv*playerItemCount, 0.0f, (float) playerItemCount)));
		updateTotals(); 
	}



/*
	Terribly inefficient way to get the slider from an item name
*/
	UISlider getBuyUISlider(string name){
		foreach (GameObject s in buySliders) {
			string objectName = s.transform.name.Substring(0,s.transform.name.Length - 6);

			if (objectName.Equals(name)) {
				foreach (Transform t in s.transform) {
					if (t.name == "Slider") {
						return t.GetComponent<UISlider>();
					}
				}
			}
		}
		return null;
	}



	void OnCompleteButtonPress(){
		playerScript.playerGold += netProfit; 
		print("PURCHASED");
		/*
			TODO: playerScript should determine player net weight
		*/
		playerScript.cargoCurrentHold = netWeight; 

		foreach (Cargo playerItem in playerCargo){
			Cargo buyItem = getCargoItemByName(buyCargo, playerItem.name);
			Cargo sellItem = getCargoItemByName(sellCargo, playerItem.name);
			
			Cargo portItem = getCargoItemByName(portCargo, playerItem.name);

			int netQuantity = buyItem.getQuantity() - sellItem.getQuantity();

			playerItem.setQuantity(playerItem.getQuantity() + netQuantity);
			portItem.setQuantity(portItem.getQuantity() - netQuantity);

			buyItem.setQuantity(0);
			sellItem.setQuantity(0);
		}

		updateLabels();

		/*
			Reset sliders
		*/
		foreach (GameObject s in buySliders) {
			foreach (Transform t in s.transform) {
				if (t.name.Equals("Slider")){
					t.GetComponent<UISlider>().sliderValue = 0;
				}
			}
		}
		foreach (GameObject s in sellSliders) {
			foreach (Transform t in s.transform) {
				if (t.name.Equals("Slider")){
					t.GetComponent<UISlider>().sliderValue = 0;
				}
			}
		}
	}

	public Cargo getCargoItemByName(List<Cargo> list, string itemName)
	{

		//Sometimes the sliders initialize before the actual cargo lists. 
		if (list != null){
				
			
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].name.Equals(itemName))
				{
					return list[i];
				}
			}
		}
		return null;
	}

}

