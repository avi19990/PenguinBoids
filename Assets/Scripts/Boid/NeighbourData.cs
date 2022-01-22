namespace Boid
{
    public class NeighbourData
    {
        public Boid boid;
        public float sqrDistance;

        public NeighbourData(Boid _boid, float _sqrDistance)
        {
            boid = _boid;
            sqrDistance = _sqrDistance;
        }
    }
}