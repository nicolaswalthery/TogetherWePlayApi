using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IPathfinder2eBusinessLayer
    {
        public Pf2eMonsterDto GetOneRandomPf2eCoreMonster();
    }
}
