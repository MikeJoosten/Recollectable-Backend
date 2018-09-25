using Recollectable.Core.Entities.Collections;
using System.Collections.Generic;

namespace Recollectable.Core.DTOs.Users
{
    public class UserCreationDto : UserManipulationDto
    {
        public List<Collection> Collections { get; set; } = new List<Collection>();
    }
}