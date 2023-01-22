using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
	//Trigger example
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Destructor"))
        {
            Destroy(gameObject);
        }
    }
}
