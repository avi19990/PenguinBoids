using Cinemachine;
using UnityEngine;

public class MovingController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 15;
    [SerializeField] private CinemachineVirtualCamera camera;

    private Vector2 input;
    private float mouseXInput;
    private Vector3 movingDirection;

    private void Update()
    {
        GetInput();
        GetMouseInput();
        MovePlayer();
    }

    private void GetMouseInput()
    {
        mouseXInput = Input.GetAxis("Mouse X");
    }


    private void MovePlayer()
    {
        var cameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
        var cameraRight = Quaternion.Euler(0, 90, 0) * cameraForward;
        transform.position += input.x * cameraRight + input.y * cameraForward;
    }

    private void GetInput()
    {
        var xInput = Input.GetAxis("Horizontal");
        var yInput = Input.GetAxis("Vertical");
        input = new Vector2(xInput, yInput);
    }
}