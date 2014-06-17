using UnityEngine;
using System.Collections;


public class ThirdPersonCameraNET : MonoBehaviour
{
	private int	MAIN_CAMERA = 0;
	private int PORT_CAMERA = 1;
	private int STARBOARD_CAMERA = 2;


	public Collider target;
		// The object we're looking at
	new public Camera camera;
		// The camera to control


	public LayerMask obstacleLayers = -1, groundLayers = -1;
		// Which layers should count as obstructing the view? And which are designated ground?
		// NOTICE: Make sure that the target collider is not in any of these layers!
	public float groundedCheckOffset = 0.7f;
		// Tweak so check starts from just within target footing
	public float rotationUpdateSpeed = 60.0f,
		lookUpSpeed = 20.0f,
		distanceUpdateSpeed = 10.0f,
		followUpdateSpeed = 10.0f;
		// Tweak these to adjust camera responsiveness
	public float maxForwardAngle = 80.0f;
		// Tweak to adjust camera clamping angle - specifies the maximum angle between target and clamped camera forward
	public float minDistance = 0.1f,
		maxDistance = 10.0f,
		zoomSpeed = 1.0f;
		// Tweak to adjust scrollwheel zoom
	public bool
		showGizmos = true,
			// Turn this off to reduce gizmo clutter if needed
		requireLock = true,
			// Turn this off if the camera should be controllable even without cursor lock
		controlLock = true;
			// Turn this off if you want mouse lock controlled elsewhere
		
		
	private const float movementThreshold = 0.1f, rotationThreshold = 0.1f;
		// Tweak these to adjust camera responsiveness
	private const float groundedDistance = 0.5f;
		// Tweak if the camera goes into ground mode too soon or late
		
		
	private Vector3 lastStationaryPosition;
	private float optimalDistance; //, targetDistance;
	private bool grounded = false;

	public int selectCamera = 0; //which camera to use? 0 - normal   1 - port, 2-broadside

	private GameObject portViewPos;
	private GameObject starboardViewPos;
	private GameObject mainViewPos;


	private float journeyLength;

	private bool isGameWorld = true; 

	private bool isRemotePlayer = true;
	
	public void SetIsRemotePlayer(bool val)
	{
		isRemotePlayer = val;
	}

	void Awake(){
		AssignCameraPositions();
	}

	//TODO: Make this not suck
	public void AssignCameraPositions(){ //GameObject main, GameObject port, GameObject starboard){
		foreach(Transform t in transform){
			foreach (Transform child in t.transform){
				if (child.name == "mainCameraPos")
					mainViewPos = child.gameObject;
				else if(child.name == "portCameraPos")
					portViewPos = child.gameObject;
				else if(child.name == "starboardCameraPos")
					starboardViewPos = child.gameObject;
			}
		}

		//mainViewPos = main;
		//portViewPos = port;
		//starboardViewPos = starboard;
	}

	public void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	void OnLevelWasLoaded(int level) {
		if (level == 0){
			isGameWorld = true;

			Reset();

			Camera extraCamera;
			if (extraCamera = GameObject.Find("Main Camera").camera)
				Destroy(extraCamera.gameObject);

			//fix the farClipPlane as it reset for some reason
			Camera.main.farClipPlane = 1000.0f;
			
		}else{
			isGameWorld = false;
		}
	}

