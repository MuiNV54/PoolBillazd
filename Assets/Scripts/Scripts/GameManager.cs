using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour 
{
	public IList<GameObject> ballFinished;

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
	public int currentBallFinished;
	public int turnStyle;
	public int playerCountball;
	public int prePlayerCountBall;
	public bool nextTurnActive;

	public Vector3 ballScaleSize = new Vector3(0.3f, 0.3f, 0.3f);

	/// <summary>
	/// Network papam
	/// </summary>
	public GUIStyle HiglightStyle;
	private string UserNameInput = "my name";
	private string InputLine = string.Empty;        // stores input for gui
	private const int MaxInput = 12; 

	void Start () 
	{
		balls = GameObject.FindGameObjectsWithTag("Ball");
		cue = GameObject.FindGameObjectWithTag("Cue");
		_cue = cue.GetComponent<Cue> ();
		cueMesh = GameObject.Find("cueMesh");
		isOnTurn = false;

		ballFinished = new List<GameObject>();

		turnStyle = 0;
		isForceNextTurn = false;

		allBallStatic = true;
		breaked = false;
		playerCountball = 0;
		prePlayerCountBall = 0;
		nextTurnActive = true;
	}
	
	void Update () 
	{
		if(turnStyle == 0)
			turnStyle = this._cue.NetworkCom.turnStyle;
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
								turnStyle =2;
								// dispatch Style to the opponent
								this._cue.NetworkCom.SendStyle(1);
								break;
							case -1:
								turnStyle =1;
								// dispatch Style to the opponent
								this._cue.NetworkCom.SendStyle(2);
								break;
							}
							// Debug.Log("turnStyle" + turnStyle);
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
		playerCountball = 0;

		for (int i = 0; i < ballFinished.Count; i++)
		{
			if (turnStyle == 2)
			{
				if (string.Compare(ballFinished[i].name, "ball08") == 1)
				{
					playerCountball ++;
					ballFinished[i].transform.position = new Vector3 ( 1 + playerCountball * 0.5f, 4, 2.6f);
				}
				else
					ballFinished[i].transform.position = new Vector3 ( -3 + playerCountball * 0.5f, 4, 2.6f);
			}
			else
			{
				if (string.Compare(ballFinished[i].name, "ball08") == -1)
				{
					playerCountball ++;
					ballFinished[i].transform.position = new Vector3 ( 1 + playerCountball * 0.5f, 4, 2.6f);
				}
				else
					ballFinished[i].transform.position = new Vector3 ( -3 + playerCountball * 0.5f, 4, 2.6f);
			}
			ballFinished[i].transform.eulerAngles = new Vector3 (90, 0, 0);
		}

		if (playerCountball == prePlayerCountBall)
		{	
			if (isOnTurn==true)
				nextTurn();
		} 
		else 
		{
			prePlayerCountBall = playerCountball;
			nextTurnActive = false;
		}

	}

	void ManagerStaff()
	{
		if (allBallStatic)
		{
			isOnTurn = true;
			cue.SetActive(true);

			Cue cueScripts = cue.GetComponent<Cue>();
			if (!cueScripts.staffMoveEnabled)
			{
				staffDirection.SetActive(true);
				staffTarget.SetActive(true);
			}

			if (CueBall.ballFirstShoted && (!breaked))
			{
				breaked = true;
				Debug.Log("changed");
			}
		}
		else
		{
			cue.SetActive(false);
			isOnTurn = false;
			nextTurnActive = true;

			if (!CueBall.ballFirstShoted)
				CueBall.ballFirstShoted = true;
		}
	}

	public void nextTurn()
	{
		if (nextTurnActive)
		{
			this._cue.isPlayable = false;
			this._cue.NetworkCom.isNextPlayer = false;
			this._cue.NetworkCom.SendTurnChange(true);
			nextTurnActive = false;
		}

	}

	/// OnGUI func
	void OnGUI()
	{
		/// State machine
		switch (_cue.NetworkCom.AppState)
		{
		case NetworkClient.NetworkStateOption.Offline:
			GUILayout.BeginArea(new Rect(Screen.width / 4, Screen.height / 3, Screen.width / 2, Screen.height / 2));
			GUILayout.Label("Billiard Demo", HiglightStyle);
			GUILayout.Label("Offline. ServerAddress: " + _cue.NetworkCom.ServerAddress, HiglightStyle);
			GUILayout.Label(_cue.NetworkCom.OfflineReason);
			if (GUILayout.Button("Connect"))
			{
				_cue.NetworkCom.Connect();
			}
			GUILayout.EndArea();
			break;
		case NetworkClient.NetworkStateOption.OutsideOfRooms:
			bool joinRoom = false;
			GUILayout.BeginArea(new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 2));
			GUILayout.Label ("Billiard Demo", HiglightStyle);
			GUILayout.Label ("Enter a nickname:");
			this.UserNameInput = GUILayout.TextField(this.UserNameInput, MaxInput);
			joinRoom = GUILayout.Button ("Proceed to Lobby");
			GUILayout.EndArea();
			/// If the user pressed enter or the button, proceed to the lobby
			if (joinRoom || Event.current.keyCode == KeyCode.Return)
			{
				_cue.NetworkCom.JoinLobby("billiard_lobby");
				_cue.NetworkCom.UserName = this.UserNameInput;
				this.InputLine = "my room";
			}
			break;
		case NetworkClient.NetworkStateOption.InLobbyRoom:
			// this screen shows available billiard-rooms within out current lobby. The user can select one or enter a new roomname to join
			string roomToJoin = null;
			GUILayout.BeginArea(new Rect (Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 2));
			GUILayout.Label("Lobby", HiglightStyle);
			GUILayout.Label(_cue.NetworkCom.UserName + ", select a room, or enter room name");
			foreach (string roomName in _cue.NetworkCom.RoomHashtable.Keys)
			{
				if(Convert.ToInt32(_cue.NetworkCom.RoomHashtable[roomName])<2)
				{
					string buttonText = string.Format ("{0} ({1})",roomName, _cue.NetworkCom.RoomHashtable[roomName]);
					if(GUILayout.Button(buttonText))
						roomToJoin = roomName;
				}
			}
			// input for "new room"
			this.InputLine = GUILayout.TextField(this.InputLine, MaxInput);
			if (GUILayout.Button("Enter") || (Event.current.keyCode == KeyCode.Return))
			{
				roomToJoin = this.InputLine;
				this._cue.isPlayable = true;
			}
			GUILayout.EndArea();

			// join the room
			if (!string.IsNullOrEmpty(roomToJoin))
			{
				// call the wrapper method 
				_cue.NetworkCom.JoinRoomFromLobby(roomToJoin);
				this.InputLine = string.Empty;
			}
			break;
		case NetworkClient.NetworkStateOption.InRoom:
			// Control players turn
			GUI.Box(new Rect(10,10,100,90),"Menu");
			string Text ="";
			if (this.turnStyle ==1)
				Text = "Solid";
			else if (this.turnStyle ==2)
				Text = "Stripe";
			else
				Text = "not assigned";
			GUI.Label(new Rect(20,40,80,20), Text);
			break;
		}
	}
}
