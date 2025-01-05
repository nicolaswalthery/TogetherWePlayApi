using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IUltraModern5eController
    {
        public ShootAndLootDto GetShootAndLoot();
    }
}
