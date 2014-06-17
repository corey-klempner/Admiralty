using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour 
{

	public enum ProjectileTypeEnum { RoundShot, ChainShot, GrapeShot };
	public ProjectileTypeEnum projectile; 

	public Vector3 muzzleVelocity;
	//public GameObject splashPrefab; 
	public float TTL;
	public float baseDamage; 

	public bool isBallistic;
	public float Drag; // in metres/s lost per second.
	
	// Use this for initialization
	void Start () 
	{
		if (TTL == 0)
			TTL = 5;
		//print(TTL);

	}

	void DestroyProjectile()
	{
		PhotonNetwork.Destroy (gameObject);
	}


	// Update is called once per frame
	void Update () 
	{

      // if (!photonView.isMine)
       // {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
          //  transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 5);
            //transform.rotation = Quaternion.Lerp(transform.rotation, correctRot, Time.deltaTime * 5);
        //}else{


			if (transform != null){
				if (Drag != 0)
					muzzleVelocity += muzzleVelocity * (-Drag * Time.deltaTime);
				
				if (isBallistic)

					muzzleVelocity += Physics.gravity * Time.deltaTime;
				
				if (muzzleVelocity == Vector3.zero)
					return;
				else

				transform.position += muzzleVelocity * Time.deltaTime;

				//transform.LookAt(transform.position + muzzleVelocity.normalized);
				Debug.DrawLine(transform.position, transform.position + muzzleVelocity.normalized, Color.red);

				if (projectile == ProjectileTypeEnum.ChainShot)
					transform.Rotate(Vector3.up * 1000.0f * Time.deltaTime);
			}
		//}
	}

	void OnCollisionEnter(Collision other)
	{
		//Only the owner should delete this object!
		if (other.transform.tag == "hull")
			{
				print ("Contact with hull");
				PlayerStatistics enemyScript = other.transform.parent.GetComponent<PlayerStatistics>();
				enemyScript.damageHull(20);
			}

			//PhotonNetwork.Destroy (gameObject);

	}


/*
	Networking info

    private Vector3 correctPos = Vector3.zero + new Vector3(0,0,10); //We lerp towards this
    private Quaternion correctRot = Quaternion.identity; //We lerp towards this


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigidbody.velocity); 
        }
        else
        {		
            correctPos = (Vector3)stream.ReceiveNext();
            correctRot = (Quaternion)stream.ReceiveNext();
            rigidbody.velocity = (Vector3)stream.ReceiveNext();

        }
    }
*/
}