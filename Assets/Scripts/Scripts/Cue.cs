using UnityEngine;
using System.Collections;

public class Cue : MonoBehaviour 
{
	public GameObject target;
	public GameObject displayStaff;
	public GameObject cueTarget;
	public GameObject cueDirection;
	private GameObject ballDirection;

	public float maxDistanceStaffMove = -3f;
	public bool staffMoveEnabled;
	public bool staffRotationEnabled;

	private Vector3 oldDisplayStaffPoint;
	private Vector3 oldDisplayStaffPosition;
	private Vector3 oldStaffPosition;

	private GameObject gameManager;
	private GameManager _gameManager;
	private CueBall cueBall;

	void Start () 
	{
		transform.position = target.transform.position;
		transform.eulerAngles = Vector3.zero;
//		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 0, transform.eulerAngles.z);

		staffMoveEnabled = false;
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Cue"),LayerMask.NameToLayer("Default"));

		oldDisplayStaffPoint = Vector3.zero;
		oldDisplayStaffPosition = displayStaff.transform.position;
		oldStaffPosition = transform.position;

		cueBall = target.GetComponent<CueBall> ();
		ballDirection = GameObject.Find("BallDirection");
//		gameManager = GameObject.FindGameObjectWithTag("GameController");
//		_gameManager = gameManager.GetComponent<GameManager> ();
	}
	
	void Update ()
	{
		TouchControll ();

		if (!staffMoveEnabled)
			transform.position = target.transform.position;
	}

	void TouchControll()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(ray,out hit, 200.0f))
		{
			Debug.DrawLine(ray.origin, hit.point);
			if (hit.collider.gameObject.tag == "Cue")
			{
				if (Input.GetMouseButtonDown(0))
				{
					staffRotationEnabled = true;
				}
			}

			if ((hit.collider.gameObject.tag == "DisplayStaff") && !staffRotationEnabled)
			{
				if (Input.GetMouseButtonDown(0))
				{
					staffMoveEnabled = true;
					oldDisplayStaffPoint = hit.point;
					oldStaffPosition = transform.position;
				}
			}
		}

		if (staffRotationEnabled)
		{
			UpdateRotation(hit.point);
		}

		if (staffMoveEnabled)
		{
			ChangeStaffPosition(hit.point);
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (staffMoveEnabled)
			{
				ReturnStaffPosition();
			}
			else if (staffRotationEnabled)
			{
				staffRotationEnabled = false;
			}
		}
	}

	void UpdateRotation(Vector3 point)
	{
		Vector3 newStaffDirection = point - transform.position;
		newStaffDirection.y = 0;
		
		if (newStaffDirection.x <= 0)
		{
			transform.localEulerAngles =new Vector3(transform.localEulerAngles.x,
			                                   Vector3.Angle(-Vector3.forward, newStaffDirection) + 90,
			                                   transform.localEulerAngles.z);
		}
		else
		{
			transform.eulerAngles =new Vector3(transform.eulerAngles.x,
			                                   - Vector3.Angle(-Vector3.forward, newStaffDirection) + 90,
			                                   transform.eulerAngles.z);
		}
	}

	void ChangeStaffPosition(Vector3 point)
	{
		cueTarget.SetActive (false);
		cueDirection.SetActive (false);

		float distanceStaffMove = point.x - oldDisplayStaffPoint.x;
		float newDisplayStaffPositionX = oldDisplayStaffPosition.x + distanceStaffMove;
		
//		if ((distanceStaffMove < 0) && (distanceStaffMove > maxDistanceStaffMove))
		if (distanceStaffMove < 0)
		{
			displayStaff.transform.position = new Vector3(newDisplayStaffPositionX,
														  displayStaff.transform.position.y,
			                                              displayStaff.transform.position.z);
			
			transform.position = oldStaffPosition - transform.right * distanceStaffMove;
		}
	}

	void ReturnStaffPosition()
	{
		staffMoveEnabled = false;
		displayStaff.transform.position = oldDisplayStaffPosition;
		
		Vector3 forceToBallDirection = target.transform.position - transform.position;
		forceToBallDirection.y = 0;
		cueBall.ForceToBall(forceToBallDirection);

		transform.position = oldStaffPosition;
	}
}
