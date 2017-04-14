using System;
using Android.App;
using Android.Content;

namespace Nagominashare {
    class TwitterClient {
        public static void Submit(string text, Activity activity) {
            var intent = new Intent(Intent.ActionView);
            var message = Android.Net.Uri.Encode(text);
            intent.SetData(Android.Net.Uri.Parse("twitter://post?message=" + message));
            activity.StartActivity(intent);
        }

        public static void SubmitDajare(IDajare dajare, Activity activity) {
            var message = "#�a�݂Ȃ����\n" +
                          "#hackdayjp\n" +
                          "��������������������\n" +
                          "�����܂����B\n" +
                          "\n" +
                          $"�w�@{dajare.ToString()}�@�x";
            Submit(message, activity);
        }
    }
}