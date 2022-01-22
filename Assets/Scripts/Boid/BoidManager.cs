using System.Collections.Generic;
using System.Linq;
using Fish;
using UnityEngine;

namespace Boid
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
        private Transform obstaclePrefab;

        [SerializeField]
        private float mapRadius;
        [SerializeField]
        private int numberOfBoids;
        [SerializeField]
        private int numberOfPredators;

        private List<Boid> boids;
        private List<Boid> predators;
        private List<GameObject> obstacles;

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

            obstacles = FindObjectsOfType<GameObject>().Where(foundGameObject => foundGameObject.CompareTag(obstaclePrefab.tag)).ToList();

            spatialHash = new BoidSpatialHash();
            spatialHash.InitializeCells(10, mapRadius * 2);

            maxBehaviorRadius = Mathf.Max(config.alignmentRadius, config.avoidanceRadius, config.cohesionRadius, config.separationRadius);
        }

        private void Update()
        {
            for (int i = 0; i < predators.Count; ++i)
            {
                Boid predator = predators[i];

                Vector3 acceleration = CombinedPredator(predator);
                acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);

                predator.velocity += acceleration * Time.deltaTime;
                predator.velocity = Vector3.ClampMagnitude(predator.velocity, config.maxVelocity);

                predator.transform.position += predator.velocity * Time.deltaTime;
            }

            spatialHash.AssignBoids(boids);

            for (int i = 0; i < boids.Count; ++i)
            {
                Boid boid = boids[i];

                List<NeighbourData> neighbours = spatialHash.GetNeighbours(boid, maxBehaviorRadius);
                neighbours.Sort((NeighbourData first, NeighbourData second) => first.sqrDistance.CompareTo(second.sqrDistance));

                Vector3 acceleration = Combined(boid, neighbours);
                acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);
                acceleration.y = 0;

                boid.velocity += acceleration * Time.deltaTime;
                boid.velocity = Vector3.ClampMagnitude(boid.velocity, config.maxVelocity);

                boid.transform.position += boid.velocity * Time.deltaTime;
            }

            spatialHash.ClearCells();
        }
        
        private Vector3 CombinedPredator(Boid boid)
        {
            return Wander(boid) * config.wanderPriority 
                   + Center(boid) * config.centerPriority 
                   + AvoidanceObstacle(boid) * config.avoidancePriority;
        }

        private Vector3 Combined(Boid boid, List<NeighbourData> neighbours)
        {
            return Cohesion(boid, neighbours) * config.cohesionPriority + 
                   Alignment(boid, neighbours) * config.alignmentPriority + 
                   Separation(boid, neighbours) * config.separationPriority + 
                   Avoidance(boid) * config.avoidancePriority + 
                   AvoidanceObstacle(boid) * config.avoidancePriority + 
                   Center(boid) * config.centerPriority + 
                   FoodSearching(boid) * (FoodSearching(boid) == Vector3.zero ? 0: config.bonusPriority);
        }

        private Vector3 FoodSearching(Boid boid)
        {
            if (GameDataHolder.FishAmount == 0)
            {
                return Vector3.zero;
            }

            List<Fish.Fish> fishes = FindObjectsOfType<Fish.Fish>().ToList().Where(fish =>
                IsBoidCloseEnough(boid, config.bonusRadius, fish.transform.position)).OrderBy(fish => Vector3.Distance(boid.transform.position, fish.transform.position)).ToList();
            if (fishes == null || fishes.Count == 0)
            {
                return Vector3.zero;
            }

            return fishes[0].transform.position;
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
        
        private Vector3 AvoidanceObstacle(Boid boid)
        {
            Vector3 avoidVector = new Vector3();
            List<GameObject> obstacles = GetClosesObstacles(boid, config.avoidanceRadius);
            
            obstacles.ForEach(obstacle => avoidVector += RunAway(boid, obstacle.transform.position));

            return avoidVector.normalized;
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

        private List<GameObject> GetClosesObstacles(Boid boid, float avoidanceRadius)
        {
            return obstacles.Where(obstacle =>
                IsBoidCloseEnough(boid, avoidanceRadius, obstacle.transform.position)).ToList();
        }

        private bool IsBoidCloseEnough(Boid boid, float avoidanceRadius, Vector3 anotherPosition)
        {
            anotherPosition.y = 0;
            boid.transform.position = new Vector3(boid.transform.position.x, 0, boid.transform.position.z);
            return Vector3.Distance(boid.transform.position, anotherPosition) <= avoidanceRadius;
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