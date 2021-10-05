using System.Threading;

namespace PlutoRoverApi
{
    interface IBoudaryService 
    {
        (int, int) WrapPosition(int x, int y);
    }

    public class BoudaryService : IBoudaryService
    {
        // boundaries
        const int MinX = 0;
        const int MinY = 0;
        const int MaxX = 10;
        const int MaxY = 10;

        public (int, int) WrapPosition(int x, int y)
        {
            if (x < MinX) x = MaxX;
            if (x > MaxX) x = MinX;
            if (y < MinY) y = MaxY;
            if (y > MaxY) y = MinY;

            return (x,y);
        }
    }
}