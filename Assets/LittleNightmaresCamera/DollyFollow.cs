using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class DollyFollow : MonoBehaviour
{
    public float xLength;
    public Transform trackedObject;
    public CinemachineDollyCart cart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cart.m_Position = trackedObject.position.x / xLength;
    }
}
