using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Galexicon.BodyTypes
{
    [Serializable]
    [XmlInclude(typeof(FreeBody))]
    public class BlackHole : FreeBody
    {
        public BlackHole()
        {
            Mass = new Mass { Value = 4.0, Unit = Mass.MassUnit.SolarMasses };
        }

        public BlackHole(BlackHole source) : base(source)
        {
            Mass = new Mass(source.Mass);
        }


        public Mass Mass { get { return GetProperty<Mass>(); } set { SetProperty(value); } }
    }
}
