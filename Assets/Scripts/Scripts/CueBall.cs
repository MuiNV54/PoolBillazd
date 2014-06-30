using UnityEngine;
using System.Collections;

public class CueBall : MonoBehaviour 
{	
	public float unitForce = 500;
	public static bool ballFirstShoted;

	void Start()
	{
		ballFirstShoted = false;
	}

	public void ForceToBall(Vector3 direction)
	{
		rigidbody.AddForce(direction * unitForce);
	}
}
