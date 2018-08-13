using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class ConditionDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public string Grade { get; set; }
    }
}