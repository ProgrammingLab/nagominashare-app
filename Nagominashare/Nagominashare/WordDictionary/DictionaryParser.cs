using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagominashare.WordDictionary {
    class DictionaryParser {

        public IEnumerable<IDictionaryWord> Parse(IEnumerable<string> raw) {
            var result = new List<IDictionaryWord>();

            foreach (var line in raw) {
                var split = line.Split(' ');
                var id = int.Parse(split[2]);
                Hinshi hinshi;
                switch (id) {
                    case 0:
                        hinshi = Hinshi.Meishi;
                        break;
                    case 1:
                        hinshi = Hinshi.Doshi;
                        break;
                    case 2:
                        hinshi = Hinshi.Keiyoshi;
                        break;
                    case 4:
                        hinshi = Hinshi.Fukushi;
                        break;
                    case 5:
                        hinshi = Hinshi.Rentaishi;
                        break;
                    case 7:
                        hinshi = Hinshi.Kandoshi;
                        break;
                    default:
                        hinshi = Hinshi.Other;
                        break;
                }

                result.Add(new DictionaryWord(new Word(split[0], hinshi, roma:split[1])));
            }

            return result;
        }  
    }
}