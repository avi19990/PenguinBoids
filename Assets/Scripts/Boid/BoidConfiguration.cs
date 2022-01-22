using UnityEngine;

namespace Boid
{
    [CreateAssetMenu(menuName = "Boid/Configuration")]
    public class BoidConfiguration : ScriptableObject
    {
        [Header("Boid config")]
        [Range(0.0f, 360.0f)]
        [SerializeField]
        public float maxFOV;
        [SerializeField]
        public float maxAcceleration;
        [SerializeField]
        public float maxVelocity;

        [Header("Behavior config")]
        [SerializeField]
        public float cohesionRadius;
        [SerializeField]
        public float cohesionPriority;
        
        [SerializeField]
        public float alignmentRadius;
        [SerializeField]
        public float alignmentPriority;
        
        [SerializeField]
        public float separationRadius;
        [SerializeField]
        public float separationPriority;
        
        [SerializeField]
        public float avoidanceRadius;
        [SerializeField]
        public float avoidancePriority;
        
        [SerializeField]
        public float centerRadius;
        [SerializeField]
        public float centerPriority;
        
        [SerializeField]
        public float wanderPriority;

        [SerializeField] public float bonusRadius;
        [SerializeField] public float bonusPriority;
        
        
    }
}