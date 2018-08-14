using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class UserCreationDto : UserManipulationDto
    {
        public List<Collection> Collections { get; set; } = new List<Collection>();
    }
}