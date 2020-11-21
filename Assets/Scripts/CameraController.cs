using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	public Car car;
	public WheelCollider wheelFL;
	
	public Text score;
	
	Vector3 cameraForward;
	
	// Use this for initialization
	void Start () {
		cameraForward = Vector3.forward;
	}
	
	// Update is called once per frame
	void Update () {
		float carVelMag = car.GetComponent<Rigidbody>().velocity.magnitude;
		Vector3 carVelNorm = car.GetComponent<Rigidbody>().velocity / carVelMag;
		
		float idle = 5f;
		
		Vector3 velGroundPlane = new Vector3(car.GetComponent<Rigidbody>().velocity.x, 0f, car.GetComponent<Rigidbody>().velocity.z).normalized;
		
		Vector3 goPoint = cameraForward + (velGroundPlane - cameraForward)*0.05f;
		Quaternion rot = Quaternion.FromToRotation(cameraForward, goPoint);
		cameraForward  = rot * cameraForward;
		
		transform.position = car.transform.position - cameraForward*idle + Vector3.up*idle*0.3f;
		transform.LookAt(car.transform.position + velGroundPlane*5f);
		
		score.text = " " + Mathf.RoundToInt(car.GetComponent<Rigidbody>().velocity.magnitude*2.16f) + " mi/hr";
	}
}

