namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IJsonRepository<TReturn>
    {
        TReturn Get(string fileName);
        IEnumerable<TReturn> GetAll(string folderPath);
    }
}
