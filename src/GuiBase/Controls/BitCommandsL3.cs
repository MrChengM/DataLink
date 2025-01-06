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
using Prism.Services.Dialogs;
using GuiBase.Behaviors;
using Prism.Ioc;
using Utillity.Data;
using GuiBase.Services;

namespace GuiBase.Controls
{
   
    public class BitCommandsL3 : Control,ISignalChanged
    {
        private FrameworkElement _rootElement;
        //private ToolTip _tooltip;
        private Panel _panel;
        private ILocalizationService _localization;

        private readonly SolidColorBrush btnNoConState = App.Current.Resources["COLOR_BRUSH_CommandBtnNoConState"] as SolidColorBrush;
        private readonly SolidColorBrush btnSetState = App.Current.Resources["COLOR_BRUSH_CommandBtnSetState"] as SolidColorBrush;
        private readonly SolidColorBrush btnNormalState = App.Current.Resources["btnSetState"] as SolidColorBrush;
        private readonly SolidColorBrush btnNoConForeground= App.Current.Resources["COLOR_BRUSH_CommandBtnNoConStateFore"] as SolidColorBrush;
        private readonly SolidColorBrush btnSetForeground = App.Current.Resources["COLOR_BRUSH_CommandBtnSetStateFore"] as SolidColorBrush;
        private readonly SolidColorBrush btnNormalForeground = App.Current.Resources["COLOR_BRUSH_CommandBtnNormalStateFore"] as SolidColorBrush;
        private readonly Style btnStyle = App.Current.Resources["BitCommandButtonL3"] as Style;

        public FrameworkElement RootElement
        {
            get
            {
                return _rootElement;
            }
        }
        public BitCommandsL3()
        {
            //SetValue(SignalsProperty, new GSignalSet());
            _localization = ContainerLocator.Container?.Resolve<ILocalizationService>();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootElement = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
            _panel = GetPanelElement();
            Dispatcher.InvokeAsync(() =>
            {

                UnRegistCommandSignals();
                RegisterCommandSignals();
                ResisterCommands();
            }
          );
            RebuidButtons(Commands);
        }

