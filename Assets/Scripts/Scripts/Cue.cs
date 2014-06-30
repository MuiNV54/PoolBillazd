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

	Vector3 beginPoint;
	Vector3 beginPos;
	float oldStaffAngle;

	private GameObject gameManager;
	private GameManager _gameManager;
	private CueBall cueBall;

	/// Network component
	public NetworkClient NetworkCom;
	public bool isPlayable;

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

		///<summary>
		/// In case this client is taking turn, it will sends the appropriate parameters to the opponent.
		/// </summary>
		if (isPlayable)
		{
			if (Physics.Raycast(ray,out hit, 200.0f))
			{
			//Debug.DrawLine(ray.origin, hit.point);
//			if (hit.collider.gameObject.tag == "Cue")
//			{
//				if (Input.GetMouseButtonDown(0))
//				{
//					staffRotationEnabled = true;
//				}
				//			}

				if (Input.GetMouseButtonDown(0))
				{
					if (hit.collider.tag != "DisplayStaff")
					{
						staffRotationEnabled = true;
						beginPoint = hit.point;

						if (-transform.forward.x <= 0)
						{
							oldStaffAngle = Vector3.Angle ( - Vector3.forward, - transform.forward);
						}
						else 
						{
							oldStaffAngle = - Vector3.Angle ( - Vector3.forward, - transform.forward);
						}
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
		///<summary>
		/// In case the opponent is taking turn, this client will get the event from other client through Photon server
		/// </summary>
		else 
		{
			UpdateFromPeer();
		}
	}
	void UpdateRotation(Vector3 point)
	{
		Vector3 oldHitPointDiretion = beginPoint - transform.position;
		Vector3 newHitPointDirection = point - transform.position;
		oldHitPointDiretion.y = 0;
		newHitPointDirection.y = 0;

		float oldPointAngle;
		float newPointAngle;

		newPointAngle = Vector3.Angle (Vector3.forward, newHitPointDirection);
		oldPointAngle = Vector3.Angle (Vector3.forward, oldHitPointDiretion);

		float adjustAngle = newPointAngle - oldPointAngle;
		if ((newHitPointDirection.x < 0) && (oldHitPointDiretion.x < 0))
		{
			adjustAngle *= -1;
		}

		transform.localEulerAngles =new Vector3(transform.localEulerAngles.x,
		                                        oldStaffAngle + adjustAngle,
		                                   		transform.localEulerAngles.z);

		/// Send the Angle Cue for the opponent
		this.NetworkCom.SendCueAngle(oldStaffAngle + adjustAngle);
	}

	void ChangeStaffPosition(Vector3 point)
	{
		Vector3 pos = transform.position;
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
			pos = transform.position;
		}
		/// Send the Cue position to the opponent
		this.NetworkCom.SendCuePos(pos);
	}

	void ReturnStaffPosition()
	{
		staffMoveEnabled = false;
		displayStaff.transform.position = oldDisplayStaffPosition;
		
		Vector3 forceToBallDirection = target.transform.position - transform.position;
		forceToBallDirection.y = 0;
		cueBall.ForceToBall(forceToBallDirection);
		/// Send the force of the cue ball to the opponent
		this.NetworkCom.SendForceToBall(forceToBallDirection);

		transform.position = oldStaffPosition;
	}

	void UpdateFromPeer()
	{
		if ( this.NetworkCom.ForceToBall != Vector3.zero)
		{
			cueBall.ForceToBall(this.NetworkCom.ForceToBall);
			this.NetworkCom.ForceToBall = Vector3.zero;
		}
		if (this.NetworkCom.CueAngle != 0.0f)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
			                                         this.NetworkCom.CueAngle,
			                                         transform.localEulerAngles.z);
			this.NetworkCom.CueAngle = 0.0f;
		}
		if (this.NetworkCom.CuePos != transform.position)
		{
			transform.position = this.NetworkCom.CuePos;
		}
		isPlayable = this.NetworkCom.isNextPlayer;
	}
}
