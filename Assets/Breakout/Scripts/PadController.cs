using UnityEngine;

public class PadController : MonoBehaviour
{
	public float velocity = 2;
	private void Update ()
	{
		transform.Translate(velocity * Input.GetAxis("Horizontal") * Time.deltaTime,0,0);
		transform.position = new Vector3 (Mathf.Clamp (transform.position.x, -13.7f, 13.7f), transform.position.y, 0f);
	}
}
