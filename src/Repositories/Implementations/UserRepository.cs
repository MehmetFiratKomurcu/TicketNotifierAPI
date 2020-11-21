using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Couchbase.Linq;
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

        public async Task<List<User>> GetAllUsers()
        {
            var context = await GetContextAsync();
            return context.Query<User>().Where(x => x.Type == "user").ToList();
        }

        public async Task UpsertUserAsync(User user)
        {
            var collection = await GetCollectionAsync();
            await collection.UpsertAsync(user.Email, user);
        }
        
        public async Task AppendEventByUserId(string userId, List<Event> eventObject)
        {
            var collection = await GetCollectionAsync();
            await collection.MutateInAsync(userId, builder =>
            {
                builder.ArrayAppend("events", eventObject, true);
            });
        }

        public async Task DeleteUser(string id)
        {
            var collection = await GetCollectionAsync();
            await RemoveAsync(id, collection);
        }
        
        private async Task<BucketContext> GetContextAsync()
        {
            var bucket = await GetBucketAsync();
            var context = new BucketContext(bucket);
            return context;
        }

        private static async Task RemoveAsync(string id, ICouchbaseCollection collection)
        {
            try
            {
                await collection.RemoveAsync(id);
            }
            catch (DocumentNotFoundException)
            {
            }
        }

        public async Task<bool> UserExistsById(string id)
        {
            var collection = await GetCollectionAsync();
            var exists = await collection.ExistsAsync(id);
            return exists.Exists;
        }

        private async Task<IGetResult> GetResultAsync(ICouchbaseCollection collection, string id)
        {
            try
            {
                var getResult = await collection.GetAsync(id);
                return getResult;
            }
            catch (DocumentNotFoundException)
            {
                return null;
            }
        }

        private async Task<ICouchbaseCollection> GetCollectionAsync()
        {
            var bucket = await GetBucketAsync();
            var collection = bucket.DefaultCollection();
            return collection;
        }

        private async Task<IBucket> GetBucketAsync()
        {
            return await _bucketProvider.GetBucketAsync();
        }
    }
}