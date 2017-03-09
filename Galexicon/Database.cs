using FuzzyString;
using Galexicon.BodyTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Galexicon
{
    public class Database
    {
        static FreeBody LoadBody(String bodyFileName)
        {
            var genericParser = new XmlSerializer(typeof(FreeBody));
            String typeName = null;

            FreeBodyFile asBodyFile = null;

            using (var f = new FileStream(bodyFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(f);
                typeName = doc.DocumentElement.Name;

                f.Seek(0, SeekOrigin.Begin);


                XmlSerializer fileParser;
                switch (Path.GetExtension(bodyFileName))
                {
                    default:
                        throw new NotImplementedException();

                    case ".fb": // FreeBody
                        fileParser = new XmlSerializer(typeof(FreeBody));
                        break;

                    case ".fbd": // FreeBody Defaults/...Definition? Needs new model
                        throw new NotImplementedException();
                        fileParser = new XmlSerializer(typeof(FreeBodyFile));
                        break;

                }

                //var file = (FreeBodyFile)fileParser.Deserialize(f);
                //Debugger.Break();
            }


            var typeMap = new Dictionary<String, Type>()
            {
                { "Asteroid", typeof(BodyTypes.Asteroid) },
                { "Belt", typeof(BodyTypes.Belt) },
                { "BlackHole", typeof(BodyTypes.BlackHole) },
                { "Comet", typeof(BodyTypes.Comet) },
                { "MegaStructure", typeof(BodyTypes.MegaStructure) },
                { "Moon", typeof(BodyTypes.Moon) },
                { "Planet", typeof(BodyTypes.Planet) },
                { "Star", typeof(BodyTypes.Star) },
                { "Settlement", typeof(BodyTypes.Settlement) },
                { "WarpGate", typeof(BodyTypes.WarpGate) },
            };

            var type = typeMap[typeName];
            var specificParser = new XmlSerializer(type);

            using (var f = new FileStream(bodyFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fb = (FreeBody)specificParser.Deserialize(f);
                fb.CelestialContexts.RemoveAll(c => c.FrameId == fb.Id); // Remove cases where the body is parented to itself
                return fb;
            }
        }

        String SaveBody(String bodyFile, FreeBody body)
        {
            switch (body.Type)
            {
                case FreeBody.BodyType.Asteroid: return SaveBody(bodyFile, body as Asteroid);
                case FreeBody.BodyType.Belt: return SaveBody(bodyFile, body as Belt);
                case FreeBody.BodyType.BlackHole: return SaveBody(bodyFile, body as BlackHole);
                case FreeBody.BodyType.Comet: return SaveBody(bodyFile, body as Comet);
                case FreeBody.BodyType.MegaStructure: return SaveBody(bodyFile, body as MegaStructure);
                case FreeBody.BodyType.Moon: return SaveBody(bodyFile, body as Moon);
                case FreeBody.BodyType.Planet: return SaveBody(bodyFile, body as Planet);
                case FreeBody.BodyType.Star: return SaveBody(bodyFile, body as Star);
                case FreeBody.BodyType.Settlement: return SaveBody(bodyFile, body as Settlement);
                case FreeBody.BodyType.WarpGate: return SaveBody(bodyFile, body as WarpGate);
            }

            throw new InvalidOperationException();
        }

        String SaveBody<T>(String bodyFile, T body) where T : FreeBody
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FreeBodyFile));
            var file = new FreeBodyFile
            {
                Body = body,
                Default = _WorkingDefaults[body]
            };

            using (var xmlStream = new MemoryStream())
            using (TextWriter fileStream = new StreamWriter(xmlStream))
            {
                serializer.Serialize(fileStream, body);
                fileStream.Flush();
                xmlStream.Seek(0, SeekOrigin.Begin);
                String text = new StreamReader(xmlStream).ReadToEnd();
                File.WriteAllText(bodyFile, text);
                return text;
            }
        }


        public static Database LoadFrom(String directory)
        {
            var bodies = Directory.EnumerateFiles(directory, "*.fb");

            Database result = new Database();
            foreach (var f in bodies)
            {
                if (f.Contains("conflict")) continue;

#if !DEBUG
                try
                {
#endif
                    var b = LoadBody(f);
                    result._Bodies.Add(LoadBody(f));
                    result._WorkingSerializations.Add(b, new FileXmlMap { Filename = f, Xml = File.ReadAllText(f) });
#if !DEBUG
                }

                catch (Exception e)
                {
                    throw new Exception("Unable to load body from file: " + f);
                }
#endif
            }

            foreach (var c in result.CelestialBodies)
            {
                var body = result.FindBodyById(c.BodyId);
                var frame = result.FindBodyById(c.FrameId);

                if (body == null || frame == null)
                    throw new Exception("Body or frame could not be found!");

                c.SetFrame(frame);
                c.SetBody(body);
            }

            var duplicates = result.Bodies.Where(b => result.Bodies.Count(o => o.Id == b.Id) > 1);
            if (duplicates.Count() > 0)
            {
                throw new Exception("Cannot load! Invalid db state!");
            }

            result.StorageDirectory = directory;

            return result;
        }

        public String StorageDirectory = null;

        class FileXmlMap
        {
            public String Filename;
            public String Xml;
        }

        //  When diffing, each body is serialized to XML and tested against the current working-model of already-serialized bodies
        //  Reload from disk refills _WorkingSerializations as well
        private Dictionary<FreeBody, FileXmlMap> _WorkingSerializations = new Dictionary<FreeBody, FileXmlMap>();
        private Dictionary<FreeBody, FreeBody> _WorkingDefaults = new Dictionary<FreeBody, FreeBody>();
        public void Save(String forcedDirectory = null)
        {
            String directory = (forcedDirectory == null ? StorageDirectory : forcedDirectory);

            //  Save each body as individual file with name indicating type, current name, and some random number
            //throw new NotImplementedException();

            var duplicates = Bodies.Where(b => Bodies.Count(o => o.Id == b.Id) > 1);
            if (duplicates.Count() > 0)
            {
                throw new Exception("Cannot save! Invalid db state!");
            }
            
            foreach (var body in Bodies)
            {
                var map = _WorkingSerializations.Where(p => p.Key.Id == body.Id).Select(p => p.Value);
                if (map.Count() > 0)
                {
                    var m = map.Single();
                    var xml = SaveBody(m.Filename, body);
                    m.Xml = xml;
                }
                else
                {
                    var fname = Path.Combine(directory, String.Format("{0}_{1}.fb", body.Type, body.Id));
                    var xml = SaveBody(fname, body);
                    _WorkingSerializations.Add(body, new FileXmlMap { Filename = fname, Xml = xml });
                }
            }
        }

        public IEnumerable<FreeBody> Bodies { get { return _Bodies; } }
        private SortedSet<FreeBody> _Bodies = new SortedSet<FreeBody>();

        public DefaultsCollection Defaults;

        public IEnumerable<Asteroid> Asteroids { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.Asteroid).Cast<Asteroid>(); } }
        public IEnumerable<Belt> Belts { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.Belt).Cast<Belt>(); } }
        public IEnumerable<BlackHole> BlackHoles { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.BlackHole).Cast<BlackHole>(); } }
        public IEnumerable<Comet> Comets { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.Comet).Cast<Comet>(); } }
        public IEnumerable<MegaStructure> MegaStructures { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.MegaStructure).Cast<MegaStructure>(); } }
        public IEnumerable<Moon> Moons { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.Moon).Cast<Moon>(); } }
        public IEnumerable<Planet> Planets { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.Planet).Cast<Planet>(); } }
        public IEnumerable<Star> Stars { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.Star).Cast<Star>(); } }
        public IEnumerable<Settlement> Stations { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.Settlement).Cast<Settlement>(); } }
        public IEnumerable<WarpGate> WarpGates { get { return Bodies.Where(b => b.Type == FreeBody.BodyType.WarpGate).Cast<WarpGate>(); } }
        public IEnumerable<CelestialBody> CelestialBodies { get { return Bodies.SelectMany(f => f.CelestialContexts); } }


        public FreeBody FindBodyById(ulong id)
        {
            return Bodies.SingleOrDefault(b => b.Id == id);
        }


        
        public OrbitalFrame FrameFor(FreeBody body)
        {
            if (body == null)
                return null;

            OrbitalFrame frame = new OrbitalFrame();
            //  Search for cyclic references between bodies (merged frames)

            var cyclicBodies =
                CelestialBodies.Where(c => c.FrameId == body.Id) // for all celestials orbiting this object
                .Where(c => body.CelestialContexts.Any(cc => cc.FrameId == c.Body.Id)) // for all orbiters that have a celestial-context whose frame is this object
                .Select(c => c.Body); // select those orbiters
            frame.Reference.Add(body);
            frame.Reference.AddRange(cyclicBodies);


            //  Find children of merged frame celestials
            var children = CelestialBodies.Where(c => frame.Reference.Any(r => r.Id == c.FrameId));
            frame.CelestialBodies.AddRange(children);
            frame.CelestialBodies.RemoveAll(c => frame.Reference.Any(r => r.Id == c.BodyId));



            frame.CelestialBodies = frame.CelestialBodies.OrderBy(cb => cb.CenterDistance.AsMeters).ToList();

            return frame;
        }

        /*
         * A default is a set of assumptions about some system
         * Some properties based on a default may have different values
         * We want to keep these properties but also maintain relativity to the default
         * Particularly - the default also represents an additional intent for the subject; the default acts as its context
         *          .. a default needs a DATE of last significant edit
         * 
         * As such, for continuous default manipulation, a history of the default must be kept in order to recognize
         * previous references (and thus synchronize procedural manipulations)
         */

        public void ChangeDefault(FreeBody body, FreeBody newDefault)
        {
            //  update the defaults used in the body's values, changes the associated default
        }

        public void Add(FreeBody body, FreeBody defaultSample)
        {
            _Bodies.Add(body);
            _WorkingDefaults.Add(body, defaultSample);
        }

        public void Delete(FreeBody body)
        {
            var frame = FrameFor(body);
            //  If the body shares a frame, transfer its celestials to the co-body
            if (frame.Reference.Count > 1)
            {
                var alternate = frame.Reference.First(r => r.Id != body.Id);
                foreach (var celestial in frame.CelestialBodies)
                {
                    if (celestial.FrameId == body.Id)
                        celestial.SetFrame(alternate);
                }
            }
            else
            {
                //  Otherwise it's removed as a reference from children referencing it
                foreach (var celestial in frame.CelestialBodies)
                {
                    celestial.Body.CelestialContexts.Remove(celestial);
                }
            }

            

            _Bodies.Remove(body);
            _WorkingDefaults.Remove(body);

            File.Delete(_WorkingSerializations.Single(p => p.Key.Id == body.Id).Value.Filename);
            _WorkingSerializations.Remove(body);
        }

        //  (from elsewhere)
        int Levenshtein(String a, String b)
        {
            if (string.IsNullOrEmpty(a))
            {
                if (!string.IsNullOrEmpty(b))
                {
                    return b.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(b))
            {
                if (!string.IsNullOrEmpty(a))
                {
                    return a.Length;
                }
                return 0;
            }

            Int32 cost;
            Int32[,] d = new int[a.Length + 1, b.Length + 1];
            Int32 min1;
            Int32 min2;
            Int32 min3;

            for (Int32 i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }

            for (Int32 i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }

            for (Int32 i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (Int32 j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    cost = Convert.ToInt32(!(a[i - 1] == b[j - 1]));

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }

        private int RankBody(String search, FreeBody body)
        {
            List<int> diffs = new List<int>();
            diffs.Add(Levenshtein(search, body.Name));
            diffs.Add(Levenshtein(search, body.Type.ToString()));
            diffs.Add(Levenshtein(search, body.Notes));

            diffs.Add(Math.Max(0, search.Length - body.Name.LongestCommonSubsequence(search).Length));
            diffs.Add(Math.Max(0, search.Length - body.Type.ToString().LongestCommonSubsequence(search).Length));
            diffs.Add(Math.Max(0, search.Length - body.Notes.LongestCommonSubsequence(search).Length));

            switch (body.Type)
            {
                case FreeBody.BodyType.MegaStructure:
                    diffs.Add(Levenshtein(search, (body as MegaStructure).StructureKind.ToString()));
                    break;
            }

            return diffs.Min();
        }

        public IEnumerable<FreeBody> SearchText(String text)
        {
            return Bodies
                .Where(b => RankBody(text, b) <= text.Length / 2)
                .OrderBy(b => RankBody(text, b));
        }
    }
}
