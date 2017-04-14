using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nagominashare.WordDictionary;

namespace Nagominashare.DajareGenerator
{
    class Dajare : IDajare {
        List<IWord> sentence;

        public Dajare() {
            sentence = new List<IWord>();
        }

        public void Add(IWord word) {
            sentence.Add(word);
        }

        public void AddUsingCount() {
            foreach (IWord w in sentence) {
                if (w is IDictionaryWord) {
                    ((IDictionaryWord) w).AddUsingCount();
                }
            }
        }

        public ReadOnlyCollection<IWord> GetWords() => sentence.AsReadOnly();

        public void FeedBack(int evaluation) {
            //TODO
        }

        public override string ToString() {
            string res = "";
            foreach (IWord w in sentence) {
                if (w == null) continue;
                res += w.ToKanji();
            }
            return res;
        }

        public string ToKanji() {
            var builder = new StringBuilder();
            foreach (var word in sentence) {
                builder.Append(word?.ToKanji());
            }

            return builder.ToString();
        }

        public string ToRoma() {
            var builder = new StringBuilder();
            foreach (var word in sentence) {
                builder.Append(word?.ToRoma());
            }

            return builder.ToString();
        }
    }
}
