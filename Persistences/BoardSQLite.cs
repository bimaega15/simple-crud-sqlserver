using MiniTrello.Helpers;
using MiniTrello.Models;
using MiniTrello.Persistences.Contracts;
using SQLite;
using System.Diagnostics;

namespace MiniTrello.Persistences
{
    public class BoardSQLite : IBoardPersistence
    {
        SQLiteAsyncConnection Database;

        public BoardSQLite() { }

        async Task Init(bool forceReconnect = false)
        {
            if (Database is not null && !forceReconnect)
                return;

            if (forceReconnect && Database is not null)
            {
                try { await Database.CloseAsync(); } catch { }
                Database = null;
            }

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<Board>();
            if (result == CreateTableResult.Created)
                Debug.WriteLine("----------> Table Board Created");
        }

        async Task<T> ExecuteWithRetry<T>(Func<Task<T>> operation)
        {
            try { return await operation(); }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Database connection closed, reconnecting...");
                await Init(forceReconnect: true);
                return await operation();
            }
        }

        public async Task<bool> AddItemAsync(Board item)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                return await Database.InsertAsync(item) == 1;
            });
        }

        public async Task<bool> UpdateItemAsync(Board item)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
            {
                item.ModifiedDate = DateTime.Now;
                return await Database.UpdateAsync(item) == 1;
            });
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            return await DeleteBoardAsync(int.Parse(id));
        }

        public async Task<bool> DeleteBoardAsync(int id)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
            {
                var item = await GetBoardAsync(id);
                if (item == null) return false;
                return await Database.DeleteAsync(item) == 1;
            });
        }

        public async Task<Board> GetItemAsync(string id)
        {
            return await GetBoardAsync(int.Parse(id));
        }

        public async Task<Board> GetBoardAsync(int id)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
                await Database.Table<Board>().Where(b => b.Id == id).FirstOrDefaultAsync());
        }

        public async Task<IEnumerable<Board>> GetItemsAsync(bool forceRefresh = false)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
                await Database.Table<Board>().OrderByDescending(b => b.ModifiedDate).ToListAsync());
        }

        public IEnumerable<Board> GetItems(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
    }
}
