using UnityEngine;
using System.Collections;

public class HarbourTriggerScript : MonoBehaviour {

	private GameObject enterPortPanelObject; 
	private Hashtable properties; 

	void Start(){
		enterPortPanelObject = GameObject.Find ("interactiveButtonsAnchor"); //.GetComponent<UIPanel>();
		//NGUITools.SetActive(enterPortPanelObject,false); 
	}


	void OnTriggerStay (Collider other)
	{
		//if (properties != null) 
		//				PhotonNetwork.room.SetCustomProperties (properties);

		if (Application.isLoadingLevel) {
						print ("LOADING!");

						return;
				}
		if ((other != null) && (other.transform.tag == "Player")){
			print ("Trigger is listening");

			if (Input.GetButton ("Interact"))
			{
				//DontDestroyOnLoad(other.transform.parent.gameObject);

				//properties = new Hashtable(); 
				//properties = PhotonNetwork.room.customProperties;
				//print ("Loading level");
//				Application.LoadLevel (1);
				PhotonNetwork.LoadLevel (1);
								//NGUITools.SetActive(enterPortPanelObject,true);

			}
		}		
	}
	
}