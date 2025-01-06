using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GuiBase.Common;
using GuiBase.Models;
using GuiBase.Helper;
using GuiBase.Services;
using Prism.Services.Dialogs;
using GuiBase.Behaviors;
using Prism.Ioc;

namespace GuiBase.Controls
{
    public class BaseElement : Control,ISignalChanged
    {
        private FrameworkElement _rootElement;
        //private ToolTip _tooltip;
        private PopupMenu _popupMenu;
        private Shape _signalElement;
        private TextBlock _textElement;
        public ILocalizationService Localization { get; private set; }
        //private readonly SolidColorBrush scadaBodyBackground= App.Current.Resources["COLOR_BRUSH_ScadaBodyBackground"] as SolidColorBrush;
        //private readonly SolidColorBrush buttonForecground= App.Current.Resources["COLOR_BRUSH_ButtonForeground"] as SolidColorBrush;
        private readonly Style menuItemStyle = App.Current.Resources["BaseMenuItemWithIcon"] as Style;
        public IToolTiProvider ToolTipProvider { get; set; }

        public bool HasSignalElement { get; private set; }
        public bool HasTextElement { get; private set; }
        public FrameworkElement RootElement
        {
            get
            {
                return _rootElement;
            }
        }
        public Brush SignalBrush
        {
            get { return GetSignalElement().Fill; }
            set { GetSignalElement().Fill = value; }
        }


        public string Text
        {
            get { return GetTextElement().Text; }
            set { GetTextElement().Text = value; }
        }



        public event EventHandler<GSignalChangedEventArgs> SignlaChangeEvent;
        public BaseElement()
        {

            //SetValue(SignalsProperty, new GSignalSet());
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            ToolTip = new ToolTip();
            Localization = ContainerLocator.Container.Resolve<ILocalizationService>();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootElement = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
            HasSignalElement = GetSignalElement() != null;
            HasTextElement = GetTextElement() != null;
            Dispatcher.InvokeAsync(() =>
            {
                UnRegistSignals(Signals);
                RegisterSignals(Signals);
                ResisterCommands();
            }
          );
        }

        public TextBlock GetTextElement()
        {

            if (_textElement == null)
            {
                _textElement = GetStyleText("Texts");
            }
            return _textElement;
        }

        public Shape GetSignalElement()
        {
            if (_signalElement == null)
            {
                _signalElement = GetStyleShape("Signal");
            }
            return _signalElement;
        }
       

