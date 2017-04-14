using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Java.Net;
using Nagominashare.DajareGenerator;
using Nagominashare.VoiceRecogning;
using Nagominashare.VoiceRecognizing;

namespace Nagominashare {
    [Activity(Label = "Nagominashare", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity {
        private DajareModel _dajareModel;
        private ImageButton _button;
        private IGenerator _generator;
        private IRecognizer _recognizer;

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.MainActivity);

            var fragment = BuildMainFragment();
            var transaction = FragmentManager.BeginTransaction();
            transaction.Add(Resource.Id.container, fragment);
            transaction.Commit();

            _generator = new Generator(Resources);
            _recognizer = new Recognizer();

            _dajareModel = DajareModel.Instance;
            _dajareModel.OnDajare += OnDajare;
            _dajareModel.OnDajared += OnDajared;
            _dajareModel.OnDajareFailed += OnDajareFailed;
            _dajareModel.Start(_generator, _recognizer);

            fragment.ClickMonitorToggleButton += (sender, args) => {
                switch (_dajareModel.CurrentStateEnum) {
                    case DajareModel.StateEnum.Standby:
                        //_dajareModel.Pause();
                        break;
                    case DajareModel.StateEnum.Uninitialized:
                    case DajareModel.StateEnum.Pause:
                        _dajareModel.Start(_generator, _recognizer);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            };
        }

        private void OnDajare(IDajare dajare) {
            var fragment = new DajareFragment();
            fragment.Arguments = DajareFragment.BuildBundle(dajare);
            ReplaceFragment(fragment);
        }

        private void OnDajared(IDajare dajare) {
            var fragment = BuildMainFragment();
            ReplaceFragment(fragment);
        }

        private void OnDajareFailed(IDajare dajare) {
            _button.Enabled = true;
            Log.Debug("main activity", "onDajareFailled");
        }

        private MainFragment BuildMainFragment() {
            var fragment = new MainFragment();
            fragment.ClickGenerateButton += OnClickGenerateButton;
            return fragment;
        }

        private void OnClickGenerateButton(object sender, EventArgs args) {
            if (_dajareModel.RecognizedCount() == 0) return;
            _dajareModel.StartGenerating();
            _button = sender as ImageButton;
            _button.Enabled = false;
        }

        private void ReplaceFragment(Fragment fragment) {
            var transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.container, fragment);
            transaction.AddToBackStack(null);
            transaction.Commit();
        }
    }
}



