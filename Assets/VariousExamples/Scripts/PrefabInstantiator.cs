using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    public GameObject prefab;
    public Transform instantiatePositionTransform;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(prefab, instantiatePositionTransform.position, Quaternion.identity);
        }
    }
}
