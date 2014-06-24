using UnityEngine;
using System.Collections;

public class StaffDirection : MonoBehaviour 
{
	public GameObject staffTarget;
	public GameObject staffDirection;

	private Staff staff;

	void Start()
	{
		staff = GetComponent<Staff> ();
	}

	void Update()
	{
		if (staff.staffRotationEnabled)
		{
			DrawTarget ();
		}

		if (staff.staffMoveEnabled)
		{
			staffDirection.SetActive(false);
//			staffTarget.SetActive(false);
		}
	}
	
	void DrawTarget()
	{
		RaycastHit hit;
		Vector3 fwd = - transform.forward;

		if (Physics.Raycast(transform.position, fwd, out hit))
		{
			if ((hit.collider.tag == "WhiteBall") && (hit.collider.name != "WhiteBall"))
			{
				Debug.DrawLine(transform.position, hit.point);
			}

			DrawDirection (hit.point);
		}
	}

	void DrawDirection(Vector3 point)
	{
//		float lengthStaffDirection = Vector3.Magnitude(transform.position - staffTarget.transform.position);
		float lengthStaffDirection = Vector3.Magnitude(transform.position - point);
		staffDirection.transform.localScale = new Vector3(staffDirection.transform.localScale.x,
		                                                  staffDirection.transform.localScale.y,
		                                                  lengthStaffDirection);
		staffTarget.transform.localScale = new Vector3 (staffTarget.transform.localScale.x,
		                                               staffTarget.transform.localScale.y,
		                                                0.3f/lengthStaffDirection);

		staffDirection.transform.position = transform.position + new Vector3(0, 0.05f, 0);
		staffDirection.transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 
		                                                    transform.eulerAngles.y,
		                                                    transform.eulerAngles.z);
		staffDirection.SetActive(true);
	}
}