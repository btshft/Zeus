using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zeus.Storage.Faster.Tests.Utils;

namespace Zeus.Storage.Faster.Tests
{
    public class StructUseCasesTests
    {
        [Theory, ClearStorage(".store")]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task InsertGet(int loops)
        {
            using var store = StoreFactory.Create<int, string>(".store", Comparers.IntComparer.Instance);

            for (var i = 0; i < loops; i++)
                await store.StoreAsync(i, $"value-{i + 1}");

            await store.CommitAsync();

            for (var i = 0; i < loops; i++)
            {
                var (exists, result) = await store.GetAsync(i);
                result.Should().Be($"value-{i + 1}");
            }

            var (found, value) = await store.GetAsync(loops + 1);

            value.Should().BeNull();
            found.Should().BeFalse();
        }

        [Theory, ClearStorage(".store")]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task InsertIterate(int loops)
        {
            using var store = StoreFactory.Create<int, string>(".store", Comparers.IntComparer.Instance);

            for (var i = 0; i < loops; i++)
                await store.StoreAsync(i, $"value-{i + 1}");

            await store.CommitAsync();

            await foreach (var (key, value) in store)
            {
                value.Should().Be($"value-{key + 1}");
            }
        }

        [Theory, ClearStorage(".store")]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task InsertUpdateIterate(int loops)
        {
            static bool Rewrite(int current) => current % 2 == 0;

            using var store = StoreFactory.Create<int, string>(".store", Comparers.IntComparer.Instance);

            for (var i = 0; i < loops; i++)
                await store.StoreAsync(i, $"value-{i + 1}");

            for (var i = 0; i < loops; i++)
            {
                if (Rewrite(i))
                    await store.StoreAsync(i, $"rewrited-{i + 1}");
            }

            await store.CommitAsync();
            await foreach (var (key, value) in store)
            {
                value.Should().Be(Rewrite(key) ? $"rewrited-{key + 1}" : $"value-{key + 1}");
            }
        }

        [Theory, ClearStorage(".store")]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task InsertRemoveIterate(int loops)
        {
            static bool Delete(int current) => current % 2 == 0;

            using var store = StoreFactory.Create<int, string>(".store", Comparers.IntComparer.Instance);

            for (var i = 0; i < loops; i++)
                await store.StoreAsync(i, $"value-{i + 1}");

            var deletedCount = 0;
            for (var i = 0; i < loops; i++)
            {
                if (!Delete(i)) 
                    continue;

                await store.RemoveAsync(i);
                deletedCount++;
            }

            await foreach (var (key, value) in store)
            {
                value.Should().Be($"value-{key + 1}");
            }

            var count = await store.CountAsync();
            count.Should().Be(loops - deletedCount);
        }

        [Theory, ClearStorage(".store")]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task CommitRestore(int loops)
        {
            using (var store = StoreFactory.Create<int, string>(".store", Comparers.IntComparer.Instance))
            {
                for (var i = 0; i < loops; i++)
                    await store.StoreAsync(i, $"value-{i + 1}");

                await store.CommitAsync();
            }

            using (var recoveredStore = StoreFactory.Create<int, string>(".store", Comparers.IntComparer.Instance))
            {
                await foreach (var (key, value) in recoveredStore)
                {
                    value.Should().Be($"value-{key + 1}");
                }

                var count = await recoveredStore.CountAsync();
                count.Should().Be(loops);
            }
        }
    }
}
