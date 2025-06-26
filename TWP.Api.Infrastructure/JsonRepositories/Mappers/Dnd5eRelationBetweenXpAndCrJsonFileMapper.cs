using System.Collections.Generic;
using Common.Extensions;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class Dnd5eRelationBetweenXpAndCrJsonFileMapper
    {
        public static List<Dnd5eRelationBetweenXpAndCrDto> ToDnd5eRelationBetweenXpAndCrDtoList(this string json)
        {
            var root = json.JsonToObject<Dnd5eRelationBetweenXpAndCrRoot>();
            return root.Dnd5eRelationBetweenXpAndCr ?? new List<Dnd5eRelationBetweenXpAndCrDto>();
        }

        private class Dnd5eRelationBetweenXpAndCrRoot
        {
            public List<Dnd5eRelationBetweenXpAndCrDto> Dnd5eRelationBetweenXpAndCr { get; set; }
        }
    }
} 