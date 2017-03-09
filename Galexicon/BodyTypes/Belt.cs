using SoftFluent.Windows;
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
    public class Belt : FreeBody
    {
        public Belt()
        {
            SeparationDensity = new Distance { Value = 15.0, Unit = Distance.DistanceUnit.Kilometer };
            LateralThickness = new Distance { Value = 2500.0, Unit = Distance.DistanceUnit.Kilometer };
            VerticalThickness = new Distance { Value = 100.0, Unit = Distance.DistanceUnit.Kilometer };
            Rockyness = new Percent { Value = 70.0 };
            Icyness = new Percent { Value = 50.0 };

            Species = new ObservableCollection<MajorSpecies>();
            PresentFactions = new ObservableCollection<Faction>();
            Economy = new Economy();

            CommonCulture = new Culture();
            CommonGovernment = new Government();

            AverageSurface = new SurfaceComposition();
            SurfaceVariance = new SurfaceComposition();
        }

        public Belt(Belt source) : base(source)
        {
            SeparationDensity = new Distance(source.SeparationDensity);
            LateralThickness = new Distance(source.LateralThickness);
            VerticalThickness = new Distance(source.VerticalThickness);
            Rockyness = new Percent(source.Rockyness);
            Icyness = new Percent(source.Icyness);
            Species = new ObservableCollection<MajorSpecies>(source.Species);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            Economy = new Economy(source.Economy);
            CommonCulture = new Culture(source.CommonCulture);
            CommonGovernment = new Government(source.CommonGovernment);
            AverageSurface = new SurfaceComposition(source.AverageSurface);
            SurfaceVariance = new SurfaceComposition(source.SurfaceVariance);
        }


        [Category("Inhabitants")]
        public ObservableCollection<MajorSpecies> Species { get { return GetProperty<ObservableCollection<MajorSpecies>>(); } set { SetProperty(value); } }

        public Economy Economy { get { return GetProperty<Economy>(); } set { SetProperty(value); } }

        

        public Culture CommonCulture { get { return GetProperty<Culture>(); } set { SetProperty(value); } }

        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        //  If a belt can be simultaneously excavated by different entities, should be List<CivlizationClass> (although
        //      list of associated organizations would be more appropriate)
        //public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }


        public SurfaceComposition AverageSurface { get { return GetProperty<SurfaceComposition>(); } set { SetProperty(value); } }
        public SurfaceComposition SurfaceVariance { get { return GetProperty<SurfaceComposition>(); } set { SetProperty(value); } }


        /// <summary>
        /// Avg distance between each asteroid in the belt in kilometers
        /// </summary>
        public Distance SeparationDensity { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        public Distance LateralThickness { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        public Distance VerticalThickness { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        public Percent Rockyness { get { return GetProperty<Percent>(); } set { SetProperty(value); } }
        public Percent Icyness { get { return GetProperty<Percent>(); } set { SetProperty(value); } }

        //  Radius would be inferred by distance
    }
}
