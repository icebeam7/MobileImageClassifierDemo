﻿using System;
using System.IO;

using UIKit;
using Foundation;
using Xam.Plugins.OnDeviceCustomVision;

namespace MobileImageClassifierDemo.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        readonly string model = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "coil100_model.mlmodel");

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            CopyModelToApplicationFolder();
            iOSImageClassifier.Init(model);

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        private void CopyModelToApplicationFolder()
        {
            if (!File.Exists(model))
            {
                var uncompiled = NSBundle.MainBundle.GetUrlForResource("coil100Model_CoreML", "mlmodel");
                using (var sr = File.OpenRead(uncompiled.Path))
                using (var fileStream = File.OpenWrite(model))
                    sr.CopyTo(fileStream);
            }
        }
    }
}
