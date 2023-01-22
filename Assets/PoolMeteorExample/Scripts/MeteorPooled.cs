using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorPooled : MonoBehaviour
{
	//Trigger example
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Destructor"))
        {
            gameObject.Despawn();
        }
    }
}
