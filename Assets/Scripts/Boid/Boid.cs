using UnityEngine;

namespace Boids
{
    public class Boid
    {
        public Transform transform;
        public Vector3 velocity;

        public Vector3 seekTarget;
        public float seekTimeout;

        public Boid(Transform transform, Vector3 velocity, Vector3 seekTarget = new Vector3(), float seekTimeout = 0.0f)
        {
            this.transform = transform;
            this.velocity = velocity;

            this.seekTarget = seekTarget;
            this.seekTimeout = seekTimeout;
        }
    }
}