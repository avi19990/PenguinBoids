using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MovingController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 15;
    [SerializeField] private CinemachineVirtualCamera camera;

    private void Update()
    {
        GetInput();
        GetMouseInput();
        RotatePlayer();
        MovePlayer();
    }

    private void GetMouseInput()
    {
        mouseXInput = Input.GetAxis("Mouse X");
    }

    private void RotatePlayer()
    {
        transform.Rotate(Vector3.up * mouseXInput);
    }

    private void MovePlayer()
    {
        transform.position += new Vector3(input.y, 0, input.x) * speed * Time.deltaTime;
    }

    private Vector2 input;
    private float mouseXInput;
    private Vector3 movingDirection;

    private void GetInput()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        input = new Vector2(xInput, yInput);
    }
}
