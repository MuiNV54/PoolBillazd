using UnityEngine;
using System.Collections;

public class CueTarget : MonoBehaviour {
	private GameObject ballDirection;
	public Vector3 directionVector;

	private GameObject cue;
	private StaffDirection staffDirection;

	void Start () {
		ballDirection = GameObject.Find("BallDirection");
		cue = GameObject.Find("Cue");
		staffDirection = cue.GetComponent<StaffDirection> ();
	}
	
	// Update is called once per frame
	void Update () {
		SetUpBallDirection ();
	}

	void SetUpBallDirection()
	{
		if (staffDirection.ballTarget != null)
		{
			Vector3 fromBallTargetToCamera = Camera.main.transform.position - staffDirection.ballTarget.transform.position;
			Vector3 staffDirectionVector = fromBallTargetToCamera * (0.14f / fromBallTargetToCamera.y);

			ballDirection.SetActive(true);
			Vector3 newPosition = new Vector3 (staffDirection.ballTarget.transform.position.x + staffDirectionVector.x,
			                                   ballDirection.transform.position.y,
			                                   staffDirection.ballTarget.transform.position.z + staffDirectionVector.z);

			ballDirection.transform.position = newPosition;

			directionVector = staffDirection.ballTarget.transform.position - transform.position;
			directionVector.y = 0;
			ballDirection.transform.right = - directionVector.normalized;

//			float angleY = Vector3.Angle(Vector3.forward , directionVector);
//			if (directionVector.z < 0)
//				angleY = -angleY;
//
//			ballDirection.transform.eulerAngles = new Vector3(ballDirection.transform.eulerAngles.x,
//			                                                  angleY,
//			                                                  ballDirection.transform.eulerAngles.z);
		}
		else
		{
			ballDirection.SetActive(false);
		}
	}
}
