using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float step = 0.1f;
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float totalDist = 0f;
			float maxDist = 20f;

			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 20f))
			{
				Debug.Log(hitInfo.point);
			}
		}
	}


}
