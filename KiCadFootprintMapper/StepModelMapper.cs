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
    internal class StepModelMapper
    {
        public bool IsLoaded { get { return MappingData.Count > 0; } }
        JObject MappingData { get; }
        public string StepModelDirectory { get; set; }

        public int CountHasModel { get; set; }
        public int CountNoneModel { get; set; }


        public StepModelMapper()
        {
            MappingData = new JObject();
            StepModelDirectory = "";
        }

        public StepModelMapper(string fileName, string stepModelDirectory)
        {
            using (StreamReader reader = File.OpenText(fileName))
            {
                MappingData = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }
            StepModelDirectory = stepModelDirectory;
        }

        public (int, int) ApplyMapping(PcbLib lib)
        {
            foreach (var item in lib.Items)
            {
                string itemName = (item as IComponent).Name;
                JToken fpMeta = MappingData[itemName]!;
                var modelInfo = fpMeta["model"]!.ToList().FirstOrDefault();
                if (((string?)modelInfo?["status"]) != "success")
                {
                    CountNoneModel += 1;
                    continue;
                }
                var modelPath = modelInfo["path"]!.ToString().Replace("{KICAD6_3DMODEL_DIR}", StepModelDirectory).Replace(".wrl", ".step");
                Coord xmin = JTokenToCoord(modelInfo["xmin"]!);
                Coord xmax = JTokenToCoord(modelInfo["xmax"]!);
                Coord ymin = JTokenToCoord(modelInfo["ymin"]!);
                Coord ymax = JTokenToCoord(modelInfo["ymax"]!);
                Coord zmin = JTokenToCoord(modelInfo["zmin"]!, true);
                Coord zmax = JTokenToCoord(modelInfo["zmax"]!, true);
                var rotation = modelInfo["rotate"]!;
                double rotx = -((double)rotation["x"]!);
                double roty = -((double)rotation["y"]!);
                double rotz = -((double)rotation["z"]!);
                string? fileContent;
                using (StreamReader reader = File.OpenText(modelPath))
                {
                    fileContent = reader.ReadToEnd();
                }
                var componentBody = new PcbComponentBody
                {
                    // MANDATORY Parameters
                    Identifier = itemName,
                    ModelId = $"{{{Guid.NewGuid()}}}",
                    StepModel = fileContent,
                    StandOffHeight = zmin,
                    OverallHeight = zmax,
                    Model3DRotX = rotx,
                    Model3DRotY = roty,
                    Model3DRotZ = rotz,
                    // Model2DRotation = rotz,

                    // nonsense default values
                    Name = " ",
                    ArcResolution = Coord.FromMils(0.5),
                    Layer = Layer.Mechanical1,
                };
                componentBody.Outline.Add(RotatedCoordPoint(xmin, ymin, rotz));
                componentBody.Outline.Add(RotatedCoordPoint(xmax, ymin, rotz));
                componentBody.Outline.Add(RotatedCoordPoint(xmax, ymax, rotz));
                componentBody.Outline.Add(RotatedCoordPoint(xmin, ymax, rotz));

                item.Add(componentBody);
                CountHasModel += 1;
            }
            return (CountHasModel, CountNoneModel);
        }

        private Coord JTokenToCoord(JToken jt, bool closeToZero = false)
        {
            double num = jt.Value<double>();
            if (closeToZero && Math.Round(num, 1) == 0)
                num = 0;
            else if ((int)(num * 1000) % 10 == 1)
                num = Math.Round(num, 2);
            else
                num = Math.Round(num, 6);
            return Coord.FromMMs(num);
        }

        /// <summary>
        /// Create an counter-clockwise rotated CoordPoint based on input coord x, y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private CoordPoint RotatedCoordPoint(Coord x, Coord y, double rotation)
        {
            switch (rotation)
            {
                case 0:
                    return new CoordPoint(x, y);
                case 90:
                    return new CoordPoint(-y, x);
                case 180:
                    return new CoordPoint(-x, -y);
                case -90:
                    return new CoordPoint(y, -x);
                default:
                    throw new NotImplementedException($"rotation value {rotation} not supported");
            }
        }
    }
}
