namespace API.Repository
{
    public interface IRedisRepository
    {
        Task<string> AcquireLockAsync(int productId, int quantity, int cartId);
        //Task ReleaseLockAsync(string key);
    }
}
