using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public IList<GameObject> ballFinished;
	public IList<GameObject> stBallFinish;
	public IList<GameObject> ndBallFinished;

	public static bool breaked;

	private GameObject[] balls;
	private GameObject cue;
	private Cue _cue;
	private GameObject cueMesh;

	public GameObject staffDirection;
	public GameObject staffTarget;

	public bool allBallStatic;
	public bool isOnTurn;
	public bool isForceNextTurn;
	public int turnNumber;
	public int currentBallFinished;
	public int turnStyle;
	public int player1Count;
	public int player2Count;
	public int prePlayer1Count;
	public int prePlayer2Count;
	public bool nextTurnActive;

	public Vector3 ballScaleSize = new Vector3(0.3f, 0.3f, 0.3f);

	void Start () 
	{
		balls = GameObject.FindGameObjectsWithTag("Ball");
		cue = GameObject.FindGameObjectWithTag("Cue");
		_cue = cue.GetComponent<Cue> ();
		cueMesh = GameObject.Find("cueMesh");
		isOnTurn = false;

		ballFinished = new List<GameObject>();
		stBallFinish = new List<GameObject> ();
		ndBallFinished = new List<GameObject> ();

		turnNumber = Random.Range(1,2);
		turnStyle = 0;
		isForceNextTurn = false;

		allBallStatic = true;
		breaked = false;
		player1Count = 0;
		player2Count = 0;
		prePlayer1Count = 0;
		prePlayer2Count = 0;
		nextTurnActive = true;
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit ();
		}

		ManagerBalls ();
		ManagerStaff ();

		if (turnStyle != 0)
			OrderBallPos();

		if (!isOnTurn)
			currentBallFinished = ballFinished.Count;
	}

	void ManagerBalls()
	{
		CheckBallIsStatic ();
		ManageBallFinished ();
	}

	void ManageBallFinished()
	{
		for (int i = 0; i < balls.Length; i++)
		{
			BallVelocity ballVelocity = balls[i].GetComponent<BallVelocity>();

			if (allBallStatic)
			{
				if (ballVelocity.isFinished && (!ballVelocity.isAddedToArray))
				{
					if (!breaked)
					{
					balls[i].transform.position = new Vector3 ( 1 + ballFinished.Count * 0.5f, 4, -2.6f);
					}
					else
					{
						if (turnStyle == 0)
						{
							switch (string.Compare(balls[i].name, "ball08"))
							{
							case 1:
								if (turnNumber == 1)
								{
									turnStyle = 2;
								}
								else
								{
									turnStyle = 1;
								}
								break;
							
							case -1:
								if (turnNumber == 1)
								{
									turnStyle = 1;
								}
								else
								{
									turnStyle = 2;
								}
								break;
							}
							Debug.Log("turnStyle" + turnStyle);
						}
					}
					ballFinished.Add(balls[i]);
					ballVelocity.isAddedToArray = true;
					balls[i].rigidbody.useGravity = false;
					balls[i].rigidbody.isKinematic = true;
				}
			}
		}
	}

	void CheckBallIsStatic()
	{
		allBallStatic = true;
		
		for (int i = 0; i < balls.Length; i++)
		{
			BallVelocity ball = balls[i].GetComponent<BallVelocity>();
			
			if (!ball.isStatic)
			{
				allBallStatic = false;
				break;
			}
		}
	}

	void OrderBallPos()
	{
		player1Count = 0;
		player2Count = 0;

		for (int i = 0; i < ballFinished.Count; i++)
		{
			if (turnStyle == 1)
			{
				if (string.Compare(ballFinished[i].name, "ball08") == 1)
				{
					player2Count ++;
					ballFinished[i].transform.position = new Vector3 ( 1 + player2Count * 0.5f, 4, 2.6f);
				}
				else
				{
					player1Count ++;
					ballFinished[i].transform.position = new Vector3 ( -3 + player1Count * 0.5f, 4, 2.6f);
				}
			}
			else
			{
				if (string.Compare(ballFinished[i].name, "ball08") == -1)
				{
					player2Count ++;
					ballFinished[i].transform.position = new Vector3 ( 1 + player2Count * 0.5f, 4, 2.6f);
				}
				else
				{
					player1Count ++;
					ballFinished[i].transform.position = new Vector3 ( -3 + player1Count * 0.5f, 4, 2.6f);
				}
			}

			ballFinished[i].transform.eulerAngles = new Vector3 (90, 0, 0);
		}

		if (turnNumber == 1)
		{
			if (player1Count == prePlayer1Count)
				nextTurn();
		}

		if (turnNumber == 2)
		{
			if (player2Count == prePlayer2Count)
				nextTurn();
		}
	}

	void ManagerStaff()
	{
		if (allBallStatic)
		{
			cue.SetActive(true);
			staffDirection.SetActive(true);
			staffTarget.SetActive(true);

			if (CueBall.ballFirstShoted && (!breaked))
			{
				breaked = true;
				Debug.Log("changed");
			}
		}
		else
		{
			cue.SetActive(false);
			isOnTurn = true;
			nextTurnActive = true;

			if (!CueBall.ballFirstShoted)
				CueBall.ballFirstShoted = true;
		}
	}

	public void nextTurn()
	{
		if (nextTurnActive)
		{
			if (turnNumber == 1)
				turnNumber = 2;
			else 
				turnNumber = 1;

			nextTurnActive = false;
		}

		Debug.Log ("turnStyle" + turnStyle);
	}
}
