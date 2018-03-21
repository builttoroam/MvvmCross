// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MvvmCross;
using MvvmCross.Forms.Platforms.Android.Views;
using Playground.Core.Interfaces;
using Playground.Core.ViewModels;
using Playground.Forms.Droid.Services;

namespace Playground.Forms.Droid
{
    [Activity(
        Label = "Playground.Forms", 
        Icon = "@mipmap/icon",
        Theme = "@style/AppTheme",
        MainLauncher = true, // No Splash Screen: Uncomment this lines if removing splash screen
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, 
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : MvxFormsAppCompatActivity<MainViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);

            Mvx.RegisterSingleton<INotificationService>(() => new NotificationService(this));
        }

        public override void OnBackPressed()
        {
            MoveTaskToBack(false);
        }

        public void SendNotification(string messageTitle, string messageBody, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            if (data != null)
            {
                foreach (var key in data.Keys)
                {
                    intent.PutExtra(key, data[key]);
                }
            }

            intent.SetFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.icon)
            .SetContentTitle(messageTitle)
            .SetContentText(messageBody)
            .SetAutoCancel(true)
            .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

    }
}
