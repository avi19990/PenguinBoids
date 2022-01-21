using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private List<CinemachineVirtualCamera> cameras;

    private int currentIndex;

    private void Start()
    {
        GetReferences();
        DisableAllCameras();
        SwitchCamera();
    }

    private void Update()
    {
        if (!HasInput()) return;

        SwitchCamera();
        IncreaseIndex();
        SwitchCamera(true);
    }

    private void DisableAllCameras()
    {
        cameras.ForEach(camera => camera.gameObject.SetActive(false));
    }

    private void GetReferences()
    {
        cameras = GetComponentsInChildren<CinemachineVirtualCamera>().ToList();
    }

    private void SwitchCamera(bool shouldEnable = false)
    {
        cameras[currentIndex].gameObject.SetActive(shouldEnable);
    }

    private void IncreaseIndex()
    {
        currentIndex = (currentIndex + 1) % cameras.Count;
    }

    private static bool HasInput()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
}