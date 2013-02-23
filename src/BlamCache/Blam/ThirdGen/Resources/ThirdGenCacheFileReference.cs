﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtryzeDLL.Flexibility;

namespace ExtryzeDLL.Blam.ThirdGen.Resources
{
    public class ThirdGenCacheFileReference
    {
        public ThirdGenCacheFileReference(StructureValueCollection values)
        {
            Load(values);
        }

        public string Path { get; set; }

        private void Load(StructureValueCollection values)
        {
            Path = values.GetString("map path");
        }
    }
}
