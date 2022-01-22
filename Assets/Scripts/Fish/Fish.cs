using System;
using UnityEngine;

namespace Fish
{
    public class Fish : MonoBehaviour
    {
        private float timer;
        private float livingTime = 20f;

        private const string BoidTag = "Boid";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(BoidTag))
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            transform.parent = null;
            GameDataHolder.FishAmount++;
        }

        private void Update()
        {
            if (HasDied())
            {
                GameDataHolder.FishAmount--;
                Destroy(gameObject);
            }
            CountUpTimer();
        }

        private bool HasDied()
        {
            return timer >= livingTime;
        }

        private void CountUpTimer()
        {
            timer += Time.deltaTime;
        }
    }
}