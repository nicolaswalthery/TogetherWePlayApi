using Common.ResultPattern;

namespace TWP.Api.Application.ETL
{
    public interface IExtractTransformLoad
    {
        Task<Result> RunAideDdMonster5eEtl();
        Task<Result> ImportMonstersFromImagesAsync();
    }
}
