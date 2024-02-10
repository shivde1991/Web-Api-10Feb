namespace Migrations
{
    public interface IDapperContextDb
    {
         Task<IEnumerable<ReturnResponse>> SearchBooks(string key);
    }
}
