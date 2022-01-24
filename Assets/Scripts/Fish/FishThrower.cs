using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Fish
{
    public static class GameDataHolder
    {
        public static int FishAmount;
    }

    public class FishThrower : MonoBehaviour
    {
        [SerializeField] private Fish fishPrefab;
        [SerializeField] private CinemachineVirtualCamera camera;

        private Vector3 cameraForward;
        
        private void Update()
        {
            if (HasInput())
            {
                GetCameraDirection();
                InstantiateFish();
            }
        }

        private void GetCameraDirection()
        {
            cameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
        }

        private static bool HasInput()
        {
            return Input.GetKeyDown(KeyCode.Q);
        }

        private void InstantiateFish()
        {
            Transform appearingSpot = transform;
            Fish fish = Instantiate(fishPrefab, appearingSpot);
            Vector3 endPosition = GetEndPosition(appearingSpot);
            fish.transform.DOMove(endPosition, 1f);
        }

        private Vector3 GetEndPosition(Transform appearingSpot)
        {
            Vector3 endPosition = appearingSpot.position + cameraForward * 7;
            endPosition.y -= 5;
            return endPosition;
        }
    }
}