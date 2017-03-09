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
    public class Moon : FreeBody
    {
        public Moon()
        {
            MoonKind = MoonType.Rocky;
            Radius = new Distance { Value = 300.0, Unit = Distance.DistanceUnit.Kilometer };
            Surface = new SurfaceComposition();

            Species = new ObservableCollection<MajorSpecies>();
            PresentFactions = new ObservableCollection<Faction>();

            Economy = new Economy();
            CommonCulture = new Culture();
            CommonGovernment = new Government();
            Civilization = new CivilizationClass();
            Atmosphere = new AtmosphericComposition();
        }

        public Moon(Moon source) : base(source)
        {
            MoonKind = source.MoonKind;
            Radius = new Distance(source.Radius);
            Surface = new SurfaceComposition(source.Surface);
            Species = new ObservableCollection<MajorSpecies>(source.Species);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            Economy = new Economy(source.Economy);
            CommonCulture = new Culture(source.CommonCulture);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
            Atmosphere = new AtmosphericComposition(source.Atmosphere);
        }

        public enum MoonType
        {
            Icy,
            Gas,
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

        [Category("Properties")]
        public MoonType MoonKind { get { return GetProperty<MoonType>(); } set { SetProperty(value); } }
        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }

        public SurfaceComposition Surface { get { return GetProperty<SurfaceComposition>(); } set { SetProperty(value); } }
        public AtmosphericComposition Atmosphere { get { return GetProperty<AtmosphericComposition>(); } set { SetProperty(value); } }
    }
}
