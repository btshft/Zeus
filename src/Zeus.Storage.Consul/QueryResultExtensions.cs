using System;
using System.Net;
using System.Threading.Tasks;
using Consul;

namespace Zeus.Storage.Consul
{
    internal static partial class QueryResultExtensions
    {
        public static T Unwrap<T>(this QueryResult<T> queryResult)
        {
            if (queryResult == null)
                throw new ArgumentNullException(nameof(queryResult));

            return queryResult.StatusCode switch
            {
                HttpStatusCode.OK => queryResult.Response,
                HttpStatusCode.NotFound => default(T),
                _ => throw new ConsulRequestException(
                    $"Unable to load data from consul. Response status code '{(int)queryResult.StatusCode}'",
                    queryResult.StatusCode)
            };
        }

        public static async Task<T> UnwrapAsync<T>(this Task<QueryResult<T>> queryResult)
        {
            var result = await queryResult;
            return result.Unwrap();
        }
    }
}