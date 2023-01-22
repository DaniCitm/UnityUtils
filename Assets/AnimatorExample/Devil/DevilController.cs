using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilController : MonoBehaviour
{
    public Animator animator;
    public CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mov = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        characterController.Move(mov * Time.deltaTime * 2);

        animator.SetFloat("speed", mov.magnitude);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.Play("Victory");
        }
    }
}
