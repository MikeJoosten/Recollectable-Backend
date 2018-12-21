using System;

namespace Recollectable.API.Models.Collections
{
    public class ConditionDto
    {
        public Guid Id { get; set; }
        public string Grade { get; set; }
        public string LanguageCode { get; set; }
    }
}