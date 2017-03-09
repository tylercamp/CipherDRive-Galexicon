using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Galexicon.BodyTypes
{
    [Serializable]
    [XmlInclude(typeof(FreeBody))]
    public class Star : FreeBody
    {
        public Star()
        {
            Age = new Time { Value = 2.0, Unit = Time.UnitType.BillionYears };
            Mass = new Mass();
            Radius = new Distance { Value = 3.8, Unit = Distance.DistanceUnit.LightSecond };
            Temperature = new Temperature { Value = 4000.0 };

            PresentFactions = new ObservableCollection<Faction>();
            Economy = new Economy();
            CommonGovernment = new Government();
            Civilization = new CivilizationClass();
        }

        public Star(Star source) : base(source)
        {
            Age = new Time(source.Age);
            Mass = new Mass(source.Mass);
            Radius = new Distance(source.Radius);
            Temperature = new Temperature(source.Temperature);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            Economy = new Economy(source.Economy);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
        }


        public enum SpectralType
        {
            O, B, A, F, G, K, M, D, C
        }

        
        public Economy Economy { get { return GetProperty<Economy>(); } set { SetProperty(value); } }
        
        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }

        [Category("Properties")]
        public SpectralType SpectralKind { get { return GetProperty<SpectralType>(); } set { SetProperty(value); } }


        public Time Age { get { return GetProperty<Time>(); } set { SetProperty(value); } }
        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        public Mass Mass { get { return GetProperty<Mass>(); } set { SetProperty(value); } }
        public Temperature Temperature { get { return GetProperty<Temperature>(); } set { SetProperty(value); } }
    }
}
