using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;
using GuiBase.Controls;
using GuiBase.Helper;
using GuiBase.Models;
using System.Windows;
using GuiBase.Common;
using DataServer;
using System.Windows.Media;

namespace GuiBase.Behaviors
{
    public abstract class BaseElementDefaultBahavior : Behavior<BaseElement>, IToolTiProvider
    {
        private bool _initiated;
        private bool _attached;
        private static Brush UnknownSignalColor = Application.Current.Resources["COLOR_BRUSH_Unknown"] as SolidColorBrush;
        private static Brush NotConnectColor = Application.Current.Resources["COLOR_BRUSH_NoConState"] as SolidColorBrush;
        private static Brush BadQualityColor = Application.Current.Resources["COLOR_NotParsable"] as SolidColorBrush;
        private static string UnknownText = "Unknown";
        private static string NotConnectText = "NotConnect";
        private static string BadQualityText = "BadQuality";
        private static string DeviceNameText = "Device Name";
        private static string DeviceTypeText = "Device Type";
        private static string DeviceStatusText = "Device Status";

        public BaseElement Element
        {
            get
            {
                return AssociatedObject;
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _initiated = true;
            OnAttached();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            OnDetaching();
        }

        /// <summary>
        /// What to do on attaching the behavior
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (!_initiated)
            {
                Element.Loaded += new RoutedEventHandler(OnLoaded);
                Element.Unloaded += new RoutedEventHandler(OnUnloaded);
                Element.ToolTipProvider = this;
            }
            if (!_attached)
            {
                _attached = true;
                Element.SignlaChangeEvent += OnSignalChanged;

                //Element.UnRegistSignals();
                //Element.RegisterSignals();
            }
        }

        private void getTextTraslate()
        {
            UnknownText = Element.Localization[TranslateCommonId.UnknownId];
            BadQualityText = Element.Localization[TranslateCommonId.BadQualityId];
            NotConnectText = Element.Localization[TranslateCommonId.NotConnectId];
            DeviceNameText = Element.Localization[TranslateCommonId.DeviceNameId];
            DeviceTypeText = Element.Localization[TranslateCommonId.DeviceTypeId];
            DeviceStatusText = Element.Localization[TranslateCommonId.DeviceStatusId];

        }
        public virtual void OnSignalChanged(object sender, GSignalChangedEventArgs e)
        {
            getTextTraslate();
            if (!IsSignalsKnown(e))
            {
                if (Element.HasSignalElement)
                {
                    Element.SignalBrush = UnknownSignalColor;
                }
                if (Element.HasTextElement)
                {
                    Element.Text = UnknownText;
                }
            }
            else
            {

                if (!IsSignalsInitialized(e))
                {
                    if (Element.HasSignalElement)
                    {
                        Element.SignalBrush = NotConnectColor;
                    }
                    if (Element.HasTextElement)
                    {
                        Element.Text = NotConnectText;
                    }
                }
                else
                {
                    if (!IsConnected(e))
                    {
                        if (Element.HasSignalElement)
                        {
                            Element.SignalBrush = NotConnectColor;
                        }
                        if (Element.HasTextElement)
                        {
                            Element.Text = NotConnectText;
                        }
                    }
                    else
                    {
                        if (!IsSignalsOfGoodQuality(e))
                        {
                            if (Element.HasSignalElement)
                            {
                                Element.SignalBrush = BadQualityColor;
                            }
                            if (Element.HasTextElement)
                            {
                                Element.Text = BadQualityText;
                            }
                        }
                        else
                        {
                            OnSignalChanged(e);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Action on detaching the Behavior
        /// </summary>
        protected override void OnDetaching()
        {
            _attached = false;
            Element.SignlaChangeEvent -= OnSignalChanged;
            base.OnDetaching();
        }

        public virtual object GetToolTip(GSignalSet signals)
        {
            StringBuilder toolTip = new StringBuilder();
            GSignalChangedEventArgs e = new GSignalChangedEventArgs(signals);
            getTextTraslate();
            toolTip.Append(DeviceNameText);
            toolTip.Append(" : ");

            if (Element.ElementName == null)
            {
                toolTip.Append(UnknownText);
            }
            else
            {
                toolTip.Append(Element.ElementName);
            }
            toolTip.AppendLine(); 
            toolTip.AppendLine();
            toolTip.Append(DeviceTypeText);
            toolTip.Append(" : ");
            if (Element.TypeDescription==null)
            {
                toolTip.Append(UnknownText);
            }
            else
            {
                toolTip.Append(Element.TypeDescription);
            }
            toolTip.AppendLine();
            toolTip.AppendLine();

            toolTip.Append(DeviceStatusText);
            toolTip.Append(" : ");
            if (!IsSignalsKnown(e))
            {
                toolTip.Append(UnknownText);
                toolTip.Append(";");

            }
            else
            {

                if (!IsSignalsInitialized(e))
                {
                    toolTip.Append(NotConnectText);
                    toolTip.Append(";");
                }
                else
                {
                    if (!IsConnected(e))
                    {
                        toolTip.Append(NotConnectText);
                        toolTip.Append(";");
                    }
                    else
                    {
                        if (!IsSignalsOfGoodQuality(e))
                        {
                            toolTip.Append(BadQualityText);
                            toolTip.Append(";");
                        }
                        else
                        {
                            toolTip.Append(GetToolTip(e));
                        }
                    }
                }
            }
            return toolTip.ToString();
        }

        public abstract void OnSignalChanged(GSignalChangedEventArgs e);
        public abstract string GetToolTip(GSignalChangedEventArgs e);
        public virtual bool IsSignalsKnown(GSignalChangedEventArgs e)
        {
            return e.Signals != null && e.Signals.Count > 0;
        }
        public virtual bool IsSignalsInitialized(GSignalChangedEventArgs e)
        {
            return e.Signals.All(s => s.Initialized);
        }
        public virtual bool IsConnected(GSignalChangedEventArgs e)
        {
            return e.IsConnected;
        }
        public virtual bool IsSignalsOfGoodQuality(GSignalChangedEventArgs e)
        {
            return e.Signals.All(s => s.Quality >= QUALITIES.QUALITY_GOOD);
        }

        protected virtual Brush GetBrush(string codeName)
        {
            string key = $"COLOR_BRUSH_{codeName}";
            if (Application.Current.Resources.Contains(key))
            {
                return Application.Current.Resources[key] as SolidColorBrush;
            }
            else
            {
                return null;
            }
        }


        protected virtual string GetStatus(string codeName)
        {
            return Element.Localization[codeName];

            //string key = $"TEXT_STRING_{codeName}";
            //if (Application.Current.Resources.Contains(key))
            //{
            //    return Application.Current.Resources[key] as string;
            //}
            //else
            //{
            //    return null;
            //}
        }
    }
}
