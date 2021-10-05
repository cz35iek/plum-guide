using System.Threading;

namespace PlutoRoverApi
{
    public interface ICommandService
    {
        (int, int, char) Run(int x, int y, char head, char command);
    }

    public class CommandService : ICommandService
    {
        private Dictionary<char, (int, int)> _movements =>
            new Dictionary<char, (int, int)> {
                {'N', (0,1) },
                {'S', (0,-1) },
                {'E', (1,0) },
                {'W', (-1,0) }
            };

        private Dictionary<char, (char, char)> _rotations =>
            new Dictionary<char, (char, char)> {
                {'N', ('E', 'W') },
                {'S', ('W', 'E') },
                {'E', ('S', 'N') },
                {'W', ('N', 'S') }
            };

        private readonly IObstaclesDetector _obstaclesDetector;

        public CommandService(IObstaclesDetector obstaclesDetector)
        {
            _obstaclesDetector = obstaclesDetector ?? throw new ArgumentNullException(nameof(obstaclesDetector));
        }

        public (int, int, char) Run(int x, int y, char head, char command)
        {
            var deltaX = _movements[head].Item1;
            var deltaY = _movements[head].Item2;

            switch (command)
            {
                case 'F':
                    x += deltaX;
                    y += deltaY;
                    break;

                case 'B':
                    x -= deltaX;
                    y -= deltaY;
                    break;

                case 'R':
                    head = _rotations[head].Item1;
                    break;

                case 'L':
                    head = _rotations[head].Item2;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }

            if (_obstaclesDetector.IsObstacle(x, y))
            {
                throw new Exception("Obstacle detected!");
            }

            return (x, y, head);
        }
    }
}