	void Setup ()
	// If target and/or camera is not set, try using fallbacks
	{
		if (target == null)
		{
			target = GetComponent<Collider> ();
		}

			foreach (Transform child in gameObject.transform)
			{
				if (child.gameObject.name.Equals("portCameraPos"))
					portViewPos = child.gameObject;
				else if (child.gameObject.name == "starboardCameraPos")
					starboardViewPos = child.gameObject; 
				else if (child.gameObject.name == "mainCameraPos")
					mainViewPos = child.gameObject; 

			}

		if (camera == null)
		{
			if (Camera.main != null)
			{
				/*
				 * Load cameras
				 */
				camera = Camera.main;

				//Parent the camera to the player, and rename. 
				Camera.main.transform.parent = transform;
				Camera.main.farClipPlane = 1000.0f;
	            Camera.main.transform.localPosition = mainViewPos.transform.position;//new Vector3(0, 2, -10);
	            Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);
				Camera.main.name = "PlayerCamera";
			}
		}

	}


	void Start ()
	// Verify setup, initialise bookkeeping
	{
		Setup ();
			// Retry setup if references were cleared post-add
				
		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		
		if (camera == null)
		{
			Debug.LogError ("No camera assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		
		lastStationaryPosition = target.transform.position;
	//	targetDistance = optimalDistance = (camera.transform.position - target.transform.position).magnitude;


	}
	
	
	float ViewRadius
	// The minimum clear radius between the camera and the target
	{
		get
		{
			float fieldOfViewRadius = (optimalDistance / Mathf.Sin (90.0f - camera.fieldOfView / 2.0f)) * Mathf.Sin (camera.fieldOfView / 2.0f);
				// Half the width of the field of view of the camera at the position of the target
			float doubleCharacterRadius = Mathf.Max (target.bounds.extents.x, target.bounds.extents.z) * 2.0f;
			
			return Mathf.Min (doubleCharacterRadius, fieldOfViewRadius);
		}
	}
	
	
	Vector3 SnappedCameraForward
	// The camera forward vector, clamped to the target forward vector so only horizontal rotation is kept
	{
		get
		{
			Vector2 planeForward = new Vector2 (camera.transform.forward.x, camera.transform.forward.z);
			planeForward = new Vector2 (target.transform.forward.x, target.transform.forward.z).normalized *
				planeForward.magnitude;
			return new Vector3 (planeForward.x, camera.transform.forward.y, planeForward.y);
		}
	}
	
	
	void FixedUpdate ()
	// See if the camera touches the ground and adjust the camera distance if an object blocks the view
	{
		if (!isGameWorld) return;

		grounded = Physics.Raycast (
			camera.transform.position + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
		);
			// Shoot a ray downward to see if we're touching the ground
		
		Vector3 inverseLineOfSight = camera.transform.position - target.transform.position;
		
		RaycastHit hit;
		if (Physics.SphereCast (target.transform.position, ViewRadius, inverseLineOfSight, out hit, optimalDistance, obstacleLayers))
		// Cast a sphere from the target towards the camera - using the view radius - checking against the obstacle layers
		{
			//targetDistance = Mathf.Min ((hit.point - target.transform.position).magnitude, optimalDistance);
				// If something is hit, set the target distance to the hit position
		}
		else
		{
		//	targetDistance = optimalDistance;
				// If nothing is hit, target the optimal distance
		}
	}
	
	
	void Update ()
	// Update optimal distance based on scroll wheel input
	{
		if (!isGameWorld) return;
	
		optimalDistance = Mathf.Clamp (
		optimalDistance + Input.GetAxis ("Mouse ScrollWheel") * -zoomSpeed * Time.deltaTime,
		minDistance,
		maxDistance
		);

			//selectCamera variable is set by CannonManager. It is intended to allow player to aim the cannons
			//starboard or port when holding down the aim keys. 

			//Check for camera change - 

			//is camera used???
			//if ((selectCamera == 0) && (camera != Camera.main))
			if ((selectCamera == MAIN_CAMERA) && (!Input.GetMouseButton(1)))
			{
				DistanceUpdate();
			}

			//else if ((selectCamera == PORT_CAMERA) && (camera.name != "cameraPort") && (portCamera != null))
			else if ((selectCamera == PORT_CAMERA))
			{
				Vector3 startPos = camera.transform.position;
				Vector3 endPos = portViewPos.transform.position;
				Quaternion startRot = camera.transform.rotation;
				Quaternion endRot = portViewPos.transform.rotation;
				camera.transform.position = Vector3.Lerp (startPos, endPos, 1.5f*Time.deltaTime);
				camera.transform.rotation = Quaternion.Lerp (startRot, endRot,  1.5f*Time.deltaTime);
			}

			//else if ((selectCamera == STARBOARD_CAMERA) && (camera.name != "cameraStarboard") && (starboardCamera != null))
			else if ((selectCamera == STARBOARD_CAMERA) )
			{
				// float distance = time * speed; 
				Vector3 startPos = camera.transform.position;
				Vector3 endPos = starboardViewPos.transform.position;
				Quaternion startRot = camera.transform.rotation;
				Quaternion endRot = starboardViewPos.transform.rotation;

				camera.transform.position = Vector3.Lerp (startPos, endPos, 1.5f*Time.deltaTime);
				camera.transform.rotation = Quaternion.Lerp (startRot, endRot, 1.5f*Time.deltaTime);


			}

	}
	

	void LateUpdate ()
	// Update camera position - specifics are delegated to camera mode functions
	{
		if (!isGameWorld) return;

		if (
			(Input.GetMouseButton (1)) &&	// Act if a mouse button is down
			(!requireLock || controlLock || Screen.lockCursor)			// ... and we're allowed to
		)
		{
			if (controlLock)
			{
				Screen.lockCursor = true;
			}
			
			FreeUpdate ();
			lastStationaryPosition = target.transform.position;
				// Update the stationary position so we don't get an immediate snap back when releasing the mouse button
		}
		else
		{
			if (controlLock)
			{
				Screen.lockCursor = false;
			}
			
			Vector3 movement = target.transform.position - lastStationaryPosition;
			if (new Vector2 (movement.x, movement.z).magnitude > movementThreshold)
			// Only update follow camera if we moved sufficiently
			{
				FollowUpdate ();
			}
		}
		
		//DistanceUpdate ();
	}
	
	
	void FollowUpdate ()
	// Have the camera follow behind the character
	{

		if (isRemotePlayer)
			return;


		Vector3 cameraForward = target.transform.position - camera.transform.position;
		

		// JUNE 12 delete if ineffective 
		cameraForward = new Vector3 (cameraForward.x, 0.0f, cameraForward.z);
		//cameraForward = mainViewPos.transform.position - camera.transform.position;





// Ignore camera elevation when calculating the angle
		
		float rotationAmount = Vector3.Angle (cameraForward, target.transform.forward);
		
		if (rotationAmount < rotationThreshold)
		// Stop rotating if we're within the threshold
		{
			lastStationaryPosition = target.transform.position;
		}
		
		rotationAmount *= followUpdateSpeed * Time.deltaTime;
		
		if (Vector3.Angle (cameraForward, target.transform.right) < Vector3.Angle (cameraForward, target.transform.right * -1.0f))
		// Rotate to the left if the camera is to the right of target forward
		{
			rotationAmount *= -1.0f;
		}
		
		camera.transform.RotateAround (target.transform.position, Vector3.up, rotationAmount);
	}
	
	
	void FreeUpdate ()
	// Control the camera via the mouse
	{

		float rotationAmount;
		
		// Horizontal rotation:
		
		if (Input.GetMouseButton (1))
		{
			rotationAmount = Input.GetAxis ("Mouse X") * rotationUpdateSpeed * Time.deltaTime;
			camera.transform.RotateAround (target.transform.position, Vector3.up, rotationAmount);
		}
		
		// Vertical rotation:
		
		rotationAmount = Input.GetAxis ("Mouse Y") * -1.0f * lookUpSpeed * Time.deltaTime;
			// Calculate vertical rotation
		
		bool lookFromBelow = Vector3.Angle (camera.transform.forward, target.transform.up * -1) >
			Vector3.Angle (camera.transform.forward, target.transform.up);
			// Is the camera looking up at the target?
		
		if (grounded &&	lookFromBelow )
		// If we're grounded and look up from this position - applying the vertical rotation to the camera pivot point
		{
			camera.transform.RotateAround (camera.transform.position, camera.transform.right, rotationAmount);
		}
		else
		// If we're not grounded, apply the vertical rotation to the target pivot point
		{
			camera.transform.RotateAround (target.transform.position, camera.transform.right, rotationAmount);
			camera.transform.LookAt (target.transform.position);
				// Apply rotation and keep looking at the target
			
			float forwardAngle = Vector3.Angle (target.transform.forward, SnappedCameraForward);
				// Get the new rotation relative to the target forward vector
			
			if (forwardAngle > maxForwardAngle)
			// If the new rotation brought the camera over the clamp max, rotate it back by the difference
			{
				camera.transform.RotateAround (
					target.transform.position,
					camera.transform.right,
					lookFromBelow ? forwardAngle - maxForwardAngle : maxForwardAngle - forwardAngle
				);
			}
		}
	}
	
	
	void DistanceUpdate ()
	// Apply any change in camera distance
	{
		//Vector3 targetPosition = target.transform.position + (camera.transform.position - target.transform.position).normalized * targetDistance;
		//camera.transform.position = Vector3.Lerp (camera.transform.position, targetPosition, Time.deltaTime * distanceUpdateSpeed);


		Vector3 startPos = camera.transform.position;
		Vector3 endPos = mainViewPos.transform.position;
		Quaternion startRot = camera.transform.rotation;
		Quaternion endRot = mainViewPos.transform.rotation;
		camera.transform.position = Vector3.Lerp (startPos, endPos, 5.5f*Time.deltaTime);
		camera.transform.rotation = Quaternion.Lerp (startRot, endRot,  5.5f*Time.deltaTime);
	}
	
	
	void OnDrawGizmosSelected ()
	// Use gizmos to gain information about the state of your setup
	{
		if (!showGizmos || target == null || camera == null)
		{
			return;
		}
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine (target.transform.position, target.transform.position + target.transform.forward);
			// Visualise the target forward vector
		
		Gizmos.color = grounded ? Color.blue : Color.red;
		Gizmos.DrawLine (camera.transform.position + target.transform.up * -groundedCheckOffset,
			camera.transform.position + target.transform.up * -(groundedCheckOffset + groundedDistance));
			// Visualise the camera grounded check and whether or not it is grounded
			
		Gizmos.color = Color.green;
		Gizmos.DrawLine (camera.transform.position, camera.transform.position + camera.transform.forward);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (camera.transform.position, camera.transform.position + SnappedCameraForward);
			// Visualise the camera forward vector (green) vs. the SnappedCameraForward
	}
}
