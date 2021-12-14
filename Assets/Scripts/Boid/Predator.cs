using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class Predator : Boid
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private float directionChangeCooldown;

        private float timeUntilDirectionChange;

        void Start()
        {
            position = transform.position;
            velocity = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * speed;

            timeUntilDirectionChange = Time.time;

            manager = FindObjectOfType<BoidManager>();
        }

        // Update is called once per frame
        void Update()
        {
            position += velocity * Time.deltaTime;
            WrapAround(ref position, -manager.bounds, manager.bounds);

            if (timeUntilDirectionChange < Time.time)
            {
                velocity = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * speed;

                timeUntilDirectionChange = Time.time + directionChangeCooldown;
            }

            transform.position = position;
        }
    }
}