using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Nagominashare.WordDictionary {
    class DictionaryReader : IDictionaryReader {
        private Resources _resources;

        public DictionaryReader(Resources resources) {
            _resources = resources;
        }

        public IEnumerable<string> ReadAll() {
            var result = new List<string>();
            using (
                var reader = new StreamReader(_resources.OpenRawResource(Resource.Raw.NAIST_Japanese_Dictionary_HackU)
                )) {
                long count = 0;
                while (!reader.EndOfStream) {
                    if (count % 8 == 0 || true) {
                        result.Add(reader.ReadLine());
                    } else {
                        reader.ReadLine();
                    }
                    count++;
                }
            }

            return result;
        }
    }
}