using UnityEngine;
using System.Collections;

public class CueBall : MonoBehaviour 
{	
	public float unitForce = 500;
	public bool ballFirstShoted;  // when add force first to cueBall, the value is true 
	public string firstBallCollision;    // name of ball that cueBall first collision
	public bool wrongBall;
	public bool cueBallMovable;
	public bool ballCollision;      // collision with other ball

	public Vector3 firstPosition;

	private GameObject gameManager;
	private GameManager _gameManager;

	private GameObject cue;
	private Cue _cue;

	void Start()
	{
		firstPosition = transform.position;
		wrongBall = false;
		firstBallCollision = "";
		ballFirstShoted = false;

		gameManager = GameObject.Find("GameManager");
		_gameManager = gameManager.GetComponent<GameManager> ();

		cue = GameObject.Find("Cue");
		_cue = cue.GetComponent<Cue> ();

		cueBallMovable = false;
		ballCollision = false;
	}

	public void Update()
	{
		if (_gameManager.ballInHand)
		{
			ControllBallInHand();
		}

		if (!cueBallMovable)
		{
			for (int i = 0; i < _gameManager.balls.Length; i++)
			{
				BallVelocity ballVelocity = _gameManager.balls[i].GetComponent<BallVelocity>();
				
				if ((_gameManager.balls[i].name != "CUE_BALL") && !ballVelocity.isAddedToArray)
				{
					_gameManager.balls[i].rigidbody.isKinematic = false;
					_gameManager.balls[i].rigidbody.useGravity = true;
				}
			}
		}
	}

	public void ControllBallInHand()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (_cue.isPlayable)
		{
			if (Physics.Raycast(ray,out hit, 200.0f))
			{
				if (Input.GetMouseButtonDown(0))
				{
					if (hit.collider.name == "CUE_BALL")
					{
						cueBallMovable = true;
						cue.gameObject.SetActive(false);
					}
				}
			}

			if (cueBallMovable)
			{
				for (int i = 0; i < _gameManager.balls.Length; i++)
				{
					BallVelocity ballVelocity = _gameManager.balls[i].GetComponent<BallVelocity>();

					if ((_gameManager.balls[i].name != "CUE_BALL") && !ballVelocity.isAddedToArray)
					{
						_gameManager.balls[i].rigidbody.isKinematic = true;
						_gameManager.balls[i].rigidbody.useGravity = false;
					}
				}

				// vector distance fromm hit.point to cueball position
				Vector3 hitPointDiff = hit.point - transform.position;
				hitPointDiff *= 10;
				rigidbody.velocity = new Vector3(hitPointDiff.x, 0, hitPointDiff.z);
				SetBorder();
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			cueBallMovable = false;
			cue.SetActive(true);
			_cue.staffRotationEnabled = false;
			rigidbody.velocity = Vector3.zero;
		}
	}

	public void ForceToBall(Vector3 direction)
	{
		rigidbody.AddForce(direction * unitForce);

		firstBallCollision = "";
		_gameManager.foul = false;
		_gameManager.turnTimer = _gameManager.resetTurnTimer;

		if ((!_gameManager.breaked) && ballFirstShoted)
		{
			_gameManager.breaked = true;
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if (other.collider.tag == "Pocket")
		{
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.constraints = RigidbodyConstraints.None;
			Debug.Log("CueBall in pocket");
		}

		if (_gameManager.turnStyle != 0)
		{
			if (other.gameObject.tag == "Ball")
			{
				if (_gameManager.isOnTurn) 
				{
					if (firstBallCollision == "")
					{
						firstBallCollision = other.gameObject.name;

						if (_gameManager.ball8Enable)
						{
							if (string.Compare(firstBallCollision, "ball08") != 0)
							{
								wrongBall = true;
							}
							else
							{
								wrongBall = true;
							}
						}
						else
						{
							if (_gameManager.turnStyle == 1)
							{
								if (string.Compare(firstBallCollision, "ball08") == -1)
								{
									wrongBall = false;
								}
								else
								{
									wrongBall = true;
								}
							}
							else
							{
								if (string.Compare(firstBallCollision, "ball08") == 1)
								{
									wrongBall = false;
								}
								else
								{
									wrongBall = true;
								}
							}
						}
					}
				}
			}
		}
	}

	void SetBorder ()
	{
		float xBorder, zBorder;
		if (transform.position.z < -2.48f)
		{
			zBorder = -2.48f;
		}
		else
		{
			if (transform.position.z > 1.9f)
			{
				zBorder = 1.9f;
			}
			else
				zBorder = transform.position.z;
		}

		if (transform.position.x < -3.1f)
		{
			xBorder = -3.1f;
		}
		else
		{
			if (transform.position.z > 5.8f)
			{
				xBorder = 5.8f;
			}
			else
				xBorder = transform.position.x;
		}

		transform.position = new Vector3 (xBorder, transform.position.y, zBorder);
	}

	void ControlBallCollision(GameObject other)
	{
		Vector3 vectorOtherBallDiff = transform.position - other.transform.position;

		vectorOtherBallDiff = vectorOtherBallDiff * (0.258f / vectorOtherBallDiff.magnitude);
		vectorOtherBallDiff.y = 0;

		transform.position = other.transform.position + vectorOtherBallDiff;
	}

	public void OnCueBallFail()
	{
		transform.position = firstPosition;
		rigidbody.velocity = Vector3.zero;
		rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		this._cue.NetworkCom.cueBallFail = false;
	}
}
