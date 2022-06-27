using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AltiumSharp.BasicTypes;

namespace AltiumSharp.Records
{
    /// <summary>
    /// Model of a component.
    /// </summary>
    public class SchImplementation : SchPrimitive
    {
        public override int Record => 45;
        public string Description { get; set; }
        public string ModelName { get; set; }
        public string ModelType { get; set; }
        public List<string> DataFileKinds { get; set; }
        public List<string> DataFileEntities { get; set; }
        public List<string> DataFileNames { get; set; }
        public bool IsCurrent { get; set; }
       
        public override void ImportFromParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            base.ImportFromParameters(p);
            Description = p["DESCRIPTION"].AsStringOrDefault();
            ModelName = p["MODELNAME"].AsStringOrDefault();
            ModelType = p["MODELTYPE"].AsStringOrDefault();
            int length = p["DATAFILECOUNT"].AsIntOrDefault();
            DataFileKinds = Enumerable.Range(0, length)
                .Select(i => 
                    p[string.Format(CultureInfo.InvariantCulture, "MODELDATAFILEKIND{0}", i)].AsStringOrDefault())
                .ToList();
            DataFileEntities = Enumerable.Range(0, length)
                .Select(i =>
                    p[string.Format(CultureInfo.InvariantCulture, "MODELDATAFILEENTITY{0}", i)].AsStringOrDefault())
                .ToList();
            DataFileNames = Enumerable.Range(0, length)
                .Select(i =>
                    p[string.Format(CultureInfo.InvariantCulture, "MODELDATAFILE{0}", i)].AsStringOrDefault())
                .ToList();

            IsCurrent = p["ISCURRENT"].AsBool();
        }
        
        public override void ExportToParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            base.ExportToParameters(p);
            p.Add("DESCRIPTION", Description);
            p.Add("MODELNAME", ModelName);
            p.Add("MODELTYPE", ModelType);
            p.Add("DATAFILECOUNT", DataFileKinds.Count);
            for (var i = 0; i < DataFileKinds.Count; i++)
            {
                p.Add(string.Format(CultureInfo.InvariantCulture, "MODELDATAFILEKIND{0}", i), DataFileKinds[i]);
                p.Add(string.Format(CultureInfo.InvariantCulture, "MODELDATAFILEENTITY{0}", i), DataFileEntities[i]);
                if (DataFileNames.Count > i && DataFileNames[i].Length > 0)
                    p.Add(string.Format(CultureInfo.InvariantCulture, "MODELDATAFILE{0}", i), DataFileNames[i]);
            }
            p.Add("ISCURRENT", IsCurrent);
        }
    }
}
