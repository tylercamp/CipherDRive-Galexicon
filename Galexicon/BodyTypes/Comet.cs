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
    public class Comet : FreeBody
    {
        public Comet()
        {
            Radius = new Distance { Value = 20.0, Unit = Distance.DistanceUnit.Kilometer };
            CometKind = CometType.Icy;
            Composition = new SurfaceComposition();

            PresentFactions = new ObservableCollection<Faction>();

            CommonGovernment = new Government();
            Civilization = new CivilizationClass();
        }

        public Comet(Comet source) : base(source)
        {
            Radius = new Distance(source.Radius);
            CometKind = source.CometKind;
            Composition = new SurfaceComposition(source.Composition);
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
        }


        public enum CometType
        {
            Icy,
            Rocky
        }

        //  'Survey' comets have scanning devices that collect data and contribute to an organization's notification network
        [Category("Properties")]
        public bool Survey { get { return GetProperty<bool>(); } set { SetProperty(value); } }

        [Category("Properties")]
        public bool Artificial { get { return GetProperty<bool>(); } set { SetProperty(value); } }

        

        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        //  Civilization class determines what kind/range of tech you'll find on a survey comet
        [Description("A Comet's CivlizationClass determines what range of tech you'll find on a survey comet.")]
        public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }


        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }
        [Category("Properties")]
        public CometType CometKind { get { return GetProperty<CometType>(); } set { SetProperty(value); } }

        public SurfaceComposition Composition { get { return GetProperty<SurfaceComposition>(); } set { SetProperty(value); } }
    }
}
