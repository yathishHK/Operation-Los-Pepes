using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController        controller;
    [SerializeField] Joystick                   joystick;
    [SerializeField] Transform                  groundCheck;
    [SerializeField] float                      sensivity = 10f; 
    [SerializeField] float                      movementSensivity = 10f;
    [SerializeField] float                      jumpHeight = 3f;
    [SerializeField] Camera                     playerCamera;
    [SerializeField] LayerMask                  groundMask;
    [SerializeField] LayerMask                  mask;
    

    float                                       screenPosX = 0f;
    float                                       screenPosY = 0f;
    float                                       xRotation = 0f;
    float                                       joystickY = 0f;
    float                                       gravity = -9.8f;
    float                                       joystickX = 0f;
    float                                       groundDistance = 0.3f;
    Vector2                                     startTouchPosition;
    Vector2                                     currentRotationPosition;
    Vector3                                     velocity;
    bool                                        isGrounded;
    bool                                        moveInAir = true;
    int                                         frameRateCount = 5;
    bool                                        buttonIsDown = false;
    
    void Update()
    {
        PlayerRotaion();
        PlayerMove();
        PlayerGravity();
    }

    private void PlayerGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void PlayerMove()
    {
       if(Input.touchCount > 0)
        {
            if (moveInAir)
            {
                joystickX = joystick.Horizontal * movementSensivity * Time.deltaTime;
                joystickY = joystick.Vertical * movementSensivity * Time.deltaTime;
            }

            ControlInAir();

            Vector3 move = transform.right * joystickX + transform.forward * joystickY;

            controller.Move(move);
        }
    }

    private void PlayerRotaion()
    {
        if(Input.touchCount > 0)
        {
            PlayerLookAround();
        }
    }

    private void PlayerLookAround()
    {
        //if (moveInAir)
        {
            foreach (Touch touch in Input.touches)
            {
                if (IsTouchOverUI(touch))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        startTouchPosition = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        currentRotationPosition = touch.position - startTouchPosition;
                        startTouchPosition = touch.position;
                        screenPosX = currentRotationPosition.x * sensivity * Time.deltaTime;
                        screenPosY = currentRotationPosition.y * sensivity * Time.deltaTime;
                        transform.Rotate(Vector3.up * screenPosX);
                        xRotation -= screenPosY;
                        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
                        playerCamera.transform.localEulerAngles = new Vector3(xRotation, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touch"></param>
    /// <returns></returns>
    private bool IsTouchOverUI(Touch touch)
    {
        //return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = touch.position;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if(raycastResults[i].gameObject.tag != "Rotation")
            {
                raycastResults.RemoveAt(i);
                i--;
            }
        }

        return raycastResults.Count > 0;
    }

    public void JumpPlayer()
    {
        if(isGrounded)
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void ControlInAir()
    {
        if (!isGrounded && moveInAir)
        {
            //frameRateCount--;
            //if (frameRateCount == 0)
            moveInAir = false;
        }

        else if (isGrounded)
        {
            moveInAir = true;
            //frameRateCount = 5;
        }
    }

    public void Fire()
    {

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, Mathf.Infinity, mask))
        {
            print(hit.transform.name);
            Debug.DrawRay(playerCamera.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
    }

}
