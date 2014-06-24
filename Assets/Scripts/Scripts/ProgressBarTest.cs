using UnityEngine;
using System.Collections;

public class ProgressBarTest : MonoBehaviour {
	public float barDisplay1;
	public float barDisplay2;
	public Vector2 pos = new Vector2(450,40);
	public Vector2 pos2 = new Vector2 (520, 40);
	public Vector2 size = new Vector2(60,20);
	public Texture2D emptyTex;
	public Texture2D fullTex;

	public GUIStyle Style1, Style2;

	
	void OnGUI() {
		//draw the background:
		GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
		GUI.Box(new Rect(0,0, size.x, size.y), emptyTex);
		
		//draw the filled-in part:
		GUI.BeginGroup(new Rect(0,0, size.x * barDisplay1, size.y));
		GUI.Box(new Rect(0,0, size.x, size.y), fullTex);
		GUI.EndGroup();
		GUI.EndGroup();

		GUI.BeginGroup(new Rect(pos2.x, pos2.y, size.x, size.y));
		GUI.Box(new Rect(0,0, size.x, size.y), emptyTex);
		
		//draw the filled-in part:
		GUI.BeginGroup(new Rect(0,0, size.x * barDisplay2, size.y));
		GUI.Box(new Rect(0,0, size.x, size.y), fullTex);
		GUI.EndGroup();
		GUI.EndGroup();
	}
	
	void Update() {
		//for this example, the bar display is linked to the current time,
		//however you would set this value based on your desired display
		//eg, the loading progress, the player's health, or whatever.
		barDisplay1 = Time.time*0.5f;
		barDisplay2 += Time.deltaTime;
		//   barDisplay = MyControlScript.staticHealth;
	}
}