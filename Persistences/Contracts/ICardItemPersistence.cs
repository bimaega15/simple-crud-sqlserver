using MiniTrello.Models;

namespace MiniTrello.Persistences.Contracts
{
    public interface ICardItemPersistence : IDataPersistence<CardItem>
    {
        Task<bool> DeleteCardAsync(int id);
        Task<CardItem> GetCardAsync(int id);
        Task<IEnumerable<CardItem>> GetCardsByBoardAsync(int boardId);
        Task<IEnumerable<CardItem>> GetCardsByBoardAndStatusAsync(int boardId, int status);
        Task<bool> UpdateCardStatusAsync(int cardId, int newStatus);
    }
}
