using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
	
	static float RECIP_WIDTH;
	static float RECIP_HEIGHT;

	public Transform wheelFL;
	public Transform wheelFR;
	public Transform wheelBL;
	public Transform wheelBR;

	public WheelCollider frontLeft;
	public WheelCollider frontRight;
	public WheelCollider backLeft;
	public WheelCollider backRight;

	public TrailRenderer frontLeftSkid;
	public TrailRenderer frontRightSkid;
	public TrailRenderer backLeftSkid;
	public TrailRenderer backRightSkid;

	float inputSteer;
	float inputTorque;

	bool attachedFL;
	bool attachedFR;
	bool attachedBL;
	bool attachedBR;

	float antiRoll = 5000f;

	float maxSteer = 55f;

	int currentGear = 0;

	float[] gearRatios = new float[6];
	float engineTorque = 1000f;
	float maxEngineRPM = 3000f;
	float minEngineRPM = 1000f;

	float targetSteering;
	public float maxSteering;
	public float maxTorque;

	bool spinOkay;

	Vector3 inAirMousePos;

	// Use this for initialization
	void Start () {
		wheelFL = frontLeft.transform;
		wheelFR = frontRight.transform;
		wheelBL = backLeft.transform;
		wheelBR = backRight.transform;

		attachedFL = true;
		attachedFR = true;
		attachedBL = true;
		attachedBR = true;

		maxSteering = 50f;
		maxTorque = 400f;
		targetSteering = 0f;

		// center of mass adjustment
		//GetComponent<Rigidbody>().centerOfMass += new Vector3(0f, -0.1f, 0.2f);

		// all 6 gears
		int i = 0;
		gearRatios[i++] = 4.31f;
		gearRatios[i++] = 2.71f;
		gearRatios[i++] = 1.88f;
		gearRatios[i++] = 1.41f;
		gearRatios[i++] = 1.13f;
		gearRatios[i++] = 0.93f;

		// static
		RECIP_WIDTH = 1f / Screen.width;
		RECIP_HEIGHT = 1f / Screen.height;

		GetComponent<Rigidbody>().AddForce(Vector3.forward*50f);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void AttachWheelToCollider(WheelCollider collider)
	{
		if (collider.transform.childCount == 0)
		{
			return;
		}

		Transform visualWheel = collider.transform.GetChild(1);

		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);

		visualWheel.position = position;
		visualWheel.rotation = rotation;
	}

	private void ControlOnGround()
	{
		inputTorque = 0f;
		inputSteer = 0f;

		if (Input.GetMouseButton(0))
		{
			Vector3 pos = Input.mousePosition;
			
			float inputHorizontal = pos.x*RECIP_WIDTH - 0.5f;
			//inputSteer = inputHorizontal * maxSteering;

			float inputVertical = pos.y*RECIP_HEIGHT * 1.5f + 0.1f;
			inputTorque = inputVertical * engineTorque;
			
			float loosten = 1.6f; // increase to loosten
			inputSteer = Mathf.Pow(inputHorizontal*loosten, 3f) * maxSteering;
			
			if (inputSteer > maxSteer)
			{
				inputSteer = maxSteer;
			}
			else if (inputSteer < -maxSteer)
			{
				inputSteer = -maxSteer;
			}

			if (inputTorque > engineTorque)
			{
				inputTorque = engineTorque;
			}
		}
	}

	void ControlInAir()
	{
		Rigidbody rigBod = GetComponent<Rigidbody>();
		if ((!attachedBL || !backLeft.isGrounded) && (!attachedBR || !backRight.isGrounded) && (!attachedFR || !frontRight.isGrounded) && (!attachedFL || !frontLeft.isGrounded))
		{
			if (Input.GetMouseButton(0) && spinOkay)
			{
				float dx = Input.mousePosition.x - inAirMousePos.x;
				float dy = Input.mousePosition.y - inAirMousePos.y;

				float swipe = 0.02f;

				rigBod.AddRelativeTorque(new Vector3(dy*swipe, -dx*swipe, 0f), ForceMode.VelocityChange);

				inAirMousePos = Input.mousePosition;
			}

			if (Input.GetMouseButtonDown(0))
			{
				inAirMousePos = Input.mousePosition;
				rigBod.angularVelocity = new Vector3(0f, 0f, 0f);
				spinOkay = true;
			}
		}

		if (!Input.GetMouseButton(0))
		{
			spinOkay = false;
		}
	}

	void ApplyControl()
	{
		// steering
		if (attachedFL)
		{
			frontLeft.steerAngle = inputSteer;

			AttachWheelToCollider(frontLeft);
		}
		if (attachedFR)
		{
			frontRight.steerAngle = inputSteer;

			AttachWheelToCollider(frontRight);
		}
		if (attachedBL)
		{
			//backLeft.motorTorque = inputSteer;

			AttachWheelToCollider(backLeft);
		}
		if (attachedBR)
		{
			//backRight.motorTorque = inputSteer;

			AttachWheelToCollider(backRight);
		}

		// torque
		if (attachedFL)
		{
			//frontLeft.motorTorque = inputTorque;
		}
		if (attachedFR)
		{
			//frontRight.motorTorque = inputTorque;
		}
		if (attachedBL)
		{
			backLeft.motorTorque = inputTorque;
		}
		if (attachedBR)
		{
			backRight.motorTorque = inputTorque;
		}
	}

	/*
	void OnCollisionEnter(Collision col)
	{
		float breakAmt = 5f;
		Rigidbody rig;
		
		foreach (ContactPoint contact in col.contacts)
		{
			if (Vector3.Dot(contact.normal, col.relativeVelocity) > breakAmt)
			{
				if (contact.thisCollider.gameObject.name == "fl")
				{
					//Destroy(frontLeft.gameObject);
					//contact.thisCollider.transform.parent.parent = null;
					frontLeft = null;
					attachedFL = false;
					//rig = wheelFL.gameObject.AddComponent<Rigidbody>();
				}
				else if (contact.thisCollider.gameObject.name == "fr")
				{
					//Destroy(frontRight.gameObject);
					//contact.thisCollider.transform.parent.parent = null;
					frontRight = null;
					attachedFR = false;
					//rig = wheelFR.gameObject.AddComponent<Rigidbody>();
				}
				else if (contact.thisCollider.gameObject.name == "bl")
				{
					//Destroy(backLeft.gameObject);
					//contact.thisCollider.transform.parent.parent = null;
					backLeft = null;
					attachedBL = false;
					//rig = wheelBL.gameObject.AddComponent<Rigidbody>();
				}
				else if (contact.thisCollider.gameObject.name == "bl")
				{
					//Destroy(backRight.gameObject);
					//contact.thisCollider.transform.parent.parent = null;
					backRight = null;
					attachedBR = false;
					//rig = wheelBR.gameObject.AddComponent<Rigidbody>();
				}
			}
		}
	}
	*/

	void CheckDamage()
	{
		Rigidbody rig;

		float dmgAngle = 45f;
		float dmgSpeed = 15f;

		RaycastHit hitInfo;
		if (attachedFL && frontLeft.Raycast(new Ray(wheelFL.position, GetComponent<Rigidbody>().velocity.normalized), out hitInfo, GetComponent<Rigidbody>().velocity.magnitude))
		{
			Destroy(frontLeft);
			frontLeft = null;
			attachedFL = false;
			rig = wheelFL.gameObject.AddComponent<Rigidbody>();
		}
		if (attachedFR && frontRight.Raycast(new Ray(wheelFR.position, GetComponent<Rigidbody>().velocity.normalized), out hitInfo, GetComponent<Rigidbody>().velocity.magnitude))
		{
			Destroy(frontRight);
			frontRight = null;
			attachedFR = false;
			rig = wheelFR.gameObject.AddComponent<Rigidbody>();
		}
		if (attachedBL && backLeft.Raycast(new Ray(wheelBL.position, GetComponent<Rigidbody>().velocity.normalized), out hitInfo, GetComponent<Rigidbody>().velocity.magnitude))
		{
			Destroy(backLeft);
			backLeft = null;
			attachedBL = false;
			rig = wheelBL.gameObject.AddComponent<Rigidbody>();
		}
		if (attachedBR && backRight.Raycast(new Ray(wheelBR.position, GetComponent<Rigidbody>().velocity.normalized), out hitInfo, GetComponent<Rigidbody>().velocity.magnitude))
		{
			Destroy(backRight);
			backRight = null;
			attachedBR = false;
			rig = wheelBR.gameObject.AddComponent<Rigidbody>();
		}

		bool wheelUpDmg = false;
		if (Mathf.Acos(Vector3.Dot(GetComponent<Rigidbody>().velocity.normalized, -transform.up))*Mathf.Rad2Deg < dmgAngle 
		    && GetComponent<Rigidbody>().velocity.magnitude*2.16f > dmgSpeed)
		{
			// break wheel if grounded
			if (attachedFL && frontLeft.isGrounded)
			{
				Destroy(frontLeft);
				frontLeft = null;
				attachedFL = false;
				rig = wheelFL.gameObject.AddComponent<Rigidbody>();
			}
			if (attachedFR && frontRight.isGrounded)
			{
				Destroy(frontRight);
				frontRight = null;
				attachedFR = false;
				rig = wheelFR.gameObject.AddComponent<Rigidbody>();
			}
			if (attachedBL && backLeft.isGrounded)
			{
				Destroy(backLeft);
				backLeft = null;
				attachedBL = false;
				rig = wheelBL.gameObject.AddComponent<Rigidbody>();
			}
			if (attachedBR && backRight.isGrounded)
			{
				Destroy(backRight);
				backRight = null;
				attachedBR = false;
				rig = wheelBR.gameObject.AddComponent<Rigidbody>();
			}
		}
	}



	void FixedUpdate()
	{
		ControlOnGround();
		ControlInAir();

		//CheckDamage();

		ApplyControl();

		ApplyAntiRollAxle(frontLeft, frontRight);
		ApplyAntiRollAxle(backLeft, backRight);

		// ApplySkidMarks();
	}

	void ApplySkidMarks()
	{
		WheelHit frontLeftHit;
		WheelHit frontRightHit;
		WheelHit backLeftHit;
		WheelHit backRightHit;

		bool frontLeftTouch = backLeft.GetGroundHit(out frontLeftHit);
		bool frontRightTouch = backRight.GetGroundHit(out frontRightHit);
		bool backLeftTouch = backLeft.GetGroundHit(out backLeftHit);
		bool backRightTouch = backRight.GetGroundHit(out backRightHit);

		frontLeftSkid.enabled = false;
		if (frontLeftTouch && (frontLeftHit.sidewaysSlip >= 0.1f || frontLeftHit.forwardSlip >= 0.1f))
		{
			frontLeftSkid.enabled = true;
		}

		frontRightSkid.enabled = false;
		if (frontRightTouch && (frontRightHit.sidewaysSlip >= 0.1f || frontRightHit.forwardSlip >= 0.1f))
		{
			frontRightSkid.enabled = true;
		}

		backLeftSkid.enabled = false;
		if (backLeftTouch && (backLeftHit.sidewaysSlip >= 0.1f || backLeftHit.forwardSlip >= 0.1f))
		{
			backLeftSkid.enabled = true;
		}
		
		backRightSkid.enabled = false;
		if (backRightTouch && (backRightHit.sidewaysSlip >= 0.1f || backRightHit.forwardSlip >= 0.1f))
		{
			backRightSkid.enabled = true;
		}

		frontLeftSkid.transform.localPosition = new Vector3(0f, -backLeft.radius*1.25f, 0f);
		frontRightSkid.transform.localPosition = new Vector3(0f, -backRight.radius*1.25f, 0f);
		backLeftSkid.transform.localPosition = new Vector3(0f, -backLeft.radius*1.25f, 0f);
		backRightSkid.transform.localPosition = new Vector3(0f, -backRight.radius*1.25f, 0f);
	}

	void AdjustGear()
	{
		float avgRPM = (backLeft.rpm + backRight.rpm) * 0.5f;
		float engineRPM = avgRPM * gearRatios[currentGear];

		while (engineRPM < minEngineRPM && currentGear > 0)
		{
			currentGear--;
			engineRPM = avgRPM * gearRatios[currentGear];
		}
		while (engineRPM > maxEngineRPM && currentGear < gearRatios.GetLength(0)-1)
		{
			currentGear++;
			engineRPM = avgRPM * gearRatios[currentGear];
		}
	}

	private void ApplyAntiRollAxle(WheelCollider WheelL, WheelCollider WheelR)
	{
		WheelHit hit;
		float travelL = 1.0f;
		float travelR = 1.0f;

		bool groundedL = false;
		bool groundedR = false;

		if (WheelL != null && WheelL.GetGroundHit (out hit))
		{
			travelL = (-WheelL.transform.InverseTransformPoint (hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
		}	

		if (WheelR != null && WheelR.GetGroundHit (out hit))
		{
			travelR = (-WheelR.transform.InverseTransformPoint (hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
		}
			
		float antiRollForce = (travelL - travelR) * antiRoll;

		Rigidbody rigidbody = GetComponent<Rigidbody>();
		if (groundedL)
		{
			rigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
			                             WheelL.transform.position);
		}

		if (groundedR)
		{
			rigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce,
			                             WheelR.transform.position);
		}
	}
}
