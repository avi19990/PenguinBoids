using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

namespace Player
{
    public enum CameraType
    {
        TopDown,
        FirstPerson
    }
    public class CameraController : MonoBehaviour
    {
        private List<CinemachineVirtualCamera> cameras;
        [SerializeField] private GameObject playerMesh;
        

        private CameraType currentIndex;
        
        public Vector3 CameraForward => Vector3.ProjectOnPlane(cameras[(int)currentIndex].transform.forward, Vector3.up).normalized;

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
            SwitchPlayerMesh();
            SwitchCamera(true);
        }

        private void SwitchPlayerMesh()
        {
            playerMesh.SetActive(currentIndex == CameraType.TopDown);
        }

        private static bool HasInput()
        {
            return Input.GetKeyDown(KeyCode.R);
        }

        private void DisableAllCameras()
        {
            cameras.ForEach(camera => camera.gameObject.SetActive(false));
        }

        private void SwitchCamera(bool shouldEnable = false)
        {
            cameras[(int)currentIndex].gameObject.SetActive(shouldEnable);
        }

        private void IncreaseIndex()
        {
            currentIndex = (CameraType)((int)(currentIndex + 1) % cameras.Count);
        }

        private void GetReferences()
        {
            cameras = GetComponentsInChildren<CinemachineVirtualCamera>().ToList();
        }
    }
}