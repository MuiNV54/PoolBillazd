using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public IList<GameObject> ballFinished;
	public IList<GameObject> stBallFinish;
	public IList<GameObject> ndBallFinished;

	private GameObject[] balls;
	private GameObject staff;
	private Staff _staff;

	public bool allBallStatic;
	public bool isOnTurn;
	public bool isForceNextTurn;
	public int turnNumber;
	public int currentBallFinished;
	public int turnStyle;

	public Vector3 ballScaleSize = new Vector3(0.3f, 0.3f, 0.3f);

	void Start () 
	{
		balls = GameObject.FindGameObjectsWithTag("WhiteBall");
		staff = GameObject.FindGameObjectWithTag("Staff");
		_staff = staff.GetComponent<Staff> ();
		isOnTurn = false;

		ballFinished = new List<GameObject>();
		stBallFinish = new List<GameObject> ();
		ndBallFinished = new List<GameObject> ();

		turnNumber = 1;
		turnStyle = 0;
		isForceNextTurn = false;
	}
	
	void Update () 
	{
		ManagerBalls ();
		ManagerStaff ();

		if (!isOnTurn)
			currentBallFinished = ballFinished.Count;
	}

	void ManagerBalls()
	{
		allBallStatic = true;
		
		for (int i = 0; i < balls.Length; i++)
		{
			BallVelocity ball = balls[i].GetComponent<BallVelocity>();
			
			if (ball.isFinished)
			{
				if (balls[i].name == "WhiteBall")
				{
					if (allBallStatic)
					{
						Debug.Log("Static");
						balls[i].transform.position = ball.firstPosition;
						balls[i].rigidbody.velocity = Vector3.zero;
						balls[i].rigidbody.useGravity = true;
						ball.isFinished = false;
						//					renderer.enabled = true;
					}
				}
				else if ((!ball.isAddedToArray))
				{
					if (turnStyle == 0)
					{
						if (turnNumber == 1)
						{
							if (string.Compare(balls[i].transform.name, "Ball2") < 0)
							{
								turnStyle = 1;
							}
							else
							{
								turnStyle = 2;
							}

							stBallFinish.Add(balls[i]);
							balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
						}
						else
						{
							if (string.Compare(balls[i].transform.name, "Ball2") > 0)
							{
								turnStyle = 1;
							}
							else
							{
								turnStyle = 2;
							}
							
							ndBallFinished.Add(balls[i]);
							balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
						}
					}
					else 
					{
					if (turnStyle == 1)
						{
							if (turnNumber == 1)
							{
								if (string.Compare(balls[i].transform.name, "Ball2") < 0)
								{
									stBallFinish.Add(balls[i]);
									balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
								}
								else if (string.Compare(balls[i].transform.name, "Ball2") > 0)
								{
									ndBallFinished.Add(balls[i]);
									balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
									isForceNextTurn = true;
								}
							}
							else
							{
								if (string.Compare(balls[i].transform.name, "Ball2") > 0)
								{
									ndBallFinished.Add(balls[i]);
									balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
								}
								else if (string.Compare(balls[i].transform.name, "Ball2") < 0)
								{
									stBallFinish.Add(balls[i]);
									balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
									isForceNextTurn = true;
								}
							}
						}
						else
						{
							if (turnNumber == 2)
							{
								if (string.Compare(balls[i].transform.name, "Ball2") < 0)
								{
									ndBallFinished.Add(balls[i]);
									balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
								}
								else if (string.Compare(balls[i].transform.name, "Ball2") > 0)
								{
									stBallFinish.Add(balls[i]);
									balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
									isForceNextTurn = true;
								}
							}
							else
							{
								if (string.Compare(balls[i].transform.name, "Ball2") > 0)
								{
									stBallFinish.Add(balls[i]);
									balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
								}
								else if (string.Compare(balls[i].transform.name, "Ball2") < 0)
								{
									ndBallFinished.Add(balls[i]);
									balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
									isForceNextTurn = true;
								}
							}
						}
					}

					ballFinished.Add(balls[i]);
					ball.isAddedToArray = true;

					balls[i].rigidbody.useGravity = false;
					balls[i].rigidbody.velocity = Vector3.zero;
					balls[i].transform.localScale = ballScaleSize;
					balls[i].transform.LookAt(Camera.main.transform.position);
					//				balls[i].transform.eulerAngles = Vector3.zero;
				}
				balls[i].collider.isTrigger = false;
			}
			
			if (!ball.isStatic)
			{
				allBallStatic = false;
				break;
			}
		}
	}

	void ManagerStaff()
	{
		if (allBallStatic)
		{
			staff.SetActive(true);
			if (isOnTurn)
			{
				if ((ballFinished.Count == currentBallFinished) || (isForceNextTurn))
				{
					nextTurn();
					isForceNextTurn = false;
					Debug.Log(turnNumber);
				}
			}
			isOnTurn = false;
		}
		else
		{
			staff.SetActive(false);
			isOnTurn = true;
		}
	}

	public void nextTurn()
	{
		if (turnNumber == 1)
			turnNumber = 2;
		else 
			turnNumber = 1;

		Debug.Log ("turnStyle" + turnStyle);
	}
}
