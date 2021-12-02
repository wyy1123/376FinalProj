using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 m_Movement;
    Animator m_Animator;
    //rotation is represented as a quaternion
    Quaternion m_Rotation = Quaternion.identity;
    Rigidbody m_Rigidbody;

    // a circle has 2 pi, approx 6, turnspeed=3 means it takes 1 sec to turn around
    public float turnSpeed = 20f;
    public float moveSpeed = 0.06f;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    //We need to use fixed update bc onanimator move is updated per physics update, not rendering
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        //normalizes it so that it doesnt move faster diagonally
        m_Movement.Normalize();

        //checks for both vertical and horizontal movements
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        //if both are approx 0, then we think the character is not moving
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("IsWalking", isWalking);
        //Debug.Log(isWalking);

        //generate the rotation vector: needs to time speed by delta time, bc this line gets called 60 times
        // per second if it update gets called 60 times per second,
        // we do so because turnspeed is the rotation speed(change in angle) we want per second
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);

        //This line simply calls the LookRotation method and creates a rotation looking in the direction of the given parameter. 
        m_Rotation = Quaternion.LookRotation(desiredForward);

    }

    //allows you to apply root motion as you want,
    //which means that movement and rotation can be applied separately.
    private void OnAnimatorMove()
    {
        //Animator’s deltaPosition is the change in position due to root motion that would have been applied to this frame.
        //You’re taking the magnitude of that (the length of it) and multiplying by the
        //movement vector which is in the actual direction we want the character to move.
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * moveSpeed);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}
