using UnityEngine;
using System.Collections;

public class PlayerTurn : MonoBehaviour 
{
	public int turnNumber;

	void Start () 
	{
		turnNumber = 1;
	}
	
	void Update ()
	{
		
	}

	void NextTurn()
	{
		if (turnNumber == 1)
			turnNumber = 2;
		else 
			turnNumber = 1;
	}
}
