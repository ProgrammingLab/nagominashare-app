using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Util;
using Nagominashare.WordDictionary;

namespace Nagominashare.DajareGenerator {
    public class Generator : IGenerator {
        private Resources resources;
        private TaskFactory<IDajare> taskFactory = new TaskFactory<IDajare>();

        public Generator(Resources resources) {
            this.resources = resources;
        }

        private List<IWord> extractMeishi(List<IWord> words) {
            List<IWord> res = new List<IWord>();
            foreach (IWord w in words) {
                if (w.GetHinshi() == Hinshi.Meishi) res.Add(w);
            }
            return res;
        }

        public Task<IDajare> Generate(List<IWord> keywords) {
            return taskFactory.StartNew(() => {
                try {
                    keywords = extractMeishi(keywords);
                    //keywords数が多い場合は削る
                    //TODO: 調整
                    while (keywords.Count > 10) keywords.RemoveAt(keywords.Count - 1);
                    int minUsingCount = 100100100;
                    List<IDictionaryWord> searchedWords = new List<IDictionaryWord>();
                    Searcher s = Searcher.GetInstance(resources);
                    foreach (IWord w in keywords) {
                        searchedWords.Add(s.FindBestWord(w));
                        minUsingCount = Math.Min(minUsingCount, searchedWords.Last().GetUsingCount());
                    }
                    int index = -1;
                    double maxEval = -1e10;
                    for (int i = 0; i < searchedWords.Count; i++) {
                        IDictionaryWord dw = searchedWords[i];
                        if (dw.GetUsingCount() > minUsingCount) continue;
                        double eval = dw.GetEvalution();
                        if (maxEval < eval) {
                            maxEval = eval;
                            index = i;
                        }
                    }
                    Log.Debug("generator", index.ToString());
                    WordAssembler wa = new WordAssembler(keywords[index]);
                    IDajare res = wa.Assemble(searchedWords[index].GetWord());
                    if (res == null) {
                        ;
                    }
                    res.AddUsingCount();
                    return res;
                } catch (Exception e) {
                    Log.Debug("generator", e.ToString());
                    //throw;
                    return null;
                }
            });
        }
    }
}
