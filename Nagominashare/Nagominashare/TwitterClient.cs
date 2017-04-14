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
            var message = "#和みなしゃれ\n" +
                          "#hackdayjp\n" +
                          "ﾃﾞﾃﾞﾃﾞﾃﾞﾃﾞﾃﾞﾃﾞﾃﾞﾃﾞﾃﾞ\n" +
                          "整いました。\n" +
                          "\n" +
                          $"『　{dajare.ToString()}　』";
            Submit(message, activity);
        }
    }
}