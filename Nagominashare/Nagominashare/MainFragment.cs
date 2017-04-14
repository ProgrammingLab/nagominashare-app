using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Nagominashare {
    class MainFragment : Fragment {
        public event EventHandler ClickMonitorToggleButton;
        public event EventHandler ClickGenerateButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.Main, null, false);
            var monitorToggleButton = view.FindViewById<ImageButton>(Resource.Id.monitor_toggle_button);
            monitorToggleButton.Click +=
                (sender, args) => ClickMonitorToggleButton?.Invoke(sender, args);
            var generateButton = view.FindViewById<ImageButton>(Resource.Id.generate_button);
            generateButton.Click += (sender, args) => ClickGenerateButton?.Invoke(sender, args);
            return view;
        }
    }
}