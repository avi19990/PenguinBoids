using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class BoidSpatialHash
    {
        private List<Boid>[] cells;

        private static int record = 0;

        private float cellSize;
        private int cellCount;

        public void InitializeCells(float cellSize, float mapSize)
        {
            this.cellSize = cellSize;

            cellCount = Mathf.CeilToInt(mapSize / cellSize);
            if (cellCount == (int)(mapSize / cellSize))
                cellCount += 1;

            cells = new List<Boid>[cellCount * cellCount];
            for (int i = 0; i < cells.Length; ++i)
                cells[i] = new List<Boid>();
        }

        public void ClearCells()
        {
            for (int i = 0; i < cells.Length; ++i)
                cells[i].Clear();
        }

        public void AssignBoids(List<Boid> boids)
        {
            foreach (Boid boid in boids)
            {
                Vector2Int cellPos = new Vector2Int((int)(boid.transform.position.x / cellSize), (int)(boid.transform.position.z / cellSize));

                cells[cellPos.x + cellPos.y * cellCount].Add(boid);
            }
        }

        public List<NeighbourData> GetNeighbours(Boid boid, float radius)
        {
            List<NeighbourData> neighbours = new List<NeighbourData>();

            Vector2Int cellPos = new Vector2Int((int)(boid.transform.position.x / cellSize), (int)(boid.transform.position.z / cellSize));
            int cellRadius = Mathf.CeilToInt(radius / cellSize);

            float radiusSqr = radius * radius;

            int neighbourCount = 0;
            for (int y = cellPos.y - cellRadius; y <= cellPos.y + cellRadius; ++y)
                for (int x = cellPos.x - cellRadius; x <= cellPos.x + cellRadius; ++x)
                    if (IsCellInBounds(new Vector2Int(x, y)))
                    {
                        List<Boid> cell = cells[x + y * cellCount];

                        foreach (Boid neighbourBoid in cell)
                        {
                            float sqrtDistance = (neighbourBoid.transform.position - boid.transform.position).sqrMagnitude;

                            if (sqrtDistance <= radiusSqr)
                            {
                                neighbours.Add(new NeighbourData(neighbourBoid, sqrtDistance));
                                neighbourCount += 1;
                            }

                            if (neighbourCount > 50)
                                return neighbours;
                        }
                    }

            return neighbours;
        }

        private bool IsCellInBounds(Vector2Int position)
        {
            return position.x >= 0 && position.x < cellCount && position.y >= 0 && position.y < cellCount;
        }
    }
}