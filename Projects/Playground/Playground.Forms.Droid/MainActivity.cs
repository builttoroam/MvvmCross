// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MvvmCross;
using MvvmCross.Forms.Platform.Android.Core;
using MvvmCross.Forms.Platform.Android.Views;
using MvvmCross.Platform.Android.Core;
using MvvmCross.ViewModels;
using Playground.Core.ViewModels;
using Playground.Forms.UI;

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
        public override MvxAndroidSetup CreateSetup(Context applicationContext)
        {
            return new Setup(applicationContext);// MvxFormsAndroidSetup<Core.App, FormsApp>(applicationContext);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);

            // No Splash Screen: Uncomment these lines if removing splash screen
            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();
            InitializeForms(bundle);
        }

        public override void OnBackPressed()
        {
            MoveTaskToBack(false);
        }
    }
}
