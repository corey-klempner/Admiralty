using UnityEngine;
using System.Collections;

public class MastHitboxScript : MonoBehaviour {
	
	PlayerStatistics playerScript;
	
	void OnApplicationLoadLevel(int level){
		if (level == 0){
			playerScript = transform.parent.parent.GetComponent<PlayerStatistics>();
		}
	}
	
	
	public void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == ("cannonball")){
			
			ProjectileScript ps = other.GetComponent<ProjectileScript>();


			/*
			 * Calculate damage multiplier 
			 * 
			 * - For masts, chain shot is 2x. grape shot is 0.5x. 
			 * - For crew, chain shot is 2x. grape shot is 0.5x. 
			 * - For masts, chain shot is 2x. grape shot is 0.5x. 
			 */

			if (ps == null)
				print ("SCRIPT NOT FOUND ON PROJECTILE");
			
			playerScript.damageMasts(ps.baseDamage);
			
			print ("Our mast has been hit by a cannonball: " + ps.baseDamage + " damage");
		}
	}
	
	/*
	* Use Trigger Enter to start the timer.  The timer stops when the same object 
	* exits the trigger. The length of time factors the amount of damage done
	* so that raking bow-stern/stern-bow will do more damage than a broadside
	* hit. 
	*/
}
