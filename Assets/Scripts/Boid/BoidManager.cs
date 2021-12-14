using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Boids
{
    public class BoidManager : MonoBehaviour
    {
        public Boid boidPrefab;
        public Predator predatorPrefab;
        public int numberOfBoids;
        public int numberOfPredators;
        public List<Boid> boids;
        public List<Predator> predators;
        public float bounds;
        public float spawnRadius;

        void Start()
        {
            boids = new List<Boid>();
            predators = new List<Predator>();

            for (int i = 0; i < numberOfBoids; ++i)
                boids.Add(Instantiate(boidPrefab, new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)), Quaternion.identity, transform));

            for (int i = 0; i < numberOfPredators; ++i)
                predators.Add(Instantiate(predatorPrefab, new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)), Quaternion.identity, transform));
        }

        public List<Boid> GetNeighbours(Boid boid, float radius)
        {
            List<Boid> neighbours = new List<Boid>();

            //boid.neighboursFinder.Neighbours
            foreach (Boid neighbour in boid.neighboursFinder.Neighbours)
            {
                if (Vector3.Distance(boid.position, neighbour.position) <= radius)
                    neighbours.Add(neighbour);
            }

            return neighbours;
        }

        public List<Predator> GetPredators(Boid boid, float radius)
        {
            List<Predator> closePredators = new List<Predator>();

            foreach (Predator predator in predators)
            {
                if (Vector3.Distance(boid.position, predator.position) <= radius)
                    closePredators.Add(predator);
            }

            return closePredators;
        }
    }
}