        public Shape GetStyleShape(string name)
        {
            Shape result;
            if (_rootElement != null)
            {
                result = _rootElement.FindName(name) as Shape;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public TextBlock GetStyleText(string name)
        {
            TextBlock result;
            if (_rootElement != null)
            {
                result = _rootElement.FindName(name) as TextBlock;
            }
            else
            {
                result = null;
            }
            return result;
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UnRegistSignals(Signals);
            Localization.LanguageChanged -= onLanguageChanged;
            ((ToolTip)ToolTip).Opened -= new RoutedEventHandler(OnShowToolTip);

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UnRegistSignals(Signals);
            RegisterSignals(Signals);
            //SignalMangementHelper.Register(this, Signals);
            Localization.LanguageChanged += onLanguageChanged;
           ((ToolTip)ToolTip).Opened += new RoutedEventHandler(OnShowToolTip);
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            FireSignalChanged();
        }

        private void OnShowToolTip(object sender, RoutedEventArgs e)
        {
            updataToolTip();
        }

        private void updataToolTip()
        {
            ((ToolTip)ToolTip).Content = ToolTipProvider.GetToolTip(Signals);
        }

        //private static RadContextMenu _popup;




        /// <summary>
        /// 元素或设备名称
        /// </summary>
        public string ElementName
        {
            get { return (string)GetValue(ElementNameProperty); }
            set { SetValue(ElementNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElementName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementNameProperty =
            DependencyProperty.Register("ElementName", typeof(string), typeof(BaseElement), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 元素或设备类型描述
        /// </summary>
        public string TypeDescription
        {
            get { return (string)GetValue(TypeDescriptionProperty); }
            set { SetValue(TypeDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeDescriptionProperty =
            DependencyProperty.Register("TypeDescription", typeof(string), typeof(BaseElement), new PropertyMetadata(string.Empty));


        /// <summary>
        /// 导航画面名称
        /// </summary>
        public string NavigationViewName
        {
            get { return (string)GetValue(NavigationViewNameProperty); }
            set { SetValue(NavigationViewNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationViewName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationViewNameProperty =
            DependencyProperty.Register("NavigationViewName", typeof(string), typeof(BaseElement), new PropertyMetadata(string.Empty));




        //public ViewType NavigationViewType
        //{
        //    get { return (ViewType)GetValue(NavigationViewTypeProperty); }
        //    set { SetValue(NavigationViewTypeProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for NavigationViewType.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty NavigationViewTypeProperty =
        //    DependencyProperty.Register("NavigationViewType", typeof(ViewType), typeof(BaseElement), new PropertyMetadata(ViewType.Base_L2View));



        public string L3ViewName
        {
            get { return (string)GetValue(L3ViewNameProperty); }
            set { SetValue(L3ViewNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for L3ViewName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty L3ViewNameProperty =
            DependencyProperty.Register("L3ViewName", typeof(string), typeof(BaseElement), new PropertyMetadata(string.Empty));



        public ViewType ScadaLevel
        {
            get { return (ViewType)GetValue(ScadaLevelProperty); }
            set { SetValue(ScadaLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScadaLevelProperty =
            DependencyProperty.Register("ScadaLevel", typeof(ViewType), typeof(BaseElement), new PropertyMetadata(ViewType.Base_L2View));



        public ClickMode ChooseLeftClick
        {
            get { return (ClickMode)GetValue(ChooseLeftClickProperty); }
            set { SetValue(ChooseLeftClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChooseLeftClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChooseLeftClickProperty =
            DependencyProperty.Register("ChooseLeftClick", typeof(ClickMode), typeof(BaseElement), new PropertyMetadata(ClickMode.NoAction));


        public bool HasRightClickMenu
        {
            get { return (bool)GetValue(HasRightClickMenuProperty); }
            set { SetValue(HasRightClickMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasRightClickMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasRightClickMenuProperty =
            DependencyProperty.Register("HasRightClickMenu", typeof(bool), typeof(BaseElement), new PropertyMetadata(false));

        public GSignalSet Signals
        {
            get { return (GSignalSet)GetValue(SignalsProperty); }
            set { SetValue(SignalsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Signals.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SignalsProperty =
            DependencyProperty.Register("Signals", typeof(GSignalSet), typeof(BaseElement), new PropertyMetadata(new GSignalSet(),SignalSetChange));



        public string CommandSignalName
        {
            get { return (string)GetValue(CommandSignalNameProperty); }
            set { SetValue(CommandSignalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandSignalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandSignalNameProperty =
            DependencyProperty.Register("CommandSignalName", typeof(string), typeof(BaseElement), new PropertyMetadata(string.Empty));
        public string CommandType
        {
            get { return (string)GetValue(CommandTypeProperty); }
            set { SetValue(CommandTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTypeProperty =
            DependencyProperty.Register("CommandType", typeof(string), typeof(BaseElement), new PropertyMetadata(string.Empty));



        public List<IGCommand> Commands
        {
            get { return (List<IGCommand>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Commands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.Register("Commands", typeof(List<IGCommand>), typeof(BaseElement), new PropertyMetadata(default(List<IGCommand>)));


        private static void SignalSetChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as BaseElement;
            SignalMangementHelper.UnRegister(element, (GSignalSet)e.OldValue);
            SignalMangementHelper.Register(element, (GSignalSet)e.NewValue);

        }

        public void OnKeepAliveSignalChanged(List<string> signalNames, bool value)
        {
            foreach (var signalName in signalNames)
            {
                var signal = Signals.ToList().Find(s => s.SignalName == signalName);
                if (signal != null)
                {
                    signal.Connected = value;
                }
            }
            FireSignalChanged();
        }
        public void OnSignalChanged(GTag tag)
        {

            var signal = Signals.ToList().Find(s => s.SignalName == tag.Name);
            if (signal != null)
            {
                signal.Type = tag.Type;
                signal.Quality = tag.Quality;
                signal.Value = tag.Value;
            }
            FireSignalChanged();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Cursor = Cursors.Hand;

            if (ChooseLeftClick == ClickMode.Navigation)
            {
                ViewOperateHelper.NavigationBaseRegion( NavigationViewName);
            }
            else if (ChooseLeftClick == ClickMode.Level3Open)
            {
                var param = new DialogParameters();

                param.Add("L3ViewName", L3ViewName);
                param.Add("ElementName", ElementName);
                param.Add("Signals", Signals);
                param.Add("Commands", Commands);
                ViewOperateHelper.ShowL3View(param);
            }
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (HasRightClickMenu)
            {
                rebuildContextMenu();
                _popupMenu.IsOpen = true;
            }
        }
        private void FireSignalChanged()
        {
            if (ToolTipProvider != null)
            {
                ((ToolTip)ToolTip).Content = ToolTipProvider.GetToolTip(Signals);
                SignlaChangeEvent?.Invoke(this, new GSignalChangedEventArgs(Signals));
            }

        }
        public void RegisterSignals(GSignalSet signals )
        {
            if (signals != null && signals.Count > 0)
            {
                SignalMangementHelper.Register(this, signals);

            }
        }
        public void UnRegistSignals(GSignalSet signals)
        {
            if (signals != null && signals.Count > 0)
            {
                SignalMangementHelper.UnRegister(this, signals);

            }
        }
        
        public void ResisterCommands()
        {
            if (CommandSignalName != null && CommandType != null)
            {
                var commandSet = ContainerLocator.Container.Resolve<IGCommandSet>();
                commandSet.RegisterCommand(CommandType, CommandSignalName);
                if (Commands == null)
                {
                    Commands = commandSet.GetGCommands(CommandType, CommandSignalName);
                }
            }
           
        }

        private void rebuildContextMenu()
        {
            _popupMenu = new PopupMenu();
            _popupMenu.PlacementTarget = this;
            _popupMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            //_popupMenu.Background = 
            //_popupMenu.Foreground = ;
            //_popupMenu.MinWidth = 150;
            //_popupMenu.MinHeight = 300;

            if (Commands == null)
            {
                Commands = ContainerLocator.Container.Resolve<IGCommandSet>().GetGCommands(CommandType, CommandSignalName);
            }
            foreach (var command in Commands)
            {
              var menuItem=  _popupMenu.AddMenuItem(command.Name, command.Command, null);
                //menuItem.FontSize = 16;
                //menuItem.IsEnabled = command.HasPermission;
                //menuItem.Background = scadaBodyBackground;
                //menuItem.Foreground = buttonForecground;
                //menuItem.Margin = new Thickness(2);
                menuItem.Style = menuItemStyle;
                //menuItem.BorderBrush = new SolidColorBrush(Colors.Black);
                //menuItem.BorderThickness = new Thickness(1);
                //var sp = _popupMenu.AddMenuSeparatorItem();
                //sp.Margin = new Thickness(2);
                //sp.Height = 2;
                //sp.Background = scadaBodyBackground;
                //sp.Foreground = new SolidColorBrush(Colors.Black); ;
            }

        }
    }
    public enum ClickMode
    {
        Level3Open,
        Navigation,
        NoAction
    }
}
