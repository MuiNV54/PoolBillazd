using UnityEngine;
using System.Collections;

public class BallVelocity : MonoBehaviour 
{
	public bool isStatic;
	public bool isFinished;
	public bool isAddedToArray;

	public Vector3 firstPosition;
	public float MinAcceptableVelocity = 0.1f;
	public float MinAcceptableEulerangle = 0.3f;


	private GameObject gameManager;
	private GameManager _gameManager;

	void Start()
	{
		isStatic = true;
		isFinished = false;
		isAddedToArray = false;
		firstPosition = transform.position;

		gameManager = GameObject.FindGameObjectWithTag("GameController");
		_gameManager = gameManager.GetComponent<GameManager> ();
	}

	void Update () 
	{
		if ((rigidbody.velocity.magnitude < MinAcceptableVelocity))
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			isStatic = true;
		}
		else
		{
			isStatic = false;
		}

		if ((transform.position.y < 2.5f))
		{
			rigidbody.useGravity = false;
			rigidbody.velocity = Vector3.zero;

			if (_gameManager.allBallStatic)
			{
				rigidbody.useGravity = true;
				renderer.enabled = true;
				rigidbody.velocity = Vector3.zero;
				transform.position = firstPosition;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Hole")
		{
			isFinished = true;
			collider.isTrigger = true;

			Debug.Log ("abcadasdas");
			if (gameObject.name != "WhiteBall")
			{
//				rigidbody.useGravity = false;
//				rigidbody.velocity = Vector3.zero;
			}
			else 
			{
				Debug.Log ("WhiteBall");

//				rigidbody.useGravity = false;
//				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
//				renderer.enabled = false;
			}

//			transform.eulerAngles = Vector3.zero;
//			transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
		}
	}
}
