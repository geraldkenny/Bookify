namespace Bookify.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IBookRepository Book { get; }
        IUserRepository User { get; }
        void SaveChanges();
    }
}
