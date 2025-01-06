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
using GuiBase.Services;
using Prism.Ioc;
using Utillity.Data;
using Utillity.Reflection;
using Microsoft.Xaml.Behaviors;

namespace GuiBase.Controls
{
   
    public class BitCommands : Control,ISignalChanged
    {
        private FrameworkElement _rootElement;
        private TextBlock _text;
        private Panel _buttonPanel;
        private BaseElement _statusElement;
        private ILocalizationService _localization;

        private readonly SolidColorBrush btnNoConState = App.Current.Resources["COLOR_BRUSH_CommandBtnNoConState"] as SolidColorBrush;
        private readonly SolidColorBrush btnSetState = App.Current.Resources["COLOR_BRUSH_CommandBtnSetState"] as SolidColorBrush;
        private readonly SolidColorBrush btnNormalState = App.Current.Resources["btnSetState"] as SolidColorBrush;
        private readonly SolidColorBrush btnNoConForeground= App.Current.Resources["COLOR_BRUSH_CommandBtnNoConStateFore"] as SolidColorBrush;
        private readonly SolidColorBrush btnSetForeground = App.Current.Resources["COLOR_BRUSH_CommandBtnSetStateFore"] as SolidColorBrush;
        private readonly SolidColorBrush btnNormalForeground = App.Current.Resources["COLOR_BRUSH_CommandBtnNormalStateFore"] as SolidColorBrush;
        private readonly Style btnStyle = App.Current.Resources["BitCommandButton"] as Style;

        public FrameworkElement RootElement
        {
            get
            {
                return _rootElement;
            }
        }
        public BitCommands()
        {
            //SetValue(SignalsProperty, new GSignalSet());
            _localization= ContainerLocator.Container?.Resolve<ILocalizationService>();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootElement = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
            _text = GetTextElement();
            _buttonPanel = GetPanelElement();
            _statusElement = GetElement();
            if (_text!=null)
            {
                _text.Text = ElementName;

            }
            if (_statusElement != null)
            {
                _statusElement.ElementName = ElementName;
                _statusElement.Signals = Signals;
                try
                {
                    Behavior behStatus = (Behavior)ReflectionFunction.CreateInstance(Behavior);
                    if (behStatus != null)
                    {
                        Interaction.GetBehaviors(_statusElement).Add(behStatus);
                    }
                }
                catch (Exception e)
                {

                    _statusElement.Visibility = Visibility.Hidden;
                }
              
            }
            Dispatcher.InvokeAsync(() =>
            {
                ResisterCommands();
                UnRegistCommandSignals(Commands);
                RegisterCommandSignals(Commands);
            }
          );
            RebuidButtons(Commands);
        }
        public TextBlock GetTextElement()
        {
            if (_text == null)
            {
                _text = GetTextElement("Texts");
            }
            return _text;
        }
        public TextBlock GetTextElement(string name)
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
        public Panel GetPanelElement()
        {

            if (_buttonPanel == null)
            {
                _buttonPanel = GetStylePanel("Panel");
            }
            else
            {
                _buttonPanel = new StackPanel() { Orientation= Orientation.Horizontal};
            }
            return _buttonPanel;
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
        public BaseElement GetElement()
        {

            if (_statusElement == null)
            {
                _statusElement = GetStyleElement("Element");
            }
            return _statusElement;
        }
        public BaseElement GetStyleElement(string name)
        {
            BaseElement result;
            if (_rootElement != null)
            {
                result = _rootElement.FindName(name) as BaseElement;
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
            //UnRegistCommandSignals();
            if (_localization != null)
            {
                _localization.LanguageChanged -= onLanguageChanged;

            }
            UnRegistCommandSignals(Commands);
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UnRegistCommandSignals(Commands);
            RegisterCommandSignals(Commands);
            if (_localization != null)
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
        public GSignalSet Signals
        {
            get { return (GSignalSet)GetValue(SignalsProperty); }
            set { SetValue(SignalsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SignalsProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SignalsProperty =
            DependencyProperty.Register("Signals", typeof(GSignalSet), typeof(BitCommands), new PropertyMetadata(new GSignalSet()));

        public string ElementName
        {
            get { return (string)GetValue(ElementNameProperty); }
            set { SetValue(ElementNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElementName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementNameProperty =
            DependencyProperty.Register("ElementName", typeof(string), typeof(BitCommands), new PropertyMetadata(string.Empty));


        public string Behavior
        {
            get { return (string)GetValue(BehaviorProperty); }
            set { SetValue(BehaviorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Behavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.Register("Behavior", typeof(string), typeof(BitCommands), new PropertyMetadata(string.Empty));


        public string CommandSignalName
        {
            get { return (string)GetValue(CommandSignalNameProperty); }
            set { SetValue(CommandSignalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandSignalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandSignalNameProperty =
            DependencyProperty.Register("CommandSignalName", typeof(string), typeof(BitCommands), new PropertyMetadata(string.Empty));
        public string CommandType
        {
            get { return (string)GetValue(CommandTypeProperty); }
            set { SetValue(CommandTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTypeProperty =
            DependencyProperty.Register("CommandType", typeof(string), typeof(BitCommands), new PropertyMetadata(string.Empty));
        public List<IGCommand> Commands
        {
            get { return (List<IGCommand>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Commands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.Register("Commands", typeof(List<IGCommand>), typeof(BitCommands), new PropertyMetadata(new List<IGCommand>(),onCommandsChange));

        private static void onCommandsChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var commandC = d as BitCommands;
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
                            if (tag.Quality == DataServer.QUALITIES.QUALITY_GOOD)
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
        public void RegisterSignals(GSignalSet signals)
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
            if (_buttonPanel == null || commands == null)
            {
                return;
            }
            _buttonPanel.Children.Clear();
            Buttons.Clear();
            foreach (var command in commands)
            {
                var btn = getButton(command);
                _buttonPanel.Children.Add(btn);
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
                btn.MinHeight = 20;
                //btn.MinWidth = 80;
                btn.Margin = new Thickness(2);
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
