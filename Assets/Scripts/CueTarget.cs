using UnityEngine;
using System.Collections;

public class CueTarget : MonoBehaviour {
	private GameObject ballDirection;
	public GameObject ballReplection;

	float oldReplectionScale;
	public Vector3 directionVector;

	private GameObject cue;
	private StaffDirection staffDirection;

	void Start () {
		ballDirection = GameObject.Find("BallDirection");
		cue = GameObject.Find("Cue");
		staffDirection = cue.GetComponent<StaffDirection> ();

		oldReplectionScale = ballReplection.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		SetUpBallDirection ();
		SetBallReplectionPosition ();
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
		}
		else
		{
			ballDirection.SetActive(false);
		}
	}

	void SetBallReplectionPosition()
	{
		ballReplection.transform.position = transform.position;

		float angle = FindAngle (directionVector, -transform.right, Vector3.up);
		if (angle > 0)
		{
			ballReplection.transform.eulerAngles = new Vector3 (ballDirection.transform.eulerAngles.x,
		                                                    ballDirection.transform.eulerAngles.y + 90,
		                                                    ballDirection.transform.eulerAngles.z);
		}
		else
		{
			ballReplection.transform.eulerAngles = new Vector3 (ballDirection.transform.eulerAngles.x,
			                                                    ballDirection.transform.eulerAngles.y - 90,
			                                                    ballDirection.transform.eulerAngles.z);
		}

		//set length of ballDirection and ballReplection
		float currentAngle = Vector3.Angle (-transform.right, directionVector);
		Debug.Log (currentAngle);

		float scaleBallReplectionValue = Mathf.Sin (currentAngle * Mathf.PI / 180);
		float scaleBallDirectionValue = Mathf.Cos (currentAngle * Mathf.PI / 180);

		ballReplection.transform.localScale = new Vector3 (oldReplectionScale * scaleBallReplectionValue,
		                                                   ballReplection.transform.localScale.y,
		                                                   ballReplection.transform.localScale.z);

		ballDirection.transform.localScale = new Vector3 (oldReplectionScale * scaleBallDirectionValue,
		                                                  ballDirection.transform.localScale.y,
		                                                  ballDirection.transform.localScale.z);
	}

	float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		if (toVector == Vector3.zero)
		{
			return 0.0f;
		}
		
		float angle = Vector3.Angle (fromVector, toVector);
		Vector3 normal = Vector3.Cross (fromVector, toVector);
		angle *= Mathf.Sign (Vector3.Dot (normal, upVector));
		angle *= Mathf.Deg2Rad;
		return angle;
	}
}
