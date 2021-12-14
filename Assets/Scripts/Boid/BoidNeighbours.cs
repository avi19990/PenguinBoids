using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class BoidNeighbours : MonoBehaviour
    {
        [SerializeField]
        private List<Boid> neighbours = new List<Boid>();
        [SerializeField]
        private BoidConfiguration config;

        private SphereCollider sphereCollider;

        public List<Boid> Neighbours => neighbours;

        private void Awake()
        {
            GetReferences();
        }

        private void Start()
        {
            sphereCollider.radius = GetLongestBehaviorRadius() / 2;
        }

        private void GetReferences()
        {
            sphereCollider = GetComponent<SphereCollider>();
        }

        private float GetLongestBehaviorRadius()
        {
            return Mathf.Max(config.cohesionRadius, config.alignmentRadius, config.separationRadius, config.avoidanceRadius);
        }

        private void OnTriggerEnter(Collider other)
        {
            neighbours.Add(other.transform.GetComponent<Boid>());
        }

        private void OnTriggerExit(Collider other)
        {
            neighbours.Remove(other.transform.GetComponent<Boid>());
        }
    }
}