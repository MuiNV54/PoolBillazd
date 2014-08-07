using UnityEngine;
using System.Collections;

public class BallVelocity : MonoBehaviour 
{
	public bool isStatic;
	public bool isFinished;
	public bool isAddedToArray;

	public Vector3 firstPosition;
	public float MinAcceptableVelocity = 0.1f;
	public float MinAcceptableEulerangle = 0.1f;

	private GameObject gameManager;
	private GameManager _gameManager;

	private GameObject cue;
	private Cue _cue;

	void Start()
	{
		isStatic = true;
		isFinished = false;
		isAddedToArray = false;
		firstPosition = transform.position;

		gameManager = GameObject.FindGameObjectWithTag("GameManager");
		_gameManager = gameManager.GetComponent<GameManager> ();

		cue = GameObject.Find("Cue");
		_cue = cue.GetComponent<Cue> ();
	}

	void Update () 
	{
		if (rigidbody.velocity.magnitude < MinAcceptableVelocity)
		{
			if (!rigidbody.isKinematic)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
			isStatic = true;
		}
		else
		{
			isStatic = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Pocket")
		{
			Debug.Log("Ball");
			if (gameObject.name != "CUE_BALL")
			{
				if (gameObject.name != "ball08")
				{
					isFinished = true;
					rigidbody.constraints = RigidbodyConstraints.None;
				}
				else
				{
					OnEndGame();
				}
			}
			else
			{
				_gameManager.cueBallFail = true;
			}
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Rail")
		{
			if (_gameManager.isOnTurn)
			{
				if (transform.rigidbody.velocity.magnitude >= 0.1f)
					_gameManager.ballImpactRail = true;
			}
		}
	}

	void OnEndGame()
	{
		if (_gameManager.ball8Enable)
		{
			GameManager.nameWin = _gameManager.turnStyle.ToString();
		}
		else
		{
			if (_gameManager.turnStyle == 1)
			{
				GameManager.nameWin = "2";
			}
			else
			{
				GameManager.nameWin = "1";
			}
		}
		
		_gameManager.gameEnd = true;

		this._cue.NetworkCom.SendNameWin (GameManager.nameWin);
		this._cue.NetworkCom.SendEndGame (true);
	}
}
