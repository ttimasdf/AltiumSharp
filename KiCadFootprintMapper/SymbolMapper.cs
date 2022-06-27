using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AltiumSharp;
using AltiumSharp.Records;
using AltiumSharp.BasicTypes;

namespace KiCadFootprintMapper
{
    internal class SymbolMapper
    {
        public bool IsLoaded { get { return MappingData.Count > 0; } }
        JObject MappingData { get; }


        public SymbolMapper()
        {
            MappingData = new JObject();
        }

        public SymbolMapper(string fileName)
        {
            using (StreamReader reader = File.OpenText(fileName))
            {
                MappingData = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }
        }

        public void ApplyMapping(SchLib lib, string libName)
        {
            var libMeta = MappingData[libName];

            for (int i = 0; i < lib.Header.FontId.Count(); i++)
            {
                var item = lib.Header.FontId[i];
                if (item.FontName == "Yu Gothic UI Light")
                {
                    lib.Header.FontId[i] = ("Source Serif Pro", 11, item.Rotation, item.Italic, item.Bold, item.Underline);
                }
            }

            foreach (var item in lib.Items)
            {
                var componentMeta = libMeta![(item as IComponent).Name]!;
                bool shouldAddDescription = true;
                var schImps = componentMeta["ki_fp_candidates"]?.Select(f => new SchImplementation
                {
                    ModelName = (string)f!,
                    ModelType = "PCBLIB",
                    DataFileEntities = new() { (string)f! },
                    DataFileKinds = new() { "PCBLib" },
                    DataFileNames = new(),
                    // DataFileNames = new() { $"wtf.PcbLib" },
                });

                // Construct footprint list
                SchImplementationList fpList = new();
                if (schImps != null)
                {
                    foreach (var si in schImps)
                    {
                        fpList.Add(si);
                    }
                }
                item.Implementations = fpList;

                // fix pin names
                List<SchPin> pins = item.GetPrimitivesOfType<SchPin>().ToList();
                List<SchRectangle> rectangles = item.GetPrimitivesOfType<SchRectangle>().ToList();
                foreach (var pin in pins)
                {
                    if (pins.Count <= 2 && rectangles.Count == 0)
                    {
                        pin.IsDesignatorVisible = false;
                        pin.IsNameVisible = false;
                    }
                    else if (pin.IsDesignatorVisible)
                        pin.IsNameVisible = false;
                }

                // Fix parameter names
                foreach (var param in item.GetPrimitivesOfType<SchParameter>())
                {
                    switch (param.Name)
                    {
                        case "Supplier":
                            param.Name = "Datasheet";
                            break;
                        case "ki_description":
                            shouldAddDescription = false;
                            param.Name = "Description";
                            break;
                        case "Value":
                            // Hide Value in specific conditions

                            // component with few pins
                            if (pins.Count <= 2 && rectangles.Count == 0)
                            {
                                param.IsHidden = true;
                                break;
                            }

                            // component without rectangle
                            //if (rectangles.Count == 0 && item.GetPrimitivesOfType<SchPolygon>().ToList().Count == 0)
                            if (rectangles.Count == 0)
                            {
                                param.IsHidden = true;
                                break;
                            }
                            // component with "small" rectangle
                            foreach (var r in rectangles)
                            {
                                var rect = r.CalculateBounds();
                                if (rect.Width < Coord.FromMils(300) || rect.Height < Coord.FromMils(300))
                                {
                                    param.IsHidden = true;
                                    break;
                                }
                            }
                            break;
                        case "Designator":
                            break;
                        default:
                            param.IsHidden = true;
                            break;
                    }
                }
                // Add description parameter if missing (it should not)
                if (shouldAddDescription && componentMeta["ki_description"]?.Value<string>() is string desc)
                {
                    item.Add(new SchParameter
                    {
                        Name = "Description",
                        Text = desc,
                    });
                }

                // Add "Category" parameter
                item.Add(new SchParameter
                {
                    Name = "Category",
                    Text = libName,
                });

                // correct designator with Reference field
                if (componentMeta["Reference"]?.Value<string>() is string reference)
                {
                    item.Designator.Text = reference + "?";
                }
            }
        }
    }
}
