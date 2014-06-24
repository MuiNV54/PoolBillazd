using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{	
	public float unitForce = 500;

	void Start()
	{
	}

	public void ForceToBall(Vector3 direction)
	{
		rigidbody.AddForce(direction * unitForce);
	}
}
