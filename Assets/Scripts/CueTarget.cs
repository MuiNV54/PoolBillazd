using UnityEngine;
using System.Collections;

public class CueTarget : MonoBehaviour {
	private GameObject ballDirection;
	public Vector3 directionVector;

	void Start () {
		ballDirection = GameObject.Find("BallDirection");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionStay(Collision other)
	{
		if (other.gameObject.tag == "Ball")
		{
			ballDirection.SetActive(true);
			ballDirection.transform.position = new Vector3 (other.transform.position.x,
			                                                other.transform.position.y + 0.12f, 
			                                                other.transform.position.z);

			directionVector = other.transform.position - transform.position;
			float angleY = Vector3.Angle(directionVector, - Vector3.right);
			if (directionVector.z < 0)
				angleY = -angleY;

			ballDirection.transform.eulerAngles = new Vector3(ballDirection.transform.eulerAngles.x,
			                                                  angleY,
			                                                  ballDirection.transform.eulerAngles.z);
		}
		else
		{
			ballDirection.SetActive(false);
		}
	}
}
