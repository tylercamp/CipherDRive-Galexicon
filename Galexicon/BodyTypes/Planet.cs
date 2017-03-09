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
    public class Planet : FreeBody
    {
        public Planet()
        {
            Radius = new Distance { Value = 30000.0, Unit = Distance.DistanceUnit.Kilometer };
            Density = 1.0;
            Temperature = new Temperature { Value = 287.0 };
            PlanetKind = PlanetType.Rocky;
            Surface = new SurfaceComposition();

            Species = new ObservableCollection<MajorSpecies>();
            PresentFactions = new ObservableCollection<Faction>();

            Economy = new Economy();
            CommonCulture = new Culture();
            CommonGovernment = new Government();
            Civilization = new CivilizationClass();
            Atmosphere = new AtmosphericComposition();
        }

        public Planet(Planet source) : base(source)
        {
            Radius = new Distance(source.Radius);
            Density = source.Density;
            Temperature = new Temperature(source.Temperature);
            PlanetKind = source.PlanetKind;
            Surface = new SurfaceComposition(source.Surface);
            Species = new ObservableCollection<MajorSpecies>(source.Species);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            Economy = new Economy(source.Economy);
            CommonCulture = new Culture(source.CommonCulture);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
            Atmosphere = new AtmosphericComposition(source.Atmosphere);
        }

        public override string ToString()
        {
            if (IsDwarf.HasValue && IsDwarf.Value == true)
                return "Dwarf " + base.ToString();
            else
                return base.ToString();
        }

        public bool? IsDwarf { get; set; }

        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        [Category("Properties")]
        public double Density { get { return GetProperty<double>(); } set { SetProperty(value); } }

        public enum PlanetType
        {
            Gas,
            Icy,
            Rocky
        }

        [Category("Inhabitants")]
        public ObservableCollection<MajorSpecies> Species { get { return GetProperty<ObservableCollection<MajorSpecies>>(); } set { SetProperty(value); } }

        public Economy Economy { get { return GetProperty<Economy>(); } set { SetProperty(value); } }

        

        public Culture CommonCulture { get { return GetProperty<Culture>(); } set { SetProperty(value); } }

        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }


        public Temperature Temperature { get { return GetProperty<Temperature>(); } set { SetProperty(value); } }
        [Category("Properties")]
        public PlanetType PlanetKind { get { return GetProperty<PlanetType>(); } set { SetProperty(value); } }

        public SurfaceComposition Surface { get { return GetProperty<SurfaceComposition>(); } set { SetProperty(value); } }
        public AtmosphericComposition Atmosphere { get { return GetProperty<AtmosphericComposition>(); } set { SetProperty(value); } }
    }
}
