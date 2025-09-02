using Common.ResultPattern;
using TWP.Api.Application.ETL.Services;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
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
        private readonly ITraitMapperService _traitMapperService;
        private readonly IMonster5eRepository _monster5eRepository;

        public ExtractTransformLoad(
            IAideDdMonster5eRepository aideDdMonster5ERepository,
            IAideDdInterops aideDdInterops,
            IActionMapperService actionMapperService,
            ITraitMapperService traitMapperService,
            IMonster5eRepository monster5eRepository)
        {
            _aideDdMonster5ERepository = aideDdMonster5ERepository;
            _aideDdInterops = aideDdInterops;
            _actionMapperService = actionMapperService;
            _traitMapperService = traitMapperService;
            _monster5eRepository = monster5eRepository;
        }

        public async Task<Result> RunAideDdMonster5eEtl()
            => await Safe.ExecuteAsync(async () =>
            {
                var res = await _aideDdMonster5ERepository.FindByCrOrLessAsync(30);

                var monsterAlreadyLoaded = await _monster5eRepository.GetAllAsync();
                if(monsterAlreadyLoaded.IsFailure)
                    return Result.Failure(monsterAlreadyLoaded.Error, monsterAlreadyLoaded.ReasonType);

                var monsterDbEntities = new List<Monster5eDbEntity>();
                foreach (var aideDdMonsterMetadata in res.Data)
                {
                    if (monsterAlreadyLoaded.Data.Select(m => m.Name).Contains(aideDdMonsterMetadata.Name))
                        continue;

                    AideDdMonsterResponseDto aideDdMonster = new();

                    //Quick Fix : Related to AideDD data issues :/
                    //if (aideDdMonsterMetadata.Name == "will-o--wisp")
                    //    continue;//aideDdMonster = await _aideDdInterops.GetMonsterByName("will-o--wisp");

                    //if (aideDdMonsterMetadata.Name == "Yuan-ti Malison (Type 1)")
                    //    aideDdMonster = await _aideDdInterops.GetMonsterByName("yuan-ti-malison-type-1");

                    //if (aideDdMonsterMetadata.Name == "Yuan-ti Malison (Type 2)")
                    //    aideDdMonster = await _aideDdInterops.GetMonsterByName("yuan-ti-malison-type-2");

                    //if (aideDdMonsterMetadata.Name == "Yuan-ti Malison (Type 3)")
                    //    aideDdMonster = await _aideDdInterops.GetMonsterByName("yuan-ti-malison-type-3");

                    // Extract: Get monster data from AideDD
                    aideDdMonster = aideDdMonster is null ? await _aideDdInterops.GetMonsterByName(aideDdMonsterMetadata.Name) : aideDdMonster;

                    // Transform: Convert to DB entity
                    var monsterDbEntity = aideDdMonster.ToDbEntity();

                    // Transform: Map Actions using OpenAI
                    monsterDbEntity.Actions = await _actionMapperService.MapMonsterActionsAsync(
                        aideDdMonster,
                        monsterDbEntity.Id);

                    // Transform: Map Traits using OpenAI
                    monsterDbEntity.Traits = await _traitMapperService.MapMonsterTraitsAsync(
                        aideDdMonster,
                        monsterDbEntity.Id);

                    await _monster5eRepository.Insert(monsterDbEntity);
                }

                return Result.Success();
            });
    }
}
