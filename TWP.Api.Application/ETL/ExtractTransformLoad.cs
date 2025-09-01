using Common.ResultPattern;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DbEntities;
using TWP.Api.Infrastructure.Interops.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.ETL
{
    public class ExtractTransformLoad : IExtractTransformLoad
    {
        private readonly IAideDdMonster5eRepository _aideDdMonster5ERepository;
        private readonly IAideDdInterops _aideDdInterops;

        public ExtractTransformLoad(IAideDdMonster5eRepository aideDdMonster5ERepository, IAideDdInterops aideDdInterops)
        {
            _aideDdMonster5ERepository = aideDdMonster5ERepository;
            _aideDdInterops = aideDdInterops;
        }

        public async Task<Result> RunAideDdMonster5eEtl()
            => await Safe.ExecuteAsync(async () =>
            {
                var res = await _aideDdMonster5ERepository.FindByCrOrLessAsync(30);

                var monsterDbEntities = new List<Monster5eDbEntity>();
                foreach (var monster in res.Data)
                {
                    var aideDdMonster = await _aideDdInterops.GetMonsterByName(monster.Name);
                    monsterDbEntities.Add(aideDdMonster.ToDbEntity());
                }

                //Todo Load

                return Result.Success();
            });
    }
}
