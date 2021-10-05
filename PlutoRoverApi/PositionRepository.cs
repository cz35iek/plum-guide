using System.Threading;

namespace PlutoRoverApi
{
    interface IPositionRepository 
    {
        Task<Position> AddPosition(int x, int y, char head);
    }

    public class PositionRepository : IPositionRepository
    {
        private readonly PositionDbContext _db;
        private Dictionary<char, Heading> _headings => 
            new Dictionary<char, Heading>
            {
                { 'N', Heading.North },
                { 'S', Heading.South },
                { 'E', Heading.East },
                { 'W', Heading.West },
            };

        public PositionRepository(PositionDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Position> AddPosition(int x, int y, char head)
        {
            var position = new Position { X = x, Y = y, Heading = _headings[head], Created = DateTime.UtcNow };

            await _db.Positions.AddAsync(position);
            await _db.SaveChangesAsync();

            return position;
        }
    }
}