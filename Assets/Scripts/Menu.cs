using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public Car car;

	public void Reset()
	{
		car.transform.position = new Vector3(0f, 2f, 0f);
		car.transform.rotation = Quaternion.identity;
		car.GetComponent<Rigidbody>().velocity = Vector3.zero;
		car.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}
}
