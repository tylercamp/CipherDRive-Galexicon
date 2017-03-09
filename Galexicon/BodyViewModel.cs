using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalexiconUX
{
    public class BodyViewModel
    {
        [ReadOnly(true)]
        public Galexicon.FreeBody Default { get; set; }
        [ReadOnly(true)]
        public Galexicon.FreeBody Source { get; set; }

        [Browsable(false)]
        public bool IsFocused { get; set; }

        public String Type
        {
            get { return Source.Type.ToString(); }
        }

        public String DetailedType
        {
            get
            {
                switch (Source.Type)
                {
                    case Galexicon.FreeBody.BodyType.Asteroid:
                        var a = Source as Galexicon.BodyTypes.Asteroid;
                        return String.Format("{0} {1} Asteroid", a.Radius, a.AsteroidKind);
                        break;

                    case Galexicon.FreeBody.BodyType.Belt:
                        var b = Source as Galexicon.BodyTypes.Belt;
                        return String.Format("{0} icy, {1} rocky, {2}-separation Belt", b.Icyness, b.Rockyness, b.SeparationDensity);
                        break;

                    case Galexicon.FreeBody.BodyType.BlackHole:
                        var bh = Source as Galexicon.BodyTypes.BlackHole;
                        return String.Format("{0}-mass Black Hole", bh.Mass);
                        break;

                    case Galexicon.FreeBody.BodyType.Comet:
                        var c = Source as Galexicon.BodyTypes.Comet;
                        return String.Format("{0}-radius Comet", c.Radius);
                        break;

                    case Galexicon.FreeBody.BodyType.MegaStructure:
                        var ms = Source as Galexicon.BodyTypes.MegaStructure;
                        return String.Format("{0}-radius Mega-structure", ms.Radius);
                        break;

                    case Galexicon.FreeBody.BodyType.Moon:
                        var m = Source as Galexicon.BodyTypes.Moon;
                        return String.Format("{0:0.#}-radius, {1} Moon", m.Radius, m.MoonKind);
                        break;

                    case Galexicon.FreeBody.BodyType.Planet:
                        var p = Source as Galexicon.BodyTypes.Planet;
                        return String.Format("{0}, {1}-radius {2} Planet", p.Temperature, p.Radius, p.PlanetKind);
                        break;

                    case Galexicon.FreeBody.BodyType.Star:
                        var s = Source as Galexicon.BodyTypes.Star;
                        return String.Format("{0}-yo, {1}, {2}-radius Star", s.Age, s.Temperature, s.Radius);
                        break;

                    case Galexicon.FreeBody.BodyType.Settlement:
                        var st = Source as Galexicon.BodyTypes.Settlement;
                        return String.Format("{0}-radius {1} Station", st.Radius, st.SettlementKind);
                        break;

                    case Galexicon.FreeBody.BodyType.WarpGate:
                        var wg = Source as Galexicon.BodyTypes.WarpGate;
                        return String.Format("{0}-lane WarpGate", wg.ConnectedGateIds.Count);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public String DetailedName
        {
            get
            {
                return Source.Name ?? "Unnamed";
            }
        }
    }
}
