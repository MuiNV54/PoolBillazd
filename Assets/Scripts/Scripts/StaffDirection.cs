using UnityEngine;
using System.Collections;

public class StaffDirection : MonoBehaviour 
{
	public GameObject staffTarget;
	public GameObject staffDirection;

	public GameObject leftSide;
	public GameObject rightSide;

	public float distanceBall = 0.258f;

	Vector3 leftVector;
	Vector3 rightVector;
	Vector3 centerVector;
	Vector3 directionVector;

	public Transform ballTarget;
	Transform leftTarget;
	Transform centerTarget;
	Transform rightTarget;
	public GameObject targetPos;

	float initDistance;

	private Cue cue;

	void Start()
	{
		cue = GetComponent<Cue> ();
		initDistance = (staffTarget.transform.position - transform.position).magnitude;
	}

	void Update()
	{
		DrawTarget ();
	}
	
	void DrawTarget()
	{
		RaycastHit hit;
		Vector3 fwd = - transform.right;
		int layerMask = 1 << 8;

		if (Physics.Raycast(leftSide.transform.position, fwd, out hit, layerMask))
		{
			Debug.DrawLine(leftSide.transform.position, hit.point);
			leftVector = hit.point - leftSide.transform.position;

			if (hit.collider.tag == "Ball")
			{
				leftTarget = hit.collider.transform;
			}
			else 
			{
				leftTarget = null;
			}
		}

		if (Physics.Raycast(rightSide.transform.position, fwd, out hit, layerMask))
		{
			Debug.DrawLine(rightSide.transform.position, hit.point);
			rightVector = hit.point - rightSide.transform.position;

			if (hit.collider.tag == "Ball")
			{
				rightTarget = hit.collider.transform;
			}
			else 
			{
				rightTarget = null;
			}
		}

		if (Physics.Raycast(transform.position, fwd, out hit, layerMask))
		{
			Debug.DrawLine(transform.position, hit.point);
			centerVector = hit.point - transform.position;

			if (hit.collider.tag == "Ball")
			{
				centerTarget = hit.collider.transform;
			}
			else 
			{
				centerTarget = null;
			}
		}

		float ratio = (centerVector.magnitude - 0.12f) / (centerVector.magnitude);
		centerVector *= ratio;

		directionVector = centerVector;
		ballTarget = centerTarget;

		if (leftVector.magnitude < directionVector.magnitude)
		{
			directionVector = leftVector;
			ballTarget = leftTarget;
		}

		if (rightVector.magnitude < directionVector.magnitude)
		{
			directionVector = rightVector;
			ballTarget = rightTarget;
		}

		if (ballTarget != null)
		{
			Vector3 vectorDistance = ballTarget.transform.position - transform.position;
			float angle = Vector3.Angle( - transform.right, vectorDistance);
			float b = vectorDistance.magnitude * Mathf.Sin((angle * Mathf.PI)/180);
			float a = Mathf.Abs(vectorDistance.magnitude * Mathf.Cos((angle * Mathf.PI)/180));
			float e = Mathf.Sqrt(Mathf.Abs(distanceBall * distanceBall - b * b));
			float distancePos = a - e;
			
			float distanceTemp = transform.right.magnitude;
			Vector3 vectorPos = - transform.right * distancePos / (transform.right.magnitude);
			
			staffTarget.transform.position = transform.position + vectorPos;
		}
		else
		{
			staffTarget.transform.position = transform.position + directionVector ;
		}

		Vector3 distanceBallTarget = transform.position - staffTarget.transform.position;
		distanceBallTarget.y = 0;
		float ratioCueDirection = distanceBallTarget.magnitude / initDistance;

		staffDirection.transform.localScale = new Vector3(ratioCueDirection, 1, 1);
		staffDirection.transform.position = (staffTarget.transform.position + transform.position) / 2 
			+ new Vector3(0, 0.07f, 0);
	}
}