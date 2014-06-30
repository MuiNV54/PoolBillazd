using UnityEngine;
using System.Collections;

public class RailController : MonoBehaviour {

	public Vector3 Normal_vector;
	public float scale = 1000000.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider col){
		if (col.attachedRigidbody)
		{
			if((Normal_vector.z != 0 && Normal_vector.z*col.gameObject.rigidbody.velocity.z<0) || (Normal_vector.x !=0 && Normal_vector.x*col.gameObject.rigidbody.velocity.x<0))
			{
				//		Debug.Log(col.gameObject.name);
				Vector3 Incoming_vel =col.gameObject.rigidbody.velocity;
				//		Debug.Log(Mathf.Cos(angle*Mathf.Deg2Rad));
				Vector3 Outcoming_vel = Vector3.Reflect(Incoming_vel/Incoming_vel.magnitude, Normal_vector);
				col.gameObject.rigidbody.velocity = Outcoming_vel*Incoming_vel.magnitude;
			}
		}
	}


}
