using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Boids
{
    public class BoidManager : MonoBehaviour
    {
        [SerializeField]
        private BoidConfiguration config;

        [SerializeField]
        private Transform boidPrefab;
        [SerializeField]
        private Transform predatorPrefab;

        [SerializeField]
        private float mapRadius;
        [SerializeField]
        private int numberOfBoids;
        [SerializeField]
        private int numberOfPredators;

        private List<Boid> boids;
        private List<Boid> predators;

        private BoidSpatialHash spatialHash;

        private float maxBehaviorRadius;

        void Start()
        {
            boids = new List<Boid>();
            predators = new List<Boid>();

            for (int i = 0; i < numberOfBoids; ++i)
            {
                Transform boidTransform = Instantiate(boidPrefab, RandomPointOnMap(), Quaternion.identity, transform);

                boids.Add(new Boid(boidTransform, new Vector3()));
            }

            for (int i = 0; i < numberOfPredators; ++i)
            {
                Transform predatorTransform = Instantiate(predatorPrefab, RandomPointOnMap(), Quaternion.identity, transform);

                predators.Add(new Boid(predatorTransform, new Vector3()));
            }

            spatialHash = new BoidSpatialHash();
            spatialHash.InitializeCells(10, mapRadius * 2);

            maxBehaviorRadius = Mathf.Max(config.alignmentRadius, config.avoidanceRadius, config.cohesionRadius, config.separationRadius);
        }

        public List<Boid> GetPredators(Boid boid, float radius)
        {
            List<Boid> closePredators = new List<Boid>();

            foreach (Boid predator in predators)
            {
                if (Vector3.Distance(boid.transform.position, predator.transform.position) <= radius)
                    closePredators.Add(predator);
            }

            return closePredators;
        }

        private void Update()
        {
            for (int i = 0; i < predators.Count; ++i)
            {
                Boid predator = predators[i];

                Vector3 acceleration = Wander(predator) * config.wanderPriority + Center(predator) * config.centerPriority;
                acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);

                predator.velocity += acceleration * Time.deltaTime;
                predator.velocity = Vector3.ClampMagnitude(predator.velocity, config.maxVelocity);

                predator.transform.position += predator.velocity * Time.deltaTime;
                predator.transform.rotation = Quaternion.LookRotation(predator.velocity);
                predator.transform.rotation = Quaternion.Euler(new Vector3(0.0f, predator.transform.rotation.eulerAngles.y, 0.0f));
            }

            spatialHash.AssignBoids(boids);

            for (int i = 0; i < boids.Count; ++i)
            {
                Boid boid = boids[i];

                List<NeighbourData> neighbours = spatialHash.GetNeighbours(boid, maxBehaviorRadius);
                neighbours.Sort((NeighbourData first, NeighbourData second) => { return first.sqrDistance.CompareTo(second.sqrDistance); });

                Vector3 acceleration = Combined(boid, neighbours);
                acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);

                boid.velocity += acceleration * Time.deltaTime;
                boid.velocity = Vector3.ClampMagnitude(boid.velocity, config.maxVelocity);

                boid.transform.position += boid.velocity * Time.deltaTime;
                boid.transform.rotation = Quaternion.LookRotation(boid.velocity);
                boid.transform.rotation = Quaternion.Euler(new Vector3(0.0f, boid.transform.rotation.eulerAngles.y, 0.0f));
            }

            spatialHash.ClearCells();
        }

        private Vector3 Combined(Boid boid, List<NeighbourData> neighbours)
        {
            return Cohesion(boid, neighbours) * config.cohesionPriority + 
                   Alignment(boid, neighbours) * config.alignmentPriority + 
                   Separation(boid, neighbours) * config.separationPriority + 
                   Avoidance(boid) * config.avoidancePriority + 
                   Center(boid) * config.centerPriority;
        }

        private Vector3 Cohesion(Boid boid, List<NeighbourData> neighbours)
        {
            Vector3 cohesionVector = new Vector3();

            if (neighbours.Count == 0)
                return cohesionVector;

            int nearNeighbourCount = neighbours.FindIndex((NeighbourData element) => { return element.sqrDistance > (config.cohesionRadius * config.cohesionRadius); });
            if (nearNeighbourCount == -1)
                nearNeighbourCount = neighbours.Count;
            
            int boidCount = 0;
            for (int i = 0; i < nearNeighbourCount; ++i)
            {
                if (!isInFOV(boid, neighbours[i].boid.transform.position))
                    continue;

                cohesionVector += neighbours[i].boid.transform.position;
                boidCount += 1;
            }

            if (boidCount == 0)
                return cohesionVector;

            cohesionVector /= boidCount;
            cohesionVector -= boid.transform.position;

            return cohesionVector.normalized;
        }

        private Vector3 Alignment(Boid boid, List<NeighbourData> neighbours)
        {
            Vector3 alignmentVector = new Vector3();

            if (neighbours.Count == 0)
                return alignmentVector;

            int nearNeighbourCount = neighbours.FindIndex((NeighbourData element) => { return element.sqrDistance > (config.alignmentRadius * config.alignmentRadius); });
            if (nearNeighbourCount == -1)
                nearNeighbourCount = neighbours.Count;

            for (int i = 0; i < nearNeighbourCount; ++i)
            {
                if (!isInFOV(boid, neighbours[i].boid.transform.position))
                    continue;

                alignmentVector += neighbours[i].boid.velocity;
            }

            return alignmentVector.normalized;
        }

        private Vector3 Separation(Boid boid, List<NeighbourData> neighbours)
        {
            Vector3 separateVector = new Vector3();

            if (neighbours.Count == 0)
                return separateVector;

            int nearNeighbourCount = neighbours.FindIndex((NeighbourData element) => { return element.sqrDistance > (config.separationRadius * config.separationRadius); });
            if (nearNeighbourCount == -1)
                nearNeighbourCount = neighbours.Count;

            for (int i = 0; i < nearNeighbourCount; ++i)
            {
                if (!isInFOV(boid, neighbours[i].boid.transform.position))
                    continue;

                Vector3 movingTowards = boid.transform.position - neighbours[i].boid.transform.position;
                if (movingTowards.magnitude > 0)
                    separateVector += movingTowards.normalized / movingTowards.magnitude;
            }

            return separateVector.normalized;
        }

        private Vector3 Avoidance(Boid boid)
        {
            Vector3 avoidVector = new Vector3();
            List<Boid> predators = GetPredators(boid, config.avoidanceRadius);

            if (predators.Count == 0)
                return avoidVector;

            foreach (Boid predator in predators)
            {
                avoidVector += RunAway(boid, predator.transform.position);
            }

            return avoidVector.normalized;
        }

        private Vector3 Center(Boid boid)
        {
            Vector3 centerVector = new Vector3();

            float distanceFromEdge = mapRadius - Vector3.Distance(boid.transform.position, new Vector3(mapRadius, 0, mapRadius));
            if (distanceFromEdge < config.centerRadius)
                centerVector = new Vector3(mapRadius, 0, mapRadius) - boid.transform.position;

            return centerVector.normalized;
        }

        private Vector3 Wander(Boid boid)
        {
            if (boid.seekTimeout < Time.time || (boid.seekTarget - boid.transform.position).sqrMagnitude < 100)
            {
                boid.seekTimeout = Time.time + 5.0f;
                boid.seekTarget = RandomPointOnMap();
            }

            return Seek(boid);
        }

        private Vector3 Seek(Boid boid)
        {
            Vector3 direction = boid.seekTarget - boid.transform.position;

            return direction.normalized;
        }

        private Vector3 RunAway(Boid boid, Vector3 target)
        {
            Vector3 neededVelocity = (boid.transform.position - target).normalized * config.maxVelocity;
            return neededVelocity - boid.velocity;
        }

        private bool isInFOV(Boid boid, Vector3 otherPosition)
        {
            return Vector3.Angle(boid.velocity, otherPosition - boid.transform.position) <= config.maxFOV;
        }

        private Vector3 RandomPointOnMap()
        {
            Vector3 randPosition = Random.insideUnitSphere;
            randPosition.y = 0;
            randPosition = randPosition.normalized * Random.Range(0, mapRadius) + new Vector3(mapRadius, 0, mapRadius);
            return randPosition;
        }
    }
}