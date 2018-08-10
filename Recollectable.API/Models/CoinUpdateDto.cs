﻿using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Models
{
    public class CoinUpdateDto
    {
        public int FaceValue { get; set; }
        public string Type { get; set; }
        public string ReleaseDate { get; set; }
        public int Mintage { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public string Metal { get; set; }
        public string Note { get; set; }
        public string Subject { get; set; }
        public string ObverseDescription { get; set; }
        public string ObverseInscription { get; set; }
        public string ObverseLegend { get; set; }
        public string ReverseDescription { get; set; }
        public string ReverseInscription { get; set; }
        public string ReverseLegend { get; set; }
        public string EdgeType { get; set; }
        public string EdgeLegend { get; set; }
        public string Designer { get; set; }
        public string HeadOfState { get; set; }
        public string MintMark { get; set; }
        public Guid CountryId { get; set; }
        public Guid CollectorValueId { get; set; }
    }
}