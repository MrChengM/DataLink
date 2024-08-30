using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;
using Prism.Unity;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using GuiBase.Models;
using System.Windows;
using System.Windows.Media;
using DataServer.Alarm;
using GuiBase.Services;
using Prism.Events;
using System.Linq;

namespace GuiBase.ViewModels
{
    public class AlarmLiteViewModel : BindableBase
    {
        #region Private Field
        private IAlarmService _alarmService;
        private IEventAggregator _ea;

        private ViewInfor currentBaseViewInfor;
        #endregion
        private ObservableCollection<AlarmWrapper> alarms;

        public ObservableCollection<AlarmWrapper> Alarms
        {
            get { return alarms; }
            set { SetProperty(ref alarms, value, "Alarms"); }
        }
        public AlarmLiteViewModel(IAlarmService alarmService, IEventAggregator ea)
        {
            _alarmService = alarmService;
            _alarmService.AlarmRefreshEvent += _alarmService_AlarmStatusChangeEvent;
            _ea = ea;
            _ea.GetEvent<PubSubEvent<ViewInfor>>().Subscribe(getAlarms);
        }

        private void _alarmService_AlarmStatusChangeEvent(AlarmWrapper newAlarm, AlarmRefresh status)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (matchView(newAlarm))
                {
                    if (status == AlarmRefresh.Add)
                    {
                        Alarms.Add(newAlarm);
                    }
                    else
                    {
                        var oldaAlarm = Alarms.ToList().Find(s => s.AlarmName == newAlarm.AlarmName);
                        if (status == AlarmRefresh.Updata)
                        {
                            if (oldaAlarm != null)
                            {
                                oldaAlarm.CopyFrom(newAlarm);
                            }
                            else
                            {
                                Alarms.Add(newAlarm);
                            }
                        }

                        if (status == AlarmRefresh.Remove)
                        {
                            Alarms.Remove(oldaAlarm);
                        }
                    }
                }
               
            });
        }

        private bool matchView(AlarmWrapper newAlarm)
        {
            if (currentBaseViewInfor==null)
            {
                return false;

            }
            if (currentBaseViewInfor.ViewType==ViewType.L1View)
            {
                if (newAlarm.L1View == currentBaseViewInfor.ViewName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if(currentBaseViewInfor.ViewType == ViewType.L2View)
            {
                if (newAlarm.L2View == currentBaseViewInfor.ViewName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
              return  false;
            }
        }

        private void getAlarms(ViewInfor viewInfor)
        {
            if (currentBaseViewInfor == null || currentBaseViewInfor.ViewName != viewInfor.ViewName)
            {
                currentBaseViewInfor = viewInfor;
                if (currentBaseViewInfor.ViewType== ViewType.L1View)
                {
                    Alarms = new ObservableCollection<AlarmWrapper>(filterByL1View(_alarmService.AllExitAlarms));
                }
                if (currentBaseViewInfor.ViewType == ViewType.L2View)
                {
                    Alarms = new ObservableCollection<AlarmWrapper>(filterByL2View(_alarmService.AllExitAlarms));
                }
            }
        }
        private IEnumerable<AlarmWrapper> filterByL1View(IEnumerable<AlarmWrapper> alarms)
        {
            return from s in alarms
                   where s.L1View == currentBaseViewInfor.ViewName
                   select s;

        }
        private IEnumerable<AlarmWrapper> filterByL2View(IEnumerable<AlarmWrapper> alarms)
        {
            return from s in alarms
                   where s.L2View == currentBaseViewInfor.ViewName
                   select s;
        }
    }
}
