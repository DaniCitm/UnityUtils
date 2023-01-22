using UnityEngine;

public class Ball : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		if(other.collider.tag == "Brick")
		{
			ScorePoint();
			Destroy(other.collider.gameObject);
		}
	}

	private void ScorePoint()
	{
		GameData.score++;
		//TO DO Scorepoint
		CheckVictory();
	}

	private void CheckVictory()
	{
		
	}
}
