// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using Android.App;
using Android.Content;
using Android.Content.PM;
using MvvmCross.Forms.Platform.Android.Core;
using MvvmCross.Platform.Android.Core;
using MvvmCross.Platform.Android.Views;
using Playground.Forms.UI;

namespace Playground.Forms.Droid
{
    // No Splash Screen: To remove splash screen, remove this class and uncomment lines in MainActivity
    [Activity(
        Label = "Playground.Forms"
        //, MainLauncher = true
        , Icon = "@mipmap/icon"
        , Theme = "@style/AppTheme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public override MvxAndroidSetup CreateSetup(Context applicationContext)
        {
            return new Setup(applicationContext);// MvxFormsAndroidSetup<Core.App, FormsApp>(applicationContext);
        }

        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }

        protected override void TriggerFirstNavigate()
        {
            StartActivity(typeof(MainActivity));
            base.TriggerFirstNavigate();
        }
    }
}
