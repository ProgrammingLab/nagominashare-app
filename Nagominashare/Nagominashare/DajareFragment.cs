
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Nagominashare.DajareGenerator;

namespace Nagominashare {
    class DajareFragment : Fragment {
        private const string KanjiKey = "EXTRA_STRING_ARRAY_KANJI";
        private const string RomaKey = "EXTRA_STRING_ARRAY_ROMA";
        private const string HinshiKey = "EXTRA_INT_ARRAY_HINSHI";

        private IDajare _dajare;

        public static Bundle BuildBundle(IDajare dajare) {
            var bundle = new Bundle();
            var kanjiList = new List<string>();
            var romaList = new List<string>();
            var hinshiList = new List<int>();

            foreach (var word in dajare.GetWords()) {
                if (word == null) continue;
                kanjiList.Add(word.ToString());
                //TODO Dajare.ToRoma()‚ÌƒoƒO‚ª’¼‚Á‚½‚çŽg‚Á‚Ä‚Ë
                //romaList.Add(word.ToRoma());
                romaList.Add("for debug");
                hinshiList.Add((int) word.GetHinshi());
            }

            bundle.PutStringArrayList(KanjiKey, kanjiList);
            bundle.PutStringArrayList(RomaKey, romaList);
            bundle.PutIntArray(HinshiKey, hinshiList.ToArray());

            return bundle;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            base.OnCreateView(inflater, container, savedInstanceState);

            var kanji = Arguments.GetStringArrayList(KanjiKey);
            var roma = Arguments.GetStringArrayList(RomaKey);
            var hinshi = Arguments.GetIntArray(HinshiKey);
            _dajare = new Dajare();
            for (var i = 0; i < kanji.Count; ++i) {
                var word = new Word(kanji[i],
                    (Hinshi) System.Enum.ToObject(typeof (Hinshi), hinshi[i]));
                _dajare.Add(word);
            }

            var view = inflater.Inflate(Resource.Layout.Dajare, null, false);
            var dajareJpn = view.FindViewById<Android.Widget.TextView>(Resource.Id.dajare_jpn);
            dajareJpn.Text = _dajare.ToKanji();
            var dajareRoma = view.FindViewById<Android.Widget.TextView>(Resource.Id.dajare_romaji);
            //TODO ToRoma()‚ÍNRE‚ðthrow‚·‚é‚Ì‚Å‚¾‚ß
            dajareRoma.Text = "";
            //dajareRoma.Text = _dajare.ToRoma();

            var shareButton = view.FindViewById<Android.Widget.ImageButton>(Resource.Id.share_button);
            shareButton.Click +=
                (sender, args) => TwitterClient.SubmitDajare(_dajare, Activity);

            return view;
        }
    }
}