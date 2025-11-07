using System.Linq;
using Domain;
using Extensions;
using Xunit;

namespace Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void AvailableAndNewest_Works()
        {
            var a = new Book(1, "One", "A", "I1");
            var b = new Book(2, "Two", "B", "I2") { IsAvailable = false };
            var c = new Book(3, "Three", "C", "I3");

            var items = new[] { a, b, c }.AsEnumerable();
            var available = items.Available().ToList();
            Assert.Contains(a, available);
            Assert.Contains(c, available);
            Assert.DoesNotContain(b, available);

            var newest = items.Newest(2).ToList();
            Assert.Equal(2, newest.Count);
            Assert.Equal(c, newest[0]); // id 3
            Assert.Equal(b, newest[1]); // id 2
        }
    }
}