namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IJsonRepository<TReturn>
    {
        string GetJsonFile(string folderPath);
        IList<TReturn> GetAll(string folderPath);
    }
}
