using UnityEngine;
using System.Collections;

public class Nhap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

//		if (ball.isFinished)
//		{
//			if (balls[i].name == "WhiteBall")
//			{
//				if (allBallStatic)
//				{
//					Debug.Log("Static");
//					balls[i].transform.position = ball.firstPosition;
//					balls[i].rigidbody.velocity = Vector3.zero;
//					balls[i].rigidbody.useGravity = true;
//					ball.isFinished = false;
//					//					renderer.enabled = true;
//				}
//			}
//			else if ((!ball.isAddedToArray))
//			{
//				if (turnStyle == 0)
//				{
//					if (turnNumber == 1)
//					{
//						if (string.Compare(balls[i].transform.name, "Ball2") < 0)
//						{
//							turnStyle = 1;
//						}
//						else
//						{
//							turnStyle = 2;
//						}
//						
//						stBallFinish.Add(balls[i]);
//						balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
//					}
//					else
//					{
//						if (string.Compare(balls[i].transform.name, "Ball2") > 0)
//						{
//							turnStyle = 1;
//						}
//						else
//						{
//							turnStyle = 2;
//						}
//						
//						ndBallFinished.Add(balls[i]);
//						balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
//					}
//				}
//				else 
//				{
//					if (turnStyle == 1)
//					{
//						if (turnNumber == 1)
//						{
//							if (string.Compare(balls[i].transform.name, "Ball2") < 0)
//							{
//								stBallFinish.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
//							}
//							else if (string.Compare(balls[i].transform.name, "Ball2") > 0)
//							{
//								ndBallFinished.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
//								isForceNextTurn = true;
//							}
//						}
//						else
//						{
//							if (string.Compare(balls[i].transform.name, "Ball2") > 0)
//							{
//								ndBallFinished.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
//							}
//							else if (string.Compare(balls[i].transform.name, "Ball2") < 0)
//							{
//								stBallFinish.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
//								isForceNextTurn = true;
//							}
//						}
//					}
//					else
//					{
//						if (turnNumber == 2)
//						{
//							if (string.Compare(balls[i].transform.name, "Ball2") < 0)
//							{
//								ndBallFinished.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
//							}
//							else if (string.Compare(balls[i].transform.name, "Ball2") > 0)
//							{
//								stBallFinish.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
//								isForceNextTurn = true;
//							}
//						}
//						else
//						{
//							if (string.Compare(balls[i].transform.name, "Ball2") > 0)
//							{
//								stBallFinish.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -5 + 0.4f * (stBallFinish.Count - 1), 4.5f, 2);
//							}
//							else if (string.Compare(balls[i].transform.name, "Ball2") < 0)
//							{
//								ndBallFinished.Add(balls[i]);
//								balls[i].transform.position = new Vector3( -1 + 0.4f * (ndBallFinished.Count - 1), 4.5f, 2);
//								isForceNextTurn = true;
//							}
//						}
//					}
//				}
//				
//				ballFinished.Add(balls[i]);
//				ball.isAddedToArray = true;
//				
//				balls[i].rigidbody.useGravity = false;
//				balls[i].rigidbody.velocity = Vector3.zero;
//				balls[i].transform.localScale = ballScaleSize;
//				balls[i].transform.LookAt(Camera.main.transform.position);
//				//				balls[i].transform.eulerAngles = Vector3.zero;
//			}
//			balls[i].collider.isTrigger = false;
//		}
	}
}
