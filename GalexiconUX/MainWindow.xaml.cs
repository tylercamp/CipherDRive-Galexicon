using Galexicon;
using Galexicon.BodyTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

//  This is a mess of code that provides a useable interface for the relevant data

namespace GalexiconUX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String dbPath;
        //Galexicon.Database db = null;
        Galexicon.Database db;

        Galexicon.FreeBody m_FocusedBody;
        //Galexicon.OrbitalFrame m_CurrentFrame;
        Galexicon.CelestialBody m_CurrentCelestial;

        IEnumerable<BodyViewModel> m_Bodies;
        String m_CurrentDefaultsFile;
        Galexicon.DefaultsCollection m_Defaults;

        System.Windows.Forms.PropertyGrid FormsBodyViewer = new System.Windows.Forms.PropertyGrid();

        public bool HasDb { get { return db != null; } }
        public bool HasFocusBody { get { return m_FocusedBody != null; } }



        void SetFocus(Galexicon.FreeBody body)
        {
            if (body.Type == FreeBody.BodyType.Belt)
            {
                MessageBox.Show("Belt focus not yet implemented");
                return;
            }
            
            m_FocusedBody = body;

            //lblCurrentFrame.Text = String.Format("Frame: {0} - {1}", frame.FrameKind, (frame.Name == null || frame.Name == String.Empty) ? "Unnamed" : frame.Name);
            SetCurrentCelestial(null);

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
        }

        void SetCurrentCelestial(Galexicon.CelestialBody body)
        {
            m_CurrentCelestial = body;

            if (body == null)
            {
                BodyViewer.SelectedObject = null;
                lblCurrentCelestial.Text = "Celestial: None";
                return;
            }



            SelectBody(m_CurrentCelestial);
        }


        void Refresh(bool list, bool hierarchy, bool frame, bool properties, bool defaults)
        {
            if (list)
            {
                var bodiesVM = m_Bodies.ToList();
                if (m_FocusedBody != null)
                {
                    bodiesVM.Single(b => b.Source.Id == m_FocusedBody.Id).IsFocused = true;
                }

                FreeBodies.ItemsSource = bodiesVM;
            }

            var orbitFrame = db?.FrameFor(m_FocusedBody);

            if (hierarchy)
            {
                UpdateHierarchyTree(orbitFrame);
                UpdateNullHierarchyTree();
            }

            if (frame)
                BuildFrameView(orbitFrame);

            if (properties)
                BodyViewer.SelectedObject = BodyViewer.SelectedObject;

            if (defaults)
                UpdateDefaultsView();
        }

        void UpdateDefaultsView()
        {
            DefaultsView.Items.Clear();

            MouseButtonEventHandler clickHandler = (o, e) =>
            {
                //var body = (o as )
                //SelectBody()

                var item = o as TreeViewItem;
                var body = item.DataContext as FreeBody;
                SelectBody(body);
            };

            Action<IEnumerable<FreeBody>, String> add = (fbs, n) =>
            {
                var baseItem = new TreeViewItem();
                baseItem.Header = n;
                baseItem.ContextMenu = FindResource("DefaultCategoryContextMenu") as ContextMenu;

                /*
                var defaultItem = new TreeViewItem();
                defaultItem.Header = "Default";
                defaultItem.ContextMenu = FindResource("DefaultObjectContextMenu") as ContextMenu;
                defaultItem.PreviewMouseUp += clickHandler;
                baseItem.Items.Add(defaultItem);
                */

                foreach (var defaultBody in fbs)
                {
                    var newDefault = new TreeViewItem();
                    newDefault.Header = defaultBody.Name;
                    newDefault.DataContext = defaultBody;

                    newDefault.PreviewMouseUp += clickHandler;
                    newDefault.ContextMenu = FindResource("DefaultObjectContextMenu") as ContextMenu;

                    baseItem.Items.Add(newDefault);
                }

                DefaultsView.Items.Add(baseItem);
            };

            if (m_Defaults != null)
            {
                add(m_Defaults.DefaultAsteroids.Cast<FreeBody>(), "Asteroid");
                add(m_Defaults.DefaultBelts.Cast<FreeBody>(), "Belt");
                add(m_Defaults.DefaultBlackHoles.Cast<FreeBody>(), "Black hole");
                add(m_Defaults.DefaultComets.Cast<FreeBody>(), "Comet");
                add(m_Defaults.DefaultMegaStructures.Cast<FreeBody>(), "Mega-structure");
                add(m_Defaults.DefaultMoons.Cast<FreeBody>(), "Moon");
                add(m_Defaults.DefaultPlanets.Cast<FreeBody>(), "Planet");
                add(m_Defaults.DefaultSettlements.Cast<FreeBody>(), "Settlement");
                add(m_Defaults.DefaultStars.Cast<FreeBody>(), "Star");
                add(m_Defaults.DefaultWarpGates.Cast<FreeBody>(), "Warp gate");
            }
        }



        void LoadGalexicon(String path)
        {
            dbPath = path;
            db = Galexicon.Database.LoadFrom(path);

            m_CurrentCelestial = null;
            m_FocusedBody = null;
            m_Bodies = db.Bodies.Select(b => new BodyViewModel { Source = b });

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: true);

            //  Refresh datacontext
            this.DataContext = null;
            this.DataContext = this;
        }

        class Node
        {
            public object Value;
            public List<Node> Children = new List<Node>();

            public Node FindNodeFor(object value)
            {
                if (Value == value) return this;

                foreach (var graph in Children)
                {
                    var found = graph.FindNodeFor(value);
                    if (found != null)
                        return found;
                }

                return null;
            }
        }

        Node BuildGraphFor(FreeBody body)
        {
            Node result = new Node { Value = body };
            var frame = db.FrameFor(body);
            foreach (var sb in frame.CelestialBodies.Select(c => c.Body))
            {
                var n = BuildGraphFor(sb);
                result.Children.Add(n);
            }
            return result;
        }

        Node BuildGraphFor(IEnumerable<FreeBody> bodies)
        {
            Node root = new Node();
            //  Todo - Read the celestials referencing these bodies to find the major
            //      frames

            foreach (var b in bodies)
            {
                root.Children.Add(BuildGraphFor(b));
            }

            return root;
        }

        void UpdateHierarchyTree(Galexicon.OrbitalFrame frame)
        {
            HierarchyTree.Items.Clear();
            if (db == null)
                return;


            Action<TreeViewItem, Node> copyNode = null;
            copyNode = (tvi, node) =>
            {
                tvi.Header = node.Value.ToString();
                tvi.DataContext = node.Value;
                tvi.IsExpanded = true;
                if (node.Value is CelestialBody)
                {
                    tvi.ContextMenu = FindResource("CelestialContextMenu") as ContextMenu;
                    tvi.PreviewMouseUp += (s, e) => { SelectBody(node.Value as CelestialBody); };
                    tvi.MouseDoubleClick += (s, e) => {
                        if (!((TreeViewItem)s).IsSelected) return;
                        SetFocus((node.Value as CelestialBody).Body);
                    };
                }
                if (node.Value is FreeBody)
                {
                    tvi.ContextMenu = FindResource("FrameContextMenu") as ContextMenu;
                    tvi.PreviewMouseUp += (s, e) => { SelectBody(node.Value as FreeBody); };
                    tvi.MouseDoubleClick += (s, e) => {
                        if (!((TreeViewItem)s).IsSelected) return;
                        SetFocus(node.Value as FreeBody);
                    };
                }

                foreach (Node subnode in node.Children)
                {
                    TreeViewItem newItem = new TreeViewItem();
                    copyNode(newItem, subnode);
                    tvi.Items.Add(newItem);
                }
            };

            if (frame != null)
            {
                var frameMembers = frame.Reference;
                var parentFrames = frameMembers.SelectMany(b => b.CelestialContexts.Select(c => c.Frame)).Distinct().Select(p => db.FrameFor(p));
                var siblings = parentFrames.SelectMany(f => f.CelestialBodies).Distinct();

                var baseGraph = BuildGraphFor(frameMembers);
                baseGraph.Value = frame;

                var graph = new Node();
                graph.Children.Add(baseGraph);
                foreach (var s in siblings)
                    graph.Children.Add(new Node { Value = s });
                //baseGraph.Value


                foreach (Node n in graph.Children)
                {
                    TreeViewItem newItem = new TreeViewItem();
                    copyNode(newItem, n);
                    HierarchyTree.Items.Add(newItem);
                }
            }
        }

        void UpdateNullHierarchyTree()
        {
            NullHierarchyTree.Items.Clear();
            if (db == null)
                return;


            Action<TreeViewItem, Node> copyNode = null;
            copyNode = (tvi, node) =>
            {
                tvi.Header = node.Value.ToString();
                tvi.DataContext = node.Value;
                tvi.IsExpanded = true;
                if (node.Value is CelestialBody)
                {
                    tvi.ContextMenu = FindResource("CelestialContextMenu") as ContextMenu;
                    tvi.PreviewMouseUp += (s, e) => { SelectBody(node.Value as CelestialBody); };
                    tvi.MouseDoubleClick += (s, e) => {
                        if (!((TreeViewItem)s).IsSelected) return;
                        SetFocus((node.Value as CelestialBody).Body);
                    };
                }
                if (node.Value is FreeBody)
                {
                    tvi.ContextMenu = FindResource("FrameContextMenu") as ContextMenu;
                    tvi.PreviewMouseUp += (s, e) =>
                    {
                        SelectBody(node.Value as FreeBody);
                    };
                    tvi.MouseDoubleClick += (s, e) =>
                    {
                        if (!((TreeViewItem)s).IsSelected) return;
                        SetFocus(node.Value as FreeBody);
                    };
                }

                foreach (Node subnode in node.Children)
                {
                    TreeViewItem newItem = new TreeViewItem();
                    copyNode(newItem, subnode);
                    tvi.Items.Add(newItem);
                }
            };

            var nullBodies = db.Bodies.Where(b => b.CelestialContexts.Count == 0);
            //var nullFrames = nullBodies.Select(b => db.FrameFor(b)).Distinct();

            foreach (var b in nullBodies)
            {
                TreeViewItem newItem = new TreeViewItem();
                copyNode(newItem, BuildGraphFor(b));
                NullHierarchyTree.Items.Add(newItem);
            }

            /*
            foreach (Node n in graph.Children)
            {
                TreeViewItem newItem = new TreeViewItem();
                copyNode(newItem, n);
                HierarchyTree.Items.Add(newItem);
            }
            */
        }

        void SelectBody(Galexicon.FreeBody body)
        {
            //FormsBodyViewer.SelectedObject = body;
            BodyViewer.SelectedObject = body;
        }

        void SelectBody(Galexicon.CelestialBody body)
        {
            BodyViewer.SelectedObject = body;
        }


        public MainWindow()
        {
            InitializeComponent();

            if (db != null)
                m_Bodies = db.Bodies.Select(b => new BodyViewModel { Source = b });

            FrameCanvas.LayoutUpdated += FrameCanvas_LayoutUpdated;
            SearchBox.KeyDown += SearchBox_KeyDown;

            this.DataContext = this;
        }

        //  Referenced in BuildFrameView
        bool wasBuilding = false;
        Size? prevSize;
        private void FrameCanvas_LayoutUpdated(object sender, EventArgs e)
        {
            if (wasBuilding)
            {
                wasBuilding = false;
                return;
            }

            if (db == null)
                return;

            if (prevSize.HasValue && prevSize == FrameCanvas.DesiredSize)
                return;



            wasBuilding = true;
            BuildFrameView(db.FrameFor(m_FocusedBody));

            prevSize = FrameCanvas.DesiredSize;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (db == null)
                return;

            BuildFrameView(db.FrameFor(m_FocusedBody));
        }
        

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (db == null)
                return;

            if (e.Key != Key.Enter)
                return;

            String searchValue = SearchBox.Text;
            if (searchValue.Trim() != String.Empty)
            {
                SearchBox.Text = String.Empty;

                if (!SearchBox.Items.Contains(searchValue))
                    SearchBox.Items.Add(searchValue);
                
                FreeBodies.ItemsSource = db.SearchText(searchValue).ToList();
            }
            else
            {
                FreeBodies.ItemsSource = m_Bodies.ToList();
            }
        }


        private void NewGalexicon_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new NewGalexiconWindow();
            newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = newWindow.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                var newDir = Path.Combine(newWindow.DestinationFolder, newWindow.GalexiconName);
                Directory.CreateDirectory(newDir);
                m_Defaults = new Galexicon.DefaultsCollection();
                m_CurrentDefaultsFile = Path.Combine(newDir, newWindow.GalexiconName + ".gd");
                Galexicon.DefaultsCollection.SaveTo(m_CurrentDefaultsFile, m_Defaults);
                LoadGalexicon(newDir);
            }
        }

        T TryMake<T>() where T : FreeBody, new()
        {   
            var defaults = m_Defaults.Defaults<T>();
            FreeBody defaultBody = null;

            if (defaults.Count() == 0)
                defaultBody = new T();
            if (defaults.Count() == 1)
                defaultBody = defaults.Single();

            if (defaultBody == null)
            {
                var selectWindow = new SelectDefaultWindow();
                selectWindow.Defaults = defaults;
                selectWindow.ShowDialog();
                if (selectWindow.Selection == null)
                    return null;

                defaultBody = selectWindow.Selection as T;
            }

            //  Make a new copy of the default
            var body = (T)Activator.CreateInstance(typeof(T), defaultBody);
            db.Add(body, defaultBody);

            db.Save();

            return body;
        }

        private void NewAsteroid_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<Asteroid>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewBelt_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<Belt>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewBlackHole_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<BlackHole>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewComet_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<Comet>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewMegaStructure_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<MegaStructure>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewMoon_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<Moon>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewPlanet_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<Planet>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewStar_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<Star>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewSettlement_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<Settlement>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewWarpGate_Click(object sender, RoutedEventArgs e)
        {
            var body = TryMake<WarpGate>();
            if (body == null)
                return;

            Refresh(list: true, hierarchy: false, frame: false, properties: false, defaults: false);
            SelectBody(body);
        }

        private void FreeBodies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = FreeBodies.SelectedItem as BodyViewModel;
            if (vm == null)
                return;

            SelectBody(vm.Source);
        }

        Galexicon.FreeBody MenuExtractBody(RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var data = menuItem.DataContext;

            //  Note! ContextMenu occurs generally on list view! Attach to List Template!
            //  This is not correct!
            //throw new NotImplementedException();

            if (data != null)
            {
                if (data is Galexicon.FreeBody)
                    return data as Galexicon.FreeBody;

                if (data is BodyViewModel)
                    return (data as BodyViewModel).Source;

                var asControl = data as FrameworkElement;
                if (asControl != null && asControl.DataContext != null)
                {
                    data = asControl.DataContext;
                    if (data is Galexicon.FreeBody)
                        return data as Galexicon.FreeBody;

                    if (data is BodyViewModel)
                        return (data as BodyViewModel).Source;
                }

                return null;
                throw new NotImplementedException();
            }
            else
            {
                MessageBox.Show("Error! This shouldn't happen!");
            }

            return null;
        }

        void BodyMenu_EditBodyClick(object sender, RoutedEventArgs e)
        {
            var body = MenuExtractBody(e);

            SelectBody(body);
        }

        void BodyMenu_RevertToDefault(object sender, RoutedEventArgs e)
        {
            var body = MenuExtractBody(e);

            throw new NotImplementedException();
        }


        void BodyMenu_RemoveFromParent(object sender, RoutedEventArgs e)
        {
            var body = MenuExtractBody(e);

            if (body.CelestialContexts.Count == 0)
            {
                MessageBox.Show("That body has no parent!");
                return;
            }

            if (body.CelestialContexts.Count > 1)
            {
                MessageBox.Show("Body has multiple parents! Can't pick which to remove from! Operation Canceled! (Let Tyler know if you need this)");
                return;
            }

            body.CelestialContexts.Clear();

            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
        }


        void BodyMenu_DeleteClick(object sender, RoutedEventArgs e)
        {
            var body = MenuExtractBody(e);

            if (MessageBox.Show("Are you sure you want to delete this body?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            //  If there's another body in the focus' frame, focus on that instead - otherwise, deselect
            if (m_FocusedBody == body)
            {
                var frame = db.FrameFor(m_FocusedBody);
                if (frame.Reference.Count > 2)
                {
                    frame.Reference.Remove(m_FocusedBody);
                    SetFocus(frame.Reference.First());
                }
                else
                {
                    SetFocus(null);
                }
            }

            db.Delete(body);

            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
        }

        enum DiagramType
        {
            ToScaleDiagram,
            ToScaleModelDiagram,
            ModelDiagram
        };

        void BuildFrameView(Galexicon.OrbitalFrame frame)
        {
            if (FrameCanvas == null)
                return;

            if (frame == null)
            {
                FrameCanvas.Children.Clear();
                return;
            }

            frame.CelestialBodies = frame.CelestialBodies.OrderBy(c => c.CenterDistance.AsMeters).ToList();
            

            var referenceBodies = frame.Reference;
            var primaryCelestials = frame.CelestialBodies;

            var canvas = FrameCanvas;
            canvas.Children.Clear();

            double systemRadius = 0;
            bool showLabels = ShowLabelsButton.IsChecked ?? true;
            bool showSubCelestialLabels = false;


            #region Helpers
            Func<Galexicon.FreeBody, Color> makeBodyColor = (body) =>
            {
                switch (body.Type)
                {
                    case Galexicon.FreeBody.BodyType.Asteroid:
                        switch ((body as Galexicon.BodyTypes.Asteroid).AsteroidKind)
                        {
                            case Galexicon.BodyTypes.Asteroid.AsteroidType.Icy: return new Color { R = 173, G = 254, B = 255, A = 255 };
                            case Galexicon.BodyTypes.Asteroid.AsteroidType.Rocky: return new Color { R = 172, G = 157, B = 126, A = 255 };
                        }
                        break;

                    case Galexicon.FreeBody.BodyType.Belt:
                        return new Color { R = 255, G = 131, B = 48, A = 255 };

                    case Galexicon.FreeBody.BodyType.BlackHole:
                        return new Color { R = 10, G = 10, B = 10, A = 255 };

                    case Galexicon.FreeBody.BodyType.Comet:
                        switch ((body as Galexicon.BodyTypes.Comet).CometKind)
                        {
                            case Galexicon.BodyTypes.Comet.CometType.Icy: return new Color { R = 173, G = 254, B = 255, A = 255 };
                            case Galexicon.BodyTypes.Comet.CometType.Rocky: return new Color { R = 172, G = 157, B = 126, A = 255 };
                        }
                        break;

                    case Galexicon.FreeBody.BodyType.MegaStructure:
                        return new Color { R = 204, G = 204, B = 221, A = 255 };

                    case Galexicon.FreeBody.BodyType.Moon:
                        switch ((body as Galexicon.BodyTypes.Moon).MoonKind)
                        {
                            case Galexicon.BodyTypes.Moon.MoonType.Icy: return new Color { R = 173, G = 254, B = 255, A = 255 };
                            case Galexicon.BodyTypes.Moon.MoonType.Rocky: return new Color { R = 172, G = 157, B = 126, A = 255 };
                            case Galexicon.BodyTypes.Moon.MoonType.Gas: return new Color { R = 217, G = 210, B = 121, A = 255 };

                        }
                        break;

                    case Galexicon.FreeBody.BodyType.Planet:
                        switch ((body as Galexicon.BodyTypes.Planet).PlanetKind)
                        {
                            case Galexicon.BodyTypes.Planet.PlanetType.Icy: return new Color { R = 173, G = 254, B = 255, A = 255 };
                            case Galexicon.BodyTypes.Planet.PlanetType.Rocky: return new Color { R = 172, G = 157, B = 126, A = 255 };
                            case Galexicon.BodyTypes.Planet.PlanetType.Gas: return new Color { R = 217, G = 210, B = 121, A = 255 };

                        }
                        break;

                    case Galexicon.FreeBody.BodyType.Settlement:
                        return new Color { R = 204, G = 204, B = 221, A = 255 };

                    case Galexicon.FreeBody.BodyType.Star:
                        switch ((body as Galexicon.BodyTypes.Star).SpectralKind)
                        {
                            case Galexicon.BodyTypes.Star.SpectralType.O:
                                return new Color { R = 208, G = 255, B = 255, A = 255 };
                            case Galexicon.BodyTypes.Star.SpectralType.B:
                                return new Color { R = 224, G = 255, B = 255, A = 255 };
                            case Galexicon.BodyTypes.Star.SpectralType.A:
                                return new Color { R = 255, G = 255, B = 255, A = 255 };
                            case Galexicon.BodyTypes.Star.SpectralType.F:
                                return new Color { R = 255, G = 255, B = 204, A = 255 };
                            case Galexicon.BodyTypes.Star.SpectralType.G:
                                return new Color { R = 255, G = 255, B = 102, A = 255 };
                            case Galexicon.BodyTypes.Star.SpectralType.K:
                                return new Color { R = 225, G = 119, B = 0, A = 255 };
                            case Galexicon.BodyTypes.Star.SpectralType.M:
                                return new Color { R = 255, G = 51, B = 0, A = 255 };
                        }
                        break;

                    case Galexicon.FreeBody.BodyType.WarpGate:
                        return new Color { R = 204, G = 204, B = 221, A = 255 };
                }

                var color = new Color
                {
                    R = 127,
                    G = 127,
                    B = 127,
                    A = 255
                };
                return color;
            };

            //  Need to size bodies with respect to frame - returns 0 to 1 as % 'radius' of frame
            Func<Galexicon.FreeBody, double> getRealRadius = (body) =>
            {
                //double refRadius = frame.EffectiveRadius;

                //  if refRadius == body.radius, use unit size (ie r=0.15)

                //throw new NotImplementedException();

                switch (body.Type)
                {
                    case FreeBody.BodyType.Asteroid:
                        return (body as Asteroid).Radius.AsMeters;

                    case FreeBody.BodyType.Belt:
                        return (body as Belt).CelestialContexts.First().CenterDistance.AsMeters;

                    case FreeBody.BodyType.BlackHole:
                        return 0;

                    case FreeBody.BodyType.Comet:
                        return (body as Comet).Radius.AsMeters;

                    case FreeBody.BodyType.MegaStructure:
                        return (body as MegaStructure).Radius.AsMeters;

                    case FreeBody.BodyType.Moon:
                        return (body as Moon).Radius.AsMeters;

                    case FreeBody.BodyType.Planet:
                        return (body as Planet).Radius.AsMeters;

                    case FreeBody.BodyType.Settlement:
                        return (body as Settlement).Radius.AsMeters;

                    case FreeBody.BodyType.Star:
                        return (body as Star).Radius.AsMeters;

                    case FreeBody.BodyType.WarpGate:
                        return (body as WarpGate).Radius.AsMeters; // 50km
                }

                throw new InvalidOperationException();
            };

            //  Need to size bodies with respect to frame - returns 0 to 1 as % 'radius' of frame
            Func<Galexicon.FreeBody, double> getBodyRadius = (body) =>
            {
                //double refRadius = frame.EffectiveRadius;

                //  if refRadius == body.radius, use unit size (ie r=0.15)

                //throw new NotImplementedException();

                bool isCelestial = !frame.Reference.Any(r => r == body);
                bool isSubCelestial = isCelestial && !frame.CelestialBodies.Any(c => c.BodyId == body.Id);

                if (isSubCelestial)
                    return 0.01;

                if (isCelestial)
                    return 0.02;

                return 0.1;
            };

            Func<Galexicon.FreeBody, Image> findBodyImage = (b) =>
            {
                return null;

                if (b is Galexicon.BodyTypes.Star)
                {
                    switch ((b as Galexicon.BodyTypes.Star).SpectralKind)
                    {
                        case Galexicon.BodyTypes.Star.SpectralType.A: return new Image { Source = TryFindResource("Star_TypeA") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.B: return new Image { Source = TryFindResource("Star_TypeB") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.C: return new Image { Source = TryFindResource("Star_TypeC") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.D: return new Image { Source = TryFindResource("Star_TypeD") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.F: return new Image { Source = TryFindResource("Star_TypeF") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.G: return new Image { Source = TryFindResource("Star_TypeG") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.K: return new Image { Source = TryFindResource("Star_TypeK") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.M: return new Image { Source = TryFindResource("Star_TypeM") as BitmapImage };
                        case Galexicon.BodyTypes.Star.SpectralType.O: return new Image { Source = TryFindResource("Star_TypeO") as BitmapImage };
                    }
                }

                return null;
            };

            Action<object, double, Point> makeBodyRep = (obj, r, p) =>
            {
                var body = obj as Galexicon.FreeBody;
                var celestial = obj as Galexicon.CelestialBody;

                if (celestial != null)
                    body = celestial.Body;

                bool isCelestial = celestial != null;
                bool isSubCelestial = isCelestial && !referenceBodies.Any(b => b.Id == celestial.FrameId);
                Button button = new Button();

                var radius = r * canvas.ActualWidth;
                radius = Math.Max(0.75, radius);
                var width = radius * 2;
                var height = radius * 2;

                Canvas.SetLeft(button, p.X * canvas.ActualWidth - radius);
                Canvas.SetTop(button, p.Y * canvas.ActualHeight - radius);

                Image img;
                if (isCelestial)
                    img = findBodyImage(celestial.Body);
                else
                    img = findBodyImage(body);

                if (img != null)
                {
                    img.Width = width;
                    img.Height = height;
                    button.Content = img;
                }
                else
                {
                    var e = new System.Windows.Shapes.Ellipse();
                    e.Width = width;
                    e.Height = height;

                    if (body is Galexicon.BodyTypes.BlackHole)
                    {
                        e.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        e.StrokeThickness = (isCelestial ? 0.0025 : 0.025) * canvas.ActualWidth;
                    }

                    if (body is Galexicon.BodyTypes.Belt)
                    {
                        e.Stroke = new SolidColorBrush(makeBodyColor(body));
                        e.StrokeThickness = canvas.ActualWidth * (body as Belt).LateralThickness.AsMeters / systemRadius;
                        e.StrokeThickness = Math.Max(1.5, e.StrokeThickness);
                        //  
                        //e.StrokeThickness = (isCelestial ? 0.0025 : 0.025) * canvas.ActualWidth;
                    }
                    else
                        e.Fill = new SolidColorBrush(makeBodyColor(body));

                    button.Content = e;
                }

                //  http://stackoverflow.com/questions/2899948/get-rid-of-button-border-in-wpf
                //  http://stackoverflow.com/a/5755599/2692629
                string template =
                    "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' TargetType =\"Button\">" +
                         "<Border>" +
                              "<ContentPresenter Content=\"{TemplateBinding Content}\"/>" +
                         "</Border>" +
                     "</ControlTemplate>";
                button.Template = (ControlTemplate)XamlReader.Parse(template);


                button.DataContext = body;

                if (isCelestial)
                    button.ContextMenu = FindResource("CelestialContextMenu") as ContextMenu;
                else
                    button.ContextMenu = FindResource("FrameContextMenu") as ContextMenu;

                Action clickHandler;

                if (isCelestial)
                    clickHandler = () => {
                        SelectBody(celestial);
                    };
                else
                    clickHandler = () => {
                        SelectBody(body);
                    };

                button.Click += (p1, p2) => { clickHandler(); };

                if (showLabels && (!isSubCelestial || showSubCelestialLabels))
                {
                    TextBlock label = new TextBlock();
                    label.DataContext = body;
                    if (isCelestial)
                        label.ContextMenu = FindResource("CelestialContextMenu") as ContextMenu;
                    else
                        label.ContextMenu = FindResource("FrameContextMenu") as ContextMenu;
                    label.TextAlignment = TextAlignment.Center;
                    label.Width = double.NaN;
                    label.Foreground = new SolidColorBrush(new Color { R = 255, G = 255, B = 255, A = 255 });
                    label.Background = null;
                    label.PreviewMouseLeftButtonUp += (o, e) => { clickHandler(); };
                    double actualPadding = 25;
                    label.Text = body.Name;
                    Canvas.SetTop(label, p.Y * canvas.ActualHeight - radius - actualPadding);
                    Canvas.SetZIndex(label, 1);
                    canvas.Children.Add(label);
                    canvas.UpdateLayout(); // Slow, but need to do this to get text size
                    Canvas.SetLeft(label, p.X * canvas.ActualWidth - label.ActualWidth * 0.5);
                }


                canvas.Children.Add(button);
            };

            Action<double> scaleCanvas = (maxRadius) =>
            {
                return;

                canvas.Width = double.NaN;
                canvas.Height = double.NaN;
                UpdateLayout();
                double defaultWidth = canvas.ActualWidth;
                double defaultHeight = canvas.ActualHeight;

                //double maxRadiusScale = 

                double systemHeightToRadius = maxRadius / systemRadius;

                double maxRatio = 0.75;
                double minRatio = 0.1;
                double relativeRatio = Math.Min(systemHeightToRadius, maxRatio);
                relativeRatio = Math.Max(relativeRatio, minRatio);

                double scale = (defaultHeight / defaultWidth) / systemHeightToRadius;
                canvas.Width = defaultWidth * scale;
            };
            #endregion

            //  Not entirely correct yet, but close

            DiagramType method;
            switch ((ViewMode.SelectedItem as ComboBoxItem).Content as String)
            {
                case "To-Scale": method = DiagramType.ToScaleDiagram; break;
                case "To-Scale Model": method = DiagramType.ToScaleModelDiagram; break;
                case "Model": method = DiagramType.ModelDiagram; break;
                default:
                    throw new InvalidOperationException();
            }

            if (method == DiagramType.ModelDiagram)
            {
                //  Not yet done
                int gridWidth, gridHeight;
                double maxRealSize, minRealSize;

                double maxRepSize;

                gridWidth = 1 + primaryCelestials.Count;
                gridHeight = 1 + primaryCelestials.Count == 0 ? 0 : primaryCelestials.Max(c => db.FrameFor(c.Body).CelestialBodies.Count);
            }

            if (method == DiagramType.ToScaleModelDiagram)
            {
                //  Determine sizings
                double refRadius = frame.Reference.Max(r => getRealRadius(r));
                double refPadding = refRadius * 0.1333;
                double refWidth = 2 * refRadius + 2 * refPadding;

                double celestialRadii = frame.CelestialBodies.Where(cb => cb.Body.Type != FreeBody.BodyType.Belt).Sum(b => getRealRadius(b.Body)) + frame.CelestialBodies.Where(cb => cb.Body.Type == FreeBody.BodyType.Belt).Sum(c => (c.Body as Belt).LateralThickness.AsMeters);
                double celestialPadding = celestialRadii * 0.333;
                double celestialWidth = 2 * celestialRadii + (frame.CelestialBodies.Count + 1) * celestialPadding;

                double spaceRatio = Math.Max(refWidth / (refWidth + celestialWidth), 0.35);
                double refWidthNorm = spaceRatio;
                double celestialWidthNorm = 1.0 - refWidthNorm;

                systemRadius = refWidth + celestialWidth;
                scaleCanvas(
                    Math.Max(
                        frame.Reference.Where(b => b.Type != FreeBody.BodyType.Belt).Max(b => getRealRadius(b)),
                        frame.CelestialBodies.Count == 0 ? 0 : frame.CelestialBodies.Select(cb => cb.Body).Where(b => b.Type != FreeBody.BodyType.Belt).Max(b => getRealRadius(b))
                    )
                );
                

                refWidth = spaceRatio * systemRadius;
                celestialWidth = celestialWidthNorm * systemRadius;


                double refYPadding = 1.0 / (frame.Reference.Count + 1);
                double refYSpacing = (1.0 - refYPadding * 2) / frame.Reference.Count;
                double y = refYPadding;

                foreach (var refBody in referenceBodies)
                {
                    makeBodyRep(refBody, getRealRadius(refBody) / systemRadius, new Point { X = refWidthNorm * 0.5, Y = y });
                    y += refYSpacing;
                }

                var focusHighlight = new System.Windows.Shapes.Rectangle();
                focusHighlight.Height = canvas.ActualHeight;
                focusHighlight.Width = canvas.ActualWidth * refWidthNorm;
                focusHighlight.Fill = new SolidColorBrush(new Color { R = 50, G = 50, B = 50, A = 255 });
                Canvas.SetTop(focusHighlight, 0);
                Canvas.SetLeft(focusHighlight, 0);
                Canvas.SetZIndex(focusHighlight, -1);
                canvas.Children.Add(focusHighlight);

                double cx = (refWidth + celestialPadding) / systemRadius;

                foreach (var celestial in primaryCelestials)
                {
                    if (celestial.Body.Type == FreeBody.BodyType.Belt)
                    {
                        var asBelt = celestial.Body as Belt;

                        cx += asBelt.LateralThickness.AsMeters / systemRadius;
                        double fakeRadius = cx - refWidthNorm * 0.5;
                        makeBodyRep(celestial, fakeRadius, new Point { X = refWidthNorm * 0.5, Y = 0.5 });
                        cx += (asBelt.LateralThickness.AsMeters + celestialPadding) / systemRadius;
                    }
                    else
                    {
                        cx += getRealRadius(celestial.Body) / systemRadius;
                        makeBodyRep(celestial, getRealRadius(celestial.Body) / systemRadius, new Point { X = cx, Y = 0.5 });

                        var celestialFrame = db.FrameFor(celestial.Body);
                        if (celestialFrame != null)
                        {
                            celestialFrame.CelestialBodies = celestialFrame.CelestialBodies.OrderBy(cb => cb.CenterDistance.AsMeters).ToList();

                            double subCelestialSpacing = 0.01;
                            double subCelestialPadding = 0.015;

                            double correctionRatio = canvas.ActualWidth / canvas.ActualHeight;

                            double scy = 0.5 + correctionRatio * getRealRadius(celestial.Body) / systemRadius + subCelestialPadding;

                            foreach (var subCelestial in celestialFrame.CelestialBodies)
                            {
                                if (subCelestial.Body.Type == FreeBody.BodyType.Belt)
                                {
                                    scy += correctionRatio * (subCelestial.Body as Belt).LateralThickness.AsMeters / systemRadius + subCelestialSpacing;
                                    makeBodyRep(subCelestial, getRealRadius(subCelestial.Body) / systemRadius, new Point { X = cx, Y = 0.5 });
                                }
                                else
                                {
                                    scy += correctionRatio * getRealRadius(subCelestial.Body) / systemRadius;
                                    makeBodyRep(subCelestial, getRealRadius(subCelestial.Body) / systemRadius, new Point { X = cx, Y = scy });
                                    scy += correctionRatio * getRealRadius(subCelestial.Body) / systemRadius + subCelestialSpacing;
                                }
                            }
                        }

                        cx += (getRealRadius(celestial.Body) + celestialPadding) / systemRadius;
                    }
                }
            }

            if (method == DiagramType.ToScaleDiagram)
            {
                if (primaryCelestials.Count > 0)
                    systemRadius = referenceBodies.Max(r => getRealRadius(r)) * 2 + primaryCelestials.Max(c => c.Body.Type == FreeBody.BodyType.Belt ? c.CenterDistance.AsMeters : c.CenterDistance.AsMeters + getRealRadius(c.Body));
                else
                    systemRadius = referenceBodies.Max(r => getRealRadius(r)) * 2;

                scaleCanvas(
                    Math.Max(
                        frame.Reference.Where(b => b.Type != FreeBody.BodyType.Belt).Max(b => getRealRadius(b)),
                        frame.CelestialBodies.Count == 0 ? 0 : frame.CelestialBodies.Select(cb => cb.Body).Where(b => b.Type != FreeBody.BodyType.Belt).Max(b => getRealRadius(b))
                ));

                double systemPadding = 0.1;
                double availableRange = 1.0 - 2 * systemPadding;

                double startX = systemPadding + availableRange * referenceBodies.Max(b => getRealRadius(b)) / systemRadius;


                double refYPadding = 1.0 / (frame.Reference.Count + 1);
                double refYSpacing = (1.0 - refYPadding * 2) / frame.Reference.Count;
                double y = refYPadding;

                foreach (var refBody in referenceBodies)
                {
                    makeBodyRep(refBody, getRealRadius(refBody) / systemRadius, new Point { X = startX, Y = y });
                    y += refYSpacing;
                }

                var focusHighlight = new System.Windows.Shapes.Rectangle();
                focusHighlight.Height = canvas.ActualHeight;
                focusHighlight.Width = canvas.ActualWidth * (startX + referenceBodies.Max(b => getRealRadius(b)) / systemRadius + 0.01);
                focusHighlight.Fill = new SolidColorBrush(new Color { R = 50, G = 50, B = 50, A = 255 });
                Canvas.SetTop(focusHighlight, 0);
                Canvas.SetLeft(focusHighlight, 0);
                Canvas.SetZIndex(focusHighlight, -3);
                canvas.Children.Add(focusHighlight);

                //double cx = (refWidth + celestialPadding) / totalWidth;

                foreach (var celestial in primaryCelestials)
                {
                    bool isBelt = celestial.Body.Type == FreeBody.BodyType.Belt;
                    Belt asBelt = celestial.Body as Belt;

                    double norm = celestial.CenterDistance.AsMeters / systemRadius;
                    double cx = startX + availableRange * norm;
                    if (isBelt)
                    {
                        makeBodyRep(celestial, getRealRadius(celestial.Body) / systemRadius, new Point { X = startX, Y = 0.5 });
                        continue;
                    }
                    else
                        makeBodyRep(celestial, getRealRadius(celestial.Body) / systemRadius, new Point { X = cx, Y = 0.5 });


                    var celestialFrame = db.FrameFor(celestial.Body);
                    if (celestialFrame != null && celestialFrame.CelestialBodies.Count > 0)
                    {
                        celestialFrame.CelestialBodies = celestialFrame.CelestialBodies.OrderBy(cb => cb.CenterDistance.AsMeters).ToList();

                        foreach (var subCelestial in celestialFrame.CelestialBodies)
                        {
                            double scy = 0.5 + (getRealRadius(celestial.Body) + subCelestial.CenterDistance.AsMeters) / systemRadius;

                            if (subCelestial.Body.Type == FreeBody.BodyType.Belt)
                                makeBodyRep(subCelestial, getRealRadius(subCelestial.Body) / systemRadius, new Point { X = cx, Y = 0.5 });
                            else
                                makeBodyRep(subCelestial, getRealRadius(subCelestial.Body) / systemRadius, new Point { X = cx, Y = scy });

                        }
                    }


                    /*
                    if (isBelt)
                        cx += (asBelt.LateralThickness.AsMeters + celestialPadding) / totalWidth;
                    else
                        cx += getRealRadius(celestial.Body) / totalWidth;
                        */
                }
            }
        }


        
        private void BodyViewer_PropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
        }
        

        private void RefreshFrame_Click(object sender, RoutedEventArgs e)
        {
            if (m_FocusedBody == null)
                return;

            BuildFrameView(db.FrameFor(m_FocusedBody));
        }

        private void ReloadGalexicon_Click(object sender, RoutedEventArgs e)
        {
            LoadGalexicon(dbPath);

            Refresh(true, true, true, true, true);
        }

        private void LoadGalexicon_Click(object sender, RoutedEventArgs e)
        {
            var fd = new System.Windows.Forms.OpenFileDialog();

            // galexicon 'defaults'
            fd.Filter = "*.gd|*.gd";

            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var galexicon = Path.GetDirectoryName(fd.FileName);
            LoadGalexicon(galexicon);
            m_CurrentDefaultsFile = fd.FileName;
            m_Defaults = Galexicon.DefaultsCollection.LoadFrom(fd.FileName);
        }

        private void BodyMenu_SetFocusClick(object sender, RoutedEventArgs e)
        {
            var body = MenuExtractBody(e);
            SetFocus(body);
        }

        private void NewRelativeAsteroid_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<Asteroid>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeBelt_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<Belt>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeBlackHole_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<BlackHole>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeComet_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<Comet>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeMegaStructure_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<MegaStructure>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeMoon_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<Moon>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativePlanet_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<Planet>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeSettlement_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<Settlement>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeStar_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<Star>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void NewRelativeWarpGate_Click(object sender, RoutedEventArgs e)
        {
            var parentBody = MenuExtractBody(e);

            var body = TryMake<WarpGate>();
            if (body == null)
                return;

            var newCelestial = new Galexicon.CelestialBody();
            newCelestial.SetBody(body);
            newCelestial.SetFrame(parentBody);
            body.CelestialContexts.Add(newCelestial);
            
            db.Save();

            Refresh(list: true, hierarchy: true, frame: true, properties: false, defaults: false);
            SelectBody(body);
        }

        private void BodyMenu_FocusParentClick(object sender, RoutedEventArgs e)
        {
            var body = MenuExtractBody(e);
            if (body.CelestialContexts.Count == 0)
            {
                MessageBox.Show("That body has no parent!");
                return;
            }

            if (body.CelestialContexts.Count == 1)
            {
                SetFocus(body.CelestialContexts.Single().Frame);
                return;
            }

            MessageBox.Show("Body has multiple parents! Focusing on first in the list!");
            SetFocus(body.CelestialContexts.First().Frame);
        }

        private void FocusParentFocus_Click(object sender, RoutedEventArgs e)
        {
            if (m_FocusedBody == null)
                return;

            if (m_FocusedBody.CelestialContexts.Count == 0)
            {
                MessageBox.Show("That body has no parent!");
                return;
            }

            if (m_FocusedBody.CelestialContexts.Count == 1)
            {
                SetFocus(m_FocusedBody.CelestialContexts.Single().Frame);
                return;
            }

            MessageBox.Show("Body has multiple parents! Focusing on first in the list!");
            SetFocus(m_FocusedBody.CelestialContexts.First().Frame);
        }

        private void AddListItemAsCelestial_Click(object sender, RoutedEventArgs e)
        {
            if (m_FocusedBody == null)
            {
                MessageBox.Show("You need to Focus a body first to add celestials to it!");
                return;
            }

            var body = MenuExtractBody(e);
            if (body.Id == m_FocusedBody.Id)
                return;

            var frame = db.FrameFor(m_FocusedBody);

            if (frame.CelestialBodies.Any(c => c.BodyId == body.Id))
            {
                MessageBox.Show("That body is already a celestial of this Focus!");
                return;
            }

            var celestial = new Galexicon.CelestialBody();
            celestial.SetBody(body);
            celestial.SetFrame(m_FocusedBody);
            body.CelestialContexts.Add(celestial);

            db.Save();

            Refresh(list: false, hierarchy: true, frame: true, properties: false, defaults: false);
        }

        private void FreeBodies_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var bodyVM = (sender as FrameworkElement).DataContext as BodyViewModel;
            SetFocus(bodyVM.Source);
        }


        private void FreeBodiesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (db == null)
                return;

            String prop = (e.OriginalSource as GridViewColumnHeader).Content as String;

            //  I'm sure there's a simpler way to do this
            var map = new Dictionary<String, String>
            {
                {  "Name", "DetailedName" },
                { "Type", "Type" },
                { "Detailed Type", "DetailedType" }

            };

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(FreeBodies.ItemsSource);

            var d = view.SortDescriptions.SingleOrDefault();
            if (d != null && d.PropertyName == map[prop])
            {
                SortDescription r = new SortDescription();
                r.PropertyName = map[prop];

                switch (d.Direction)
                {
                    case ListSortDirection.Ascending: r.Direction = ListSortDirection.Descending; break;
                    case ListSortDirection.Descending: r.Direction = ListSortDirection.Ascending; break;
                }

                view.SortDescriptions.Clear();
                view.SortDescriptions.Add(r);

                return;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(map[prop], ListSortDirection.Ascending));
            //view.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
            //view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        private void ViewMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh(list: false, hierarchy: false, frame: true, properties: false, defaults: false);
        }

        private void ShowLabelsButton_Clicked(object sender, RoutedEventArgs e)
        {
            Refresh(list: false, hierarchy: false, frame: true, properties: false, defaults: false);
        }
    }
}
