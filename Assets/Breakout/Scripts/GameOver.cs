using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
	bool over = false;

	public GameObject gameOverText;
	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Ball")
		{
			over = true;
			gameOverText.SetActive(true);
			Debug.Log("Your score is: " + GameData.score);
			GameData.ResetScore();
		}
	}

	private void Update()
	{
		if(over && Input.anyKeyDown)
		{
			SceneManager.LoadScene(0);
		}
	}
}
