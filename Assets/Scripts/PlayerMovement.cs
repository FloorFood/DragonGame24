using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    
    float horizontalMove = 0f;
    public float moveSpeed = 40f;
    public float walkSpeed = 40f;
    public float runSpeed = 80f;
    bool jump = false;
    bool hasJumped = false;
    //bool cutScene = false;
    //bool sounded = false;
    //int currentscene;

    
    // Update is called once per frame
    void Update()
    {
        
            horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
            if (Input.GetAxisRaw("Horizontal") < 0f && Input.GetAxisRaw("Horizontal") > -0.1f || Input.GetAxisRaw("Horizontal") > 0f && Input.GetAxisRaw("Horizontal") < 0.1f)
            {
                horizontalMove = 0f;
            }
            if (Input.GetAxisRaw("Horizontal") < -0.1f)
            {
                horizontalMove = -1f *moveSpeed;
            }
            if (Input.GetAxisRaw("Horizontal") > 0.1f)
            {
                horizontalMove = 1f * moveSpeed;
            }


            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            animator.SetTrigger("Jumping");
        }

            

            /*if (Input.GetAxisRaw("Horizontal") != 0 && !sounded)
            {
                StartCoroutine(WaitForStep());
                
                    AudioManager.instance.shouldRandomizePitch = true;
                    AudioManager.instance.PlaySound("DragonStep");
                
            }*/
            /*if (Input.GetAxisRaw("Horizontal") == 0 && sounded)
            {
                AudioManager.instance.StopSound("SnowStep");
                sounded = false;
            }*/

        

        

        /*if (Input.GetButtonDown("Cancel") && paused)
        {
            paused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("Jumping", false);
        }
    }

    /*IEnumerator WaitForStep()
    {
        yield return new WaitForSeconds(0.389f);
        sounded = false;
    }*/
    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
