using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Playground.Core.Interfaces;

namespace Playground.Forms.Droid.Services
{
    public class NotificationService : INotificationService
    {
        public Activity RootActivity { get; set; }
        public NotificationService(Activity rootActivity)
        {
            RootActivity = rootActivity;
        }

        public void RaiseLocalNotification(string message)
        {
            var main = RootActivity as MainActivity;
            main.SendNotification(message, "test",null);
        }
    }
}