        public Panel GetPanelElement()
        {

            if (_panel == null)
            {
                _panel = GetStylePanel("Panel");
            }
            else
            {
                _panel = new StackPanel() { Orientation= Orientation.Horizontal};
            }
            return _panel;
        }
        public Panel GetStylePanel(string name)
        {
            Panel result;
            if (_rootElement != null)
            {
                result = _rootElement.FindName(name) as Panel;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public Dictionary<IGCommand, Button> Buttons = new Dictionary<IGCommand, Button>();
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UnRegistCommandSignals();
            UnRegistCommandSignals(Commands);

            if (_localization != null)
            {
                _localization.LanguageChanged -= onLanguageChanged;
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UnRegistCommandSignals();
            UnRegistCommandSignals(Commands);
            RegisterCommandSignals();
            //防止预编译出错
            if (_localization!=null)
            {
                _localization.LanguageChanged += onLanguageChanged;
            }
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            refreshBtnContent();
        }

        private void refreshBtnContent()
        {
            foreach (var btn in Buttons)
            {
                btn.Value.Content = _localization[btn.Key.TranslationId];
            }
        }

        public string CommandSignalName
        {
            get { return (string)GetValue(CommandSignalNameProperty); }
            set { SetValue(CommandSignalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandSignalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandSignalNameProperty =
            DependencyProperty.Register("CommandSignalName", typeof(string), typeof(BitCommandsL3), new PropertyMetadata(string.Empty));
        public string CommandType
        {
            get { return (string)GetValue(CommandTypeProperty); }
            set { SetValue(CommandTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTypeProperty =
            DependencyProperty.Register("CommandType", typeof(string), typeof(BitCommandsL3), new PropertyMetadata(string.Empty));
        public List<IGCommand> Commands
        {
            get { return (List<IGCommand>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Commands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.Register("Commands", typeof(List<IGCommand>), typeof(BitCommandsL3), new PropertyMetadata(new List<IGCommand>(),onCommandsChange));

        private static void onCommandsChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var commandC = d as BitCommandsL3;
            var commandsOld = e.OldValue as List<IGCommand>;
            var commandsNew = e.NewValue as List<IGCommand>;

            commandC.UnRegistCommandSignals(commandsOld);
            commandC.RegisterCommandSignals(commandsNew);
            commandC.RebuidButtons(commandsNew);

        }

        public void OnKeepAliveSignalChanged(List<string> signalNames, bool value)
        {
        }
        public void OnSignalChanged(GTag tag)
        {
            if (int.TryParse(tag.Value, out int tagValue))
            {
                foreach (var btn in Buttons)
                {
                    if (btn.Key is BitGCommand bc)
                    {
                        if (bc.SignalName == tag.Name)
                        {
                            if (tag.Quality==DataServer.QUALITIES.QUALITY_GOOD)
                            {
                                if (NetConvert.GetBit(tagValue, bc.Bit) == bc.Value)
                                {
                                    btn.Value.Foreground = btnSetForeground;
                                    btn.Value.Background = btnSetState;
                                }
                                else
                                {
                                    btn.Value.Foreground = btnNormalForeground;
                                    btn.Value.Background = btnNormalState;
                                }
                            }
                            else
                            {
                                btn.Value.Foreground = btnNoConForeground;
                                btn.Value.Background = btnNoConState;
                            }

                        }
                    }
                }
            }
          
        }
        public void RegisterCommandSignals()
        {
            if (CommandSignalName != null && CommandSignalName != string.Empty)
            {
                SignalMangementHelper.Register(this, CommandSignalName);

            }
        }
        public void RegisterCommandSignals(List<IGCommand> commands )
        {
            string signalName = null;
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    if (command is BitGCommand bc)
                    {
                        if (signalName == null || signalName != bc.SignalName)
                        {
                            signalName = bc.SignalName;
                            SignalMangementHelper.Register(this, signalName);
                        }
                    }

                }
            }

        }
        public void UnRegistCommandSignals()
        {
            if (CommandSignalName != null&&CommandSignalName!= string.Empty)
            {
                SignalMangementHelper.UnRegister(this, CommandSignalName);
            }
        }
        public void UnRegistCommandSignals(List<IGCommand> commands)
        {
            string signalName = null;
            if (commands!=null)
            {
                foreach (var command in commands)
                {
                    if (command is BitGCommand bc)
                    {
                        if (signalName != null && signalName != bc.SignalName)
                        {
                            signalName = bc.SignalName;
                            SignalMangementHelper.UnRegister(this, signalName);
                        }
                    }

                }
            }
         
        }
        public void ResisterCommands()
        {
            if (CommandSignalName != null && CommandType != null)
            {
                var commandSet = ContainerLocator.Container.Resolve<IGCommandSet>();
                commandSet.RegisterCommand(CommandType, CommandSignalName);
                if (Commands == null || Commands.Count == 0)
                {
                    Commands = commandSet.GetGCommands(CommandType, CommandSignalName);
                }
            }
        }

        public  void RebuidButtons(List<IGCommand> commands )
        {
            if (_panel == null || commands == null)
            {
                return;
            }
            _panel.Children.Clear();
            Buttons.Clear();
            foreach (var command in commands)
            {
                var btn = getButton(command);
                _panel.Children.Add(btn);
                Buttons.Add(command, btn);
                
            }
        }
        private Button getButton(IGCommand gCommand)
        {
            var btn = new Button
            {
                IsEnabled = gCommand.HasPermission,
                Content = _localization[gCommand.TranslationId],
                Command = gCommand.Command
            };
            if (btnStyle != null)
            {
                btn.Style = btnStyle;
            }
            else
            {
                btn.MinHeight = 25;
                btn.MinWidth = 80;
                btn.Margin = new Thickness(20, 10, 20, 10);
                btn.HorizontalAlignment = HorizontalAlignment.Center;
                btn.VerticalAlignment = VerticalAlignment.Center;
            }

            if (gCommand is BitGCommand bc)
            {
                var tag = SignalMangementHelper.GetTag(bc.SignalName);
                if (tag.Quality == DataServer.QUALITIES.QUALITY_GOOD)
                {
                    if (int.TryParse(tag.Value, out int tagValue))
                    {
                        if (NetConvert.GetBit(tagValue, bc.Bit) == bc.Value)
                        {
                            btn.Foreground = btnSetForeground;
                            btn.Background = btnSetState;
                        }
                        else
                        {
                            btn.Foreground = btnNormalForeground;
                            btn.Background = btnNormalState;
                        }
                    }
                    else
                    {
                        btn.Foreground = btnNoConForeground;
                        btn.Background = btnNoConState;
                    }
                }
                else
                {
                    btn.Foreground = btnNoConForeground;
                    btn.Background = btnNoConState;
                }
            }
            return btn;
        }
    }
}
