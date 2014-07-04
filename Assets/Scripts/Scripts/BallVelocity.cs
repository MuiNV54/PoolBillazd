using UnityEngine;
using System.Collections;

public class BallVelocity : MonoBehaviour 
{
	public bool isStatic;
	public bool isFinished;
	public bool isAddedToArray;
	public bool cueBallFail = false;

	public Vector3 firstPosition;
	public float MinAcceptableVelocity = 0.1f;
	public float MinAcceptableEulerangle = 0.1f;


	private GameObject gameManager;
	private GameManager _gameManager;

	void Start()
	{
		isStatic = true;
		isFinished = false;
		isAddedToArray = false;
		firstPosition = transform.position;

		gameManager = GameObject.FindGameObjectWithTag("GameManager");
		_gameManager = gameManager.GetComponent<GameManager> ();
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

		OnCueBallFail ();
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
					Debug.Log("Thua CMMR!");
				}
			}
			else 
			{
				rigidbody.angularVelocity = Vector3.zero;
				rigidbody.constraints = RigidbodyConstraints.None;
				cueBallFail = true;
			}
		}
	}

	void OnCueBallFail()
	{
		if ((cueBallFail) && (_gameManager.allBallStatic))
		{
			transform.position = firstPosition;
			rigidbody.velocity = Vector3.zero;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			cueBallFail = false;
		}
	}
}
