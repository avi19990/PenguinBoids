using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    [CreateAssetMenu(menuName = "Boid/Configuration")]
    public class BoidConfiguration : ScriptableObject
    {
        [Header("Boid config")]
        [Range(0.0f, 360.0f)]
        public float maxFOV;
        public float maxAcceleration;
        public float maxVelocity;

        [Header("Behavior config")]
        public float cohesionRadius;
        public float cohesionPriority;

        public float alignmentRadius;
        public float alignmentPriority;

        public float separationRadius;
        public float separationPriority;

        public float avoidanceRadius;
        public float avoidancePriority;

        public float centerRadius;
        public float centerPriority;

        public float wanderPriority;
    }
}