using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    [CreateAssetMenu(menuName = "Boid/Configuration")]
    public class BoidConfiguration : ScriptableObject
    {
        public float maxFOV;
        public float maxAcceleration;
        public float maxVelocity;

        public float cohesionRadius;
        public float cohesionPriority;

        public float alignmentRadius;
        public float alignmentPriority;

        public float separationRadius;
        public float separationPriority;

        public float avoidanceRadius;
        public float avoidancePriority;
    }
}