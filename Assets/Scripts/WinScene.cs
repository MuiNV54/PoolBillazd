using UnityEngine;
using System.Collections;

public class WinScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.Button (new Rect (100, 100, 200, 200), GameManager.nameWin + "Win CMNR");
	}
}
