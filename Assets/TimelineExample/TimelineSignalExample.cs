using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineSignalExample : MonoBehaviour
{
	public GameObject[] gameObjectsToDeactivate;

    public void DeactivateSomething()
	{
		foreach (var item in gameObjectsToDeactivate)
		{
			item.SetActive(false);
		}
	}
}
