using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

using System;

namespace HeroFishing.Main {
    public class LocoNotification : MonoBehaviour {
        public static LocoNotification Instance { get; private set; }
#if UNITY_ANDROID
        const string channelID = "RoleCall";
        List<int> NotificationIDs = new List<int>();

#endif
        public virtual void Init() {
#if UNITY_ANDROID
            AndroidNotificationCenter.Initialize();
            RegisterChannel_Android(channelID, "角色來電通知");
#elif UNITY_IOS
#endif
            Instance = this;
        }





#if UNITY_ANDROID
        void RegisterChannel_Android(string _channelID, string _channelName) {
            var channel = new AndroidNotificationChannel() {
                Id = _channelID,
                Name = _channelName,
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        int SendNotification_Android(string _channel, string _title, string _text, Color _color, DateTime _time) {
            var notification = new AndroidNotification();
            notification.Title = _title;
            notification.Text = _text;
            notification.FireTime = _time;
            notification.LargeIcon = "logo";
            notification.Color = _color;
            return AndroidNotificationCenter.SendNotification(notification, _channel);
        }
        NotificationStatus GetNotificationState_Andoird(int _id) {
            var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(_id);
            return notificationStatus;
        }
        public void CancelAllNotifications_Android() {
            AndroidNotificationCenter.CancelAllNotifications();
        }
#elif UNITY_IOS
        void SendNotification_IOS(string _category, string _title, string _text, DateTime _time) {
            var calendarTrigger = new iOSNotificationCalendarTrigger() {
                Year = _time.Year,
                Month = _time.Month,
                Day = _time.Day,
                Hour = _time.Hour,
                Minute = _time.Minute,
                Second = _time.Second,
                Repeats = false
            };


            var notification = new iOSNotification() {
                Title = _title,
                Body = _text,
                Subtitle = "",
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Badge | PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = _category,
                ThreadIdentifier = _category + "Thread",
                Trigger = calendarTrigger,
            };
            iOSNotificationCenter.ScheduleNotification(notification);
        }



#endif




    }
}