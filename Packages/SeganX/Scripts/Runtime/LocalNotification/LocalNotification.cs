using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

using System.Collections.Generic;

namespace SeganX
{
    public class LocalNotification : MonoBehaviour
    {
        [System.Serializable]
        public class Item
        {
            [Tooltip("Seconds")]
            public int delay = 24 * 60 * 60;
            public List<string> texts = new List<string>();
        }


        private bool localPushScheduled = false;

        private void Start()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
#endif
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                ScheduleLocalPush();
            }
            else
            {
#if UNITY_ANDROID
                AndroidNotificationCenter.CancelAllNotifications();
#endif
                localPushScheduled = false;
            }
        }

        private void OnApplicationQuit()
        {
            ScheduleLocalPush();
        }

        private void ScheduleLocalPush()
        {
#if UNITY_EDITOR
            if (initialized == false)
            {
                Debug.LogError("Local notification is not been initialize!!");
            }
#endif

            if (localPushScheduled) return;
            localPushScheduled = true;

#if UNITY_ANDROID
            var c = new AndroidNotificationChannel()
            {
                Id = Application.identifier,
                Name = Application.productName,
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(c);
#endif

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.texts.Count < 0) continue;
                int index = PlayerPrefs.GetInt(name + ".Notif." + i + ".Index", 0);
                PlayerPrefs.SetInt(name + ".Notif." + i + ".Index", index + 1);
                SendNotification(item.delay, item.texts[index % item.texts.Count]);
            }

            try
            {
                OnScheduleNotification?.Invoke();
            }
            catch { }
        }

            //////////////////////////////////////////////////////
            /// STATIC MEMBERS
            //////////////////////////////////////////////////////
        private static List<Item> items = new List<Item>();

#if UNITY_EDITOR
        private static bool initialized = true;
#endif

        public static event System.Action OnScheduleNotification = null;

        public static void Initialize(List<Item> notifications)
        {
#if UNITY_EDITOR
            initialized = true;
#endif
            items = notifications;
        }

        public static void SendNotification(int delaySeconds, string text, string largIcon = "app_icon")
        {
            var parts = text.Split('|');
            if (parts.Length < 2 || parts[0].IsNullOrEmpty() || parts[1].IsNullOrEmpty()) return;
            SendNotification(delaySeconds, parts[0], parts[1], largIcon);
        }

        public static void SendNotification(int delaySeconds, string title, string message, string largIcon)
        {
#if UNITY_ANDROID
            var notification = new AndroidNotification()
            {
                Title = title,
                Text = message,
                FireTime = System.DateTime.Now.AddSeconds(delaySeconds),
                LargeIcon = largIcon
            };

            AndroidNotificationCenter.SendNotification(notification, Application.identifier);
#endif
        }

        [Console("game", "notif", "(delayInSeconds) Test local notification system")]
        public static void TestNotif(int delayInSeconds)
        {
            SendNotification(delayInSeconds, "This is a test!");
        }
    }
}