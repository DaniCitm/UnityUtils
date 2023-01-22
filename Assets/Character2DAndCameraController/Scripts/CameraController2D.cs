using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    public Transform target;
    public float speed = 2;

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * speed);
        float y = Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime * speed);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
