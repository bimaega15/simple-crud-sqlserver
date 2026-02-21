using MiniTrello.Models;

namespace MiniTrello.Persistences.Contracts
{
    public interface IBoardPersistence : IDataPersistence<Board>
    {
        Task<bool> DeleteBoardAsync(int id);
        Task<Board> GetBoardAsync(int id);
    }
}
