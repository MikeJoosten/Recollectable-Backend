﻿using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Locations;

namespace Recollectable.Core.Models.Collectables
{
    public class CoinCreationDto : CoinManipulationDto
    {
        public Country Country { get; set; }
        public CollectorValue CollectorValue { get; set; }
    }
}