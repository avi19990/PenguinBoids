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

        private void Update()
        {
            if (HasInput())
            {
                InstantiateFish();
            }
        }

        private static bool HasInput()
        {
            return Input.GetKeyDown(KeyCode.Q);
        }

        private void InstantiateFish()
        {
            Transform appearingSpot = transform;
            appearingSpot.position += transform.forward;
            Fish fish = Instantiate(fishPrefab, appearingSpot);
            Vector3 endPosition = GetEndPosition(appearingSpot);
            fish.transform.DOMove(endPosition, 1f);
        }

        private Vector3 GetEndPosition(Transform appearingSpot)
        {
            Vector3 endPosition = appearingSpot.position + transform.forward * 4;
            endPosition.y -= 5;
            return endPosition;
        }
    }
}