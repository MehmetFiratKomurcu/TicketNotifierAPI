using System.Threading.Tasks;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using TicketNotifier.Data;
using TicketNotifier.Entities;
using TicketNotifier.Repositories.Interfaces;

namespace TicketNotifier.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ITicketBucketProvider _bucketProvider;

        public UserRepository(ITicketBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<T> GetByIdAsync<T>(string id) where T : class
        {
            var collection = await GetCollectionAsync();
            var getResult = await GetResultAsync(collection, id);
            return getResult?.ContentAs<T>();
        }

        public async Task UpsertUserAsync(User user)
        {
            var collection = await GetCollectionAsync();
            await collection.UpsertAsync(user.Email, user);
        }

        private async Task<IGetResult> GetResultAsync(ICouchbaseCollection collection, string id)
        {
            try
            {
                var getResult = await collection.GetAsync(id);
                return getResult;
            }
            catch (DocumentNotFoundException e)
            {
                return null;
            }
        }

        private async Task<ICouchbaseCollection> GetCollectionAsync()
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();
            return collection;
        }
    }
}