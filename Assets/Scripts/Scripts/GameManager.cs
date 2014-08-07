using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour 
{
	public IList<GameObject> ballFinished;

	public bool breaked;
	public bool ballInHand;
	public bool foul;
	public bool cueBallFail;
	public bool ball8Enable;                // check when all ball ate, and can eat ball 8
	public bool gameEnd;    				// check when have ball 8 in pocket -> endgame
	public static string nameWin;			// name of player Win
	public bool checkFirstTurn;             // available to reject first check after have turnstyle

	public GameObject timerBar1;
	public GameObject timerBar2;

	private EnergyBarRenderer energyBarRenderer1;
	private EnergyBarRenderer energyBarRenderer2;
	private EnergyBar energyBar1;
	private EnergyBar energyBar2;

	public int energyValue;

	public float turnTimer;					// timer for 1 turn
	public float resetTurnTimer;

	public bool ballImpactRail;

	public GameObject[] balls;
	private GameObject cue;
	private Cue _cue;
	private GameObject cueMesh;

	public GameObject staffDirection;
	public GameObject staffTarget;
	public GameObject cueBall;
	private CueBall _cueBall;

	public bool allBallStatic;
	public bool isOnTurn;
	public bool isForceNextTurn;
	public int currentBallFinished;
	public int turnStyle;
	public int playerCountball;
	public int preBallFinished;               //number of ball finished previous

	public int prePlayerCountBall;
	public bool nextTurnActive;
	public bool checkedBallCount;

	public float[] ballPosX;
	public float[] ballPosY;
	public float[] ballPosZ;

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
		ballPosX = new float[16];
		ballPosY = new float[16];
		ballPosZ = new float[16];

		checkFirstTurn = false;
		gameEnd = false;
		nameWin = "";

		turnTimer = 40f;
		resetTurnTimer = turnTimer;
		energyValue = 0;

		energyBarRenderer1 = timerBar1.GetComponent<EnergyBarRenderer> ();
		energyBarRenderer2 = timerBar2.GetComponent<EnergyBarRenderer> ();
		energyBar1 = timerBar1.GetComponent<EnergyBar> ();
		energyBar2 = timerBar2.GetComponent<EnergyBar> ();

		balls = GameObject.FindGameObjectsWithTag("Ball");
		cue = GameObject.FindGameObjectWithTag("Cue");
		_cue = cue.GetComponent<Cue> ();
		cueMesh = GameObject.Find("cueMesh");
		_cueBall = cueBall.GetComponent<CueBall> ();

		isOnTurn = false;
		ballInHand = false;
		foul = false;
		ball8Enable = false;

		ballFinished = new List<GameObject>();

		turnStyle = 0;
		isForceNextTurn = false;

		allBallStatic = true;
		breaked = false;
		playerCountball = 0;
		prePlayerCountBall = 0;
		preBallFinished = 0;
		nextTurnActive = true;
		checkedBallCount = true;
		ballImpactRail = false;
		cueBallFail = false;
	}
	
	void Update () 
	{
		sendBallPosition ();

		if (!_cue.isPlayable && !checkFirstTurn)
			checkFirstTurn = this._cue.NetworkCom.checkFirstTurn;

		CheckGameEnd ();

		DisplayTimer ();

		if(turnStyle == 0)
			turnStyle = this._cue.NetworkCom.turnStyle;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit ();
		}

		ManagerBalls ();
		ManagerStaff ();

		if ((_cue.isPlayable) && allBallStatic)
		{
			turnTimer -= Time.deltaTime;
		}

		if (turnTimer <= 0)
		{
			foul = true;
			nextTurn();
		}
		else
		{
			if (allBallStatic)
			{
				ManageChangeTurn();
			}
		}
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
							Debug.Log(turnStyle);
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

	void DisplayTimer()
	{
		if (_cue.isPlayable)
		{
			energyBarRenderer1.textureBarColorType = EnergyBarRenderer.ColorType.Solid;
			energyBarRenderer2.textureBarColorType = EnergyBarRenderer.ColorType.Gradient;
			energyBar1.valueCurrent = 100 - (int) (turnTimer * 100f / resetTurnTimer);

			//send percent turntimer 
			energyValue = energyBar1.valueCurrent;
			this._cue.NetworkCom.SendEnergyValue(energyValue);
		}
		else
		{
			energyBarRenderer1.textureBarColorType = EnergyBarRenderer.ColorType.Gradient;
			energyBarRenderer2.textureBarColorType = EnergyBarRenderer.ColorType.Solid;
			energyBar2.valueCurrent = this._cue.NetworkCom.energyBarPercent;
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
					ballFinished[i].transform.position = new Vector3 ( -3 + playerCountball * 0.5f, 4, 2.6f);
				}
				else
					ballFinished[i].transform.position = new Vector3 ( 1 + playerCountball * 0.5f, 4, 2.6f);
			}
			else
			{
				if (string.Compare(ballFinished[i].name, "ball08") == -1)
				{
					playerCountball ++;
					ballFinished[i].transform.position = new Vector3 ( -3 + playerCountball * 0.5f, 4, 2.6f);
				}
				else
					ballFinished[i].transform.position = new Vector3 ( 1 + playerCountball * 0.5f, 4, 2.6f);
			}
			ballFinished[i].transform.eulerAngles = new Vector3 (90, 0, 0);
		}

		if (playerCountball == 8)
		{
			ball8Enable = true;
		}

		checkedBallCount = true;
	}

	void ManagerStaff()
	{
		if (allBallStatic)
		{
			isOnTurn = false;

			if (!ballInHand)
				cue.SetActive(true);

			Cue cueScripts = cue.GetComponent<Cue>();
			if (!cueScripts.staffMoveEnabled)
			{
				staffDirection.SetActive(true);
				staffTarget.SetActive(true);
			}

			nextTurnActive = true;
		}
		else
		{
			cue.SetActive(false);
			isOnTurn = true;

			if (!_cueBall.ballFirstShoted)
			{
				_cueBall.ballFirstShoted = true;
			}
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
			cue.SetActive(true);

			if (foul)
			{
				this.ballInHand = false;
				this._cue.NetworkCom.foul = false;
				this._cue.NetworkCom.SendBallInHand(true);
			}

			turnTimer = resetTurnTimer;
		}
	}

	void ManageChangeTurn()
	{
		if ((!checkedBallCount) && (_cueBall.ballFirstShoted))
		{
			if (turnStyle != 0)
			{
				OrderBallPos();
			}

			// process when cueball in pocket
			if (cueBallFail)
			{
				foul = true;
				cueBallFail = false;
				this._cue.NetworkCom.SendBallFail(true);
				_cueBall.OnCueBallFail();

				Debug.Log("Cue ball Fail");
				nextTurn();
			}

			// Cue ball collision with no ball && turn style different 0
			if ((turnStyle != 0) && checkFirstTurn)
			{
				if (_cueBall.firstBallCollision == "")
				{
					foul = true;
					Debug.Log ("not impact ball");
					nextTurn();
				}
				else
				{
					// cueball collision with wrong ball
					if (_cueBall.wrongBall)
					{
						foul = true;
						_cueBall.wrongBall = false;
						nextTurn();
					}
					
					_cueBall.firstBallCollision = "";
				}

				checkFirstTurn = true;
				this._cue.NetworkCom.SendCheckFirstTurn(true);
			}

			// process when amount ball of player no change
			if (ballFinished.Count == preBallFinished)
			{

				// ball collision with no rail
				if (!ballImpactRail)
				{
					foul = true;
					Debug.Log("ball no rail");
				}
				else
				{
					ballImpactRail = false;
				}
				
				nextTurn();
			}
			else
			{
				preBallFinished = ballFinished.Count;
				if (turnStyle != 0)
				{
					if (playerCountball == prePlayerCountBall)
					{	
						if (!isOnTurn)
						{
							if (_cueBall.ballFirstShoted && !checkedBallCount)
							{
								checkedBallCount = true;
								nextTurn();
							}
						}
					} 
					else 
					{
						prePlayerCountBall = playerCountball;
					}
				}
			}
			
			checkedBallCount = true;
		}
	}

	void CheckGameEnd()
	{
		if (gameEnd)
		{
			Application.LoadLevel("WinScene");
		}
		
		if (!_cue.isPlayable)
		{
			nameWin = this._cue.NetworkCom.nameWin;
			gameEnd = this._cue.NetworkCom.endGame;
		}
	}

	void sendBallPosition()
	{
		if (_cue.isPlayable)
		{
			ballPosX[0] = cueBall.transform.position.x;
			ballPosY[0] = cueBall.transform.position.y;
			ballPosZ[0] = cueBall.transform.position.z;

			for (int i = 1; i< 16; i++)
			{
				string s;
				s = i.ToString();
				if (i < 10)
					s = "0" + s;

				GameObject ball = GameObject.Find("ball" + s);
				if (ball != null)
				{
					ballPosX[i] = ball.transform.position.x;
					ballPosY[i] = ball.transform.position.y;
					ballPosZ[i] = ball.transform.position.z;
				}
			}

			this._cue.NetworkCom.SendBallPosX(ballPosX);
			this._cue.NetworkCom.SendBallPosY(ballPosY);
			this._cue.NetworkCom.SendBallPosZ(ballPosZ);
		}
		else
		{
			cueBall.transform.position = new Vector3(this._cue.NetworkCom.ballPosX[0],
			                                         this._cue.NetworkCom.ballPosY[0],
			                                         this._cue.NetworkCom.ballPosZ[0]);

			for (int i = 1; i < 16; i++)
			{
				string s;
				s = i.ToString();
				if (i < 10)
					s = "0" + s;

				GameObject ball = GameObject.Find("ball" + s);
				if (ball != null)
				{
					ball.transform.position = new Vector3(this._cue.NetworkCom.ballPosX[i],
					                                      this._cue.NetworkCom.ballPosY[i],
					                                      this._cue.NetworkCom.ballPosZ[i]);
				}
			}
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
