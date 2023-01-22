using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicVariableExample : MonoBehaviour
{
    public string logString = "cadena";
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
             Debug.Log(logString);
        }
    }
}
