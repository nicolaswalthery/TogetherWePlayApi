using Common.ResultPattern;
using TWP.Api.Application.ETL.Services;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.Interops.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.ETL
{
    public class ExtractTransformLoad : IExtractTransformLoad
    {
        private readonly IAideDdMonster5eRepository _aideDdMonster5ERepository;
        private readonly IAideDdInterops _aideDdInterops;
        private readonly IActionMapperService _actionMapperService;

        public ExtractTransformLoad(IAideDdMonster5eRepository aideDdMonster5ERepository, IAideDdInterops aideDdInterops, IActionMapperService actionMapperService)
        {
            _aideDdMonster5ERepository = aideDdMonster5ERepository;
            _aideDdInterops = aideDdInterops;
            _actionMapperService = actionMapperService;
        }

        public async Task<Result> RunAideDdMonster5eEtl()
            => await Safe.ExecuteAsync(async () =>
            {
                var res = await _aideDdMonster5ERepository.FindByCrOrLessAsync(30);

                var monsterDbEntities = new List<Monster5eDbEntity>();
                foreach (var aideDdMonsterMetadata in res.Data)
                {
                    var aideDdMonster = await _aideDdInterops.GetMonsterByName(aideDdMonsterMetadata.Name);
                    var monsterDbEntity = aideDdMonster.ToDbEntity();

                    monsterDbEntity.Actions = await _actionMapperService.MapMonsterActionsAsync(aideDdMonster, monsterDbEntity.Id);
                    
                    monsterDbEntities.Add(aideDdMonster.ToDbEntity());
                }

                //Todo Load

                return Result.Success();
            });
    }
}
