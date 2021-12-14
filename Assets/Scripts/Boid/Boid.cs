using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class Boid : MonoBehaviour
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;

        public BoidManager manager;
        public BoidConfiguration config;
        public BoidNeighbours neighboursFinder;

        private void Start()
        {
            manager = FindObjectOfType<BoidManager>();

            position = transform.position;
            velocity = new Vector3(Random.Range(-3, 4), 0, Random.Range(-3, 4));
        }

        private void Update()
        {
            acceleration = Combined();
            acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);

            velocity += acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, config.maxVelocity);

            position += velocity * Time.deltaTime;
            WrapAround(ref position, -manager.bounds, manager.bounds);

            transform.position = position;
        }

        private Vector3 Combined()
        {
            return Cohesion() * config.cohesionPriority + Alignment() * config.alignmentPriority + Separation() * config.separationPriority + Avoidance() * config.avoidancePriority;
        }

        private Vector3 Cohesion()
        {
            Vector3 cohesionVector = new Vector3();
            List<Boid> neighbours = manager.GetNeighbours(this, config.cohesionRadius);

            if (neighbours.Count == 0)
                return cohesionVector;

            int boidCount = 0;
            foreach (Boid neighbour in neighbours)
            {
                if (!isInFOV(neighbour.position))
                    continue;

                cohesionVector += neighbour.position;
                boidCount += 1;
            }

            if (boidCount == 0)
                return cohesionVector;

            cohesionVector /= boidCount;
            cohesionVector -= position;

            return cohesionVector.normalized;
        }

        private Vector3 Alignment()
        {
            Vector3 alignmentVector = new Vector3();
            List<Boid> neighbours = manager.GetNeighbours(this, config.alignmentRadius);

            if (neighbours.Count == 0)
                return alignmentVector;

            foreach (Boid neighbour in neighbours)
            {
                if (!isInFOV(neighbour.position))
                    continue;

                alignmentVector += neighbour.velocity;
            }

            return alignmentVector.normalized;
        }

        private Vector3 Separation()
        {
            Vector3 separateVector = new Vector3();
            List<Boid> neighbours = manager.GetNeighbours(this, config.separationRadius);

            if (neighbours.Count == 0)
                return separateVector;

            foreach (Boid neighbour in neighbours)
            {
                if (!isInFOV(neighbour.position))
                    continue;

                Vector3 movingTowards = position - neighbour.position;
                if (movingTowards.magnitude > 0)
                    separateVector += movingTowards.normalized / movingTowards.magnitude;
            }

            return separateVector.normalized;
        }

        private Vector3 Avoidance()
        {
            Vector3 avoidVector = new Vector3();
            List<Predator> predators = manager.GetPredators(this, config.avoidanceRadius);

            if (predators.Count == 0)
                return avoidVector;

            foreach (Predator predator in predators)
            {
                avoidVector += RunAway(predator.position);
            }

            return avoidVector.normalized;
        }

        private Vector3 RunAway(Vector3 target)
        {
            Vector3 neededVelocity = (position - target).normalized * config.maxVelocity;
            return neededVelocity - velocity;
        }

        private bool isInFOV(Vector3 otherPosition)
        {
            return Vector3.Angle(velocity, otherPosition - position) <= config.maxFOV;
        }

        public void WrapAround(ref Vector3 position, float min, float max)
        {
            position.x = WrapAroundFloat(position.x, min, max);
            position.y = WrapAroundFloat(position.y, min, max);
            position.z = WrapAroundFloat(position.z, min, max);
        }

        public float WrapAroundFloat(float value, float min, float max)
        {
            if (value > max)
                value = min;
            else if (value < min)
                value = max;

            return value;
        }
    }
}