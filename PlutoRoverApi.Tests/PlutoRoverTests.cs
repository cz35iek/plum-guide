using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace PlutoRoverApi.Tests
{
    public class PlutoRoverTests
    {
        [Fact]
        public async Task Should_Run_To_Proper_Position()
        {
            await using var application = new PlutoRoverApplication();

            var client = application.CreateClient();
            var position = await client.GetFromJsonAsync<Position>("/run/RFFFFLFF");

            Assert.Equal(Heading.North, position.Heading);
            Assert.Equal(4, position.X);
            Assert.Equal(2, position.Y);
        }

        [Fact]
        public async Task Should_Fail_If_Unknown_Command()
        {
            await using var application = new PlutoRoverApplication();

            var client = application.CreateClient();
            var result = await client.GetAsync("/run/FFRWFF");

            Assert.False(result.IsSuccessStatusCode);
        }
    
        [Fact]
        public async Task Should_Fail_If_Hit_Obstacle()
        {
            await using var application = new PlutoRoverApplication();

            var client = application.CreateClient();
            var result = await client.GetAsync("/run/FFRFF");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("RBB", Heading.East, 9, 0)]
        [InlineData("LFF", Heading.West, 9, 0)]
        [InlineData("BB", Heading.North, 0, 9)]
        [InlineData("FFFFFFFFFFF", Heading.North, 0, 0)]
        public async Task Should_Wrap_Around_Edges(string commands, Heading heading, int x, int y)
        {
            await using var application = new PlutoRoverApplication();

            var client = application.CreateClient();
            var position = await client.GetFromJsonAsync<Position>($"/run/{commands}");

            Assert.Equal(heading, position.Heading);
            Assert.Equal(x, position.X);
            Assert.Equal(y, position.Y);
        }
    }

    class PlutoRoverApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<PositionDbContext>));

                services.AddDbContext<PositionDbContext>(options =>
                    options.UseInMemoryDatabase("Testing", root));
            });

            return base.CreateHost(builder);
        }
    }
}
