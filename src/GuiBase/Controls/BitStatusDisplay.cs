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
using Utillity.Data;
using DataServer;

namespace GuiBase.Controls
{
    public class BitStatusDisplay : Control,ISignalChanged
    {
        private FrameworkElement _rootElement;
        private Shape _signalElement;
        private TextBlock _textElement;
        private ILocalizationService _localization;

        private bool isInited;
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



        public BitStatusDisplay()
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
            HasSignalElement = GetSignalElement() != null;
            HasTextElement = GetTextElement() != null;
            isInited = true;
            Dispatcher.InvokeAsync(() =>
            {
                UnRegisterSignal(Signal);
                RegisterSignal(Signal);
                setText();
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
            UnRegisterSignal(Signal);
            if (_localization!=null)
            {
                _localization.LanguageChanged -= onLanguageChanged;
            }
            isInited = false;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UnRegisterSignal(Signal);
            RegisterSignal(Signal);
            if (_localization!=null)
            {
                _localization.LanguageChanged += onLanguageChanged;
            }
        }

        private void onLanguageChanged(LanguageChangedEvent e)
        {
            setText();
        }


        //private static RadContextMenu _popup;

        public GSignal Signal
        {
            get { return (GSignal)GetValue(SignalProperty); }
            set { SetValue(SignalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Signal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SignalProperty =
            DependencyProperty.Register("Signal", typeof(GSignal), typeof(BitStatusDisplay), new PropertyMetadata(null, onSingalChange) );

        private static void onSingalChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bsd = d as BitStatusDisplay;
            var signalOld = e.OldValue as GSignal;
            var signalNew = e.NewValue as GSignal;

            bsd.UnRegisterSignal(signalOld);
            bsd.RegisterSignal(signalNew);
        }


        public int Bit
        {
            get { return (int)GetValue(BitProperty); }
            set { SetValue(BitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BitProperty =
            DependencyProperty.Register("Bit", typeof(int), typeof(BitStatusDisplay), new PropertyMetadata(0));



        public string StatusCode
        {
            get { return (string)GetValue(StatusCodeProperty); }
            set { SetValue(StatusCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusCodeProperty =
            DependencyProperty.Register("StatusCode", typeof(string), typeof(BitStatusDisplay), new PropertyMetadata(string.Empty,onStatusCodeChange));

        private static void onStatusCodeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bsd = d as BitStatusDisplay;
            //bsd.Text = e.NewValue as string;
        }

        public string StatusDiscrible
        {
            get { return (string)GetValue(StatusDiscribleProperty); }
            set { SetValue(StatusDiscribleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusDiscrible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusDiscribleProperty =
            DependencyProperty.Register("StatusDiscrible", typeof(string), typeof(BitStatusDisplay), new PropertyMetadata(string.Empty,onStatusDiscribleChange));

        private static void onStatusDiscribleChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bsd = d as BitStatusDisplay;
            //bsd.Text = e.NewValue as string;
        }

        public void OnKeepAliveSignalChanged(List<string> signalNames, bool value)
        {
            foreach (var signalName in signalNames)
            {
                if (Signal.SignalName == signalName)
                {
                    Signal.Connected = value;
                }
            }
            FireSignalChanged();
        }

        private void setText()
        {
            Text = _localization[StatusCode];
        }

        public void OnSignalChanged(GTag tag)
        {
            if (Signal.SignalName == tag.Name)
            {
                Signal.Type = tag.Type;
                Signal.Quality = tag.Quality;
                Signal.Value = tag.Value;
            }
            FireSignalChanged();
        }
        private void FireSignalChanged()
        {
            //((ToolTip)ToolTip).Content = ToolTipProvider.GetToolTip(Signals);
            //SignlaChangeEvent?.Invoke(this, new GSignalChangedEventArgs(Signals));
            if (isInited)
            {
                if (Signal.Connected && Signal.Quality == QUALITIES.QUALITY_GOOD)
                {
                    if (int.TryParse(Signal.Value, out int signalVale))
                    {
                        if (NetConvert.GetBit(signalVale, Bit))
                        {
                            SignalBrush = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            SignalBrush = new SolidColorBrush(Colors.Gray);
                        }
                    }
                }
                else
                {
                    SignalBrush = new SolidColorBrush(Colors.Black);
                }
            }
        }
        public void RegisterSignal(GSignal signal)
        {
            if (signal != null && signal.SignalName!=null)
            {
                SignalMangementHelper.Register(this, Signal);
                refreshBrush(signal);
            }
        }

        private void refreshBrush(GSignal signal)
        {
            var tag = SignalMangementHelper.GetTag(signal.SignalName) as GTag;
            if (tag != null)
            {
                OnSignalChanged(tag);
            }
        }
        public void UnRegisterSignal(GSignal signal)
        {
            if (signal != null && signal.SignalName != null)
            {
                SignalMangementHelper.UnRegister(this, Signal);

            }
        }
        
      
       
    }
}
