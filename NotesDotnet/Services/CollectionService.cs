using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotesDotnet.Models;

namespace NotesDotnet.Services
{
    public class CollectionService<T> : ICollectionService<T>
        where T : MongoEntity
    {
        public readonly IMongoCollection<T> _collection;

        public CollectionService(IOptions<NotesDatabaseSettings> notesDbSettings)
        {
            var mongoClient = new MongoClient(notesDbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(notesDbSettings.Value.DatabaseName);

            string collectionName = typeof(T).Name;

            _collection = mongoDatabase.GetCollection<T>(collectionName);
        }

        public async Task<List<T>> GetAsync() => await _collection.Find(_ => true).ToListAsync();

        public async Task<T?> GetAsync(string id) =>
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(T newBook) => await _collection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, T updatedBook) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
