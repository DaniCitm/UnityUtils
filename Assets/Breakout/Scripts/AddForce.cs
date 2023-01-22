using UnityEngine;

public class AddForce : MonoBehaviour
{
	Rigidbody myRigidbody;

	public float velocity = 10;
	public Vector2[] initialForces;
	private void Awake()
	{
		myRigidbody = GetComponent<Rigidbody>();
	}
	private void OnEnable ()
	{
		myRigidbody.velocity = Vector3.zero;
		myRigidbody.AddForce(initialForces[Random.Range(0,initialForces.Length)].normalized * velocity, ForceMode.VelocityChange);
	}

	private void Update ()
	{
		
	}
}
