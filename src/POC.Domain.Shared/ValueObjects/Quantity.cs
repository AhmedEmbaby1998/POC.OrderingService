﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POC.Shared.ValueObjects
{
    public class Quantity
    {
        [JsonConstructor]
        public Quantity(double count, string unit)
        {
            Count = count;
            Unit = unit;
        }

        public double Count { get; private set; }
        public string Unit { get; private set; }
    }
}
