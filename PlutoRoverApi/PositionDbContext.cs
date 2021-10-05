using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PlutoRoverApi
{
    public class PositionDbContext : DbContext
    {
        public PositionDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Position> Positions => Set<Position>();
    }

    public class Position
    {
        [Key]
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Heading Heading { get; set; }
        public DateTime Created { get; set; }
    }

    public enum Heading
    {
        North,
        East,
        South,
        West
    }
}