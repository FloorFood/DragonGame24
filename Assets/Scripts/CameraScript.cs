using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    

    //[SerializeField] GameObject target;
    private Vector3 targetArea = Vector3.zero;
    [SerializeField] CharacterController2D character;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float lookAheadDistance = 10f, lookAheadSpeed = 3f;
    private float lookOffset;
    private bool isFalling;
    [SerializeField] float maxVerticalOffset = 5f;

    [SerializeField] float panSpeed = 3f;
    [SerializeField] GameObject player;

    private void Start()
    {
        targetArea = new Vector3(character.transform.position.x, character.transform.position.y + 1.26f, transform.position.z);
    }

    void FixedUpdate()
    {
        
            if (character.m_Grounded)
            {
                targetArea.y = character.transform.position.y + 1.26f;
            }

            if(transform.position.y - character.transform.position.y > maxVerticalOffset)
            {
                isFalling = true;
            }

            if (isFalling)
            {
                targetArea.y = character.transform.position.y;

                if (character.m_Grounded)
                {
                    isFalling = false;
                }
            }
            
            
                if (character.m_Rigidbody2D.velocity.x > 0.5f)
                {
                    lookOffset = Mathf.Lerp(lookOffset, lookAheadDistance, lookAheadSpeed * Time.deltaTime);
                }
                if (character.m_Rigidbody2D.velocity.x < -0.5f)
                {
                    lookOffset = Mathf.Lerp(lookOffset, -lookAheadDistance, lookAheadSpeed * Time.deltaTime);
                }

            

            targetArea.x = character.transform.position.x + lookOffset;

            transform.position = Vector3.Lerp(transform.position, targetArea, moveSpeed * Time.deltaTime);

    }

}
