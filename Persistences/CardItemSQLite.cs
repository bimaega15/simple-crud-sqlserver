using MiniTrello.Helpers;
using MiniTrello.Models;
using MiniTrello.Persistences.Contracts;
using SQLite;
using System.Diagnostics;

namespace MiniTrello.Persistences
{
    public class CardItemSQLite : ICardItemPersistence
    {
        SQLiteAsyncConnection Database;

        public CardItemSQLite() { }

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
            var result = await Database.CreateTableAsync<CardItem>();
            if (result == CreateTableResult.Created)
                Debug.WriteLine("----------> Table CardItem Created");
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

        public async Task<bool> AddItemAsync(CardItem item)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
            {
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                return await Database.InsertAsync(item) == 1;
            });
        }

        public async Task<bool> UpdateItemAsync(CardItem item)
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
            return await DeleteCardAsync(int.Parse(id));
        }

        public async Task<bool> DeleteCardAsync(int id)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
            {
                var item = await GetCardAsync(id);
                if (item == null) return false;
                return await Database.DeleteAsync(item) == 1;
            });
        }

        public async Task<CardItem> GetItemAsync(string id)
        {
            return await GetCardAsync(int.Parse(id));
        }

        public async Task<CardItem> GetCardAsync(int id)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
                await Database.Table<CardItem>().Where(c => c.Id == id).FirstOrDefaultAsync());
        }

        public async Task<IEnumerable<CardItem>> GetItemsAsync(bool forceRefresh = false)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
                await Database.Table<CardItem>().ToListAsync());
        }

        public async Task<IEnumerable<CardItem>> GetCardsByBoardAsync(int boardId)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
                await Database.Table<CardItem>()
                    .Where(c => c.BoardId == boardId)
                    .OrderBy(c => c.Order)
                    .ToListAsync());
        }

        public async Task<IEnumerable<CardItem>> GetCardsByBoardAndStatusAsync(int boardId, int status)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
                await Database.Table<CardItem>()
                    .Where(c => c.BoardId == boardId && c.Status == status)
                    .OrderBy(c => c.Order)
                    .ToListAsync());
        }

        public async Task<bool> UpdateCardStatusAsync(int cardId, int newStatus)
        {
            await Init();
            return await ExecuteWithRetry(async () =>
            {
                var card = await GetCardAsync(cardId);
                if (card == null) return false;
                card.Status = newStatus;
                card.ModifiedDate = DateTime.Now;
                return await Database.UpdateAsync(card) == 1;
            });
        }

        public IEnumerable<CardItem> GetItems(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
    }
}
