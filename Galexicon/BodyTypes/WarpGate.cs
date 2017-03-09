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
    public class WarpGate : FreeBody
    {
        public WarpGate()
        {
            PresentFactions = new ObservableCollection<Faction>();
            ConnectedGateIds = new ObservableCollection<uint>() { 0, 0, 0, 0, 0, 0, 0, 0 };
            Economy = new Economy();
            CommonGovernment = new Government();
            Civilization = new CivilizationClass();
        }

        public WarpGate(WarpGate source) : base(source)
        {
            PresentFactions = new ObservableCollection<Faction>(source.PresentFactions);
            ConnectedGateIds = new ObservableCollection<uint>(source.ConnectedGateIds);
            Economy = new Economy(source.Economy);
            CommonGovernment = new Government(source.CommonGovernment);
            Civilization = new CivilizationClass(source.Civilization);
        }

        public Distance Radius { get { return GetProperty<Distance>(); } set { SetProperty(value); } }

        //  'Connectivity' is a property of black holes that creates connections when manifested as wormholes - wormholes are Black Holes that approach integer connectivities (which define the relationships between Black Holes)
        [Description("Dictates how the Gate connects with other Gate - Gates with the same connectivity can be traveled between")]
        [Category("Properties")]
        public ObservableCollection<uint> ConnectedGateIds { get { return GetProperty<ObservableCollection<uint>>(); } set { SetProperty(value); } }
        //public ObservableCollection<int> Connectivity { get { return GetProperty<ObservableCollection<int>>(); } set { SetProperty(value); } }
        //  The closer connectivity is to its integer, the safer/more accurate travel becomes
        //  Connectivity band is effectively round(Connectivity)

        public Economy Economy { get { return GetProperty<Economy>(); } set { SetProperty(value); } }
        

        public Government CommonGovernment { get { return GetProperty<Government>(); } set { SetProperty(value); } }

        public CivilizationClass Civilization { get { return GetProperty<CivilizationClass>(); } set { SetProperty(value); } }

        [Category("Inhabitants")]
        public ObservableCollection<Faction> PresentFactions { get { return GetProperty<ObservableCollection<Faction>>(); } set { SetProperty(value); } }
    }
}
