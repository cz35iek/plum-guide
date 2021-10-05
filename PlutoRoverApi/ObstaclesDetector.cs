using System.Threading;

namespace PlutoRoverApi
{
    public interface IObstaclesDetector 
    {
        bool IsObstacle(int x, int y);
    }

    public class ObstaclesDetector : IObstaclesDetector
    {
        public bool IsObstacle(int x, int y)
        {
            // todo
            if (x == 1 && y == 2) return true;
            if (x == 4 && y == 6) return true;

            return false;
        }
    }
}