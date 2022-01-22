using System;
using Cinemachine;
using UnityEngine;

namespace Player
{
    public class MovingController : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;

        private Vector2 input;
        private Vector3 movingDirection;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            GetInput();
            MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 cameraForward = cameraController.CameraForward;
            Vector3 cameraRight = Quaternion.Euler(0, 90, 0) * cameraForward;
            transform.position += input.x * cameraRight + input.y * cameraForward;
        }

        private void GetInput()
        {
            float xInput = Input.GetAxis("Horizontal");
            float yInput = Input.GetAxis("Vertical");
            input = new Vector2(xInput, yInput);
        }
    }
}