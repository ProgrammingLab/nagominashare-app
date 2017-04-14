using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagominashare {
    class Word : IWord {
        private Hinshi _hinshi;
        private string _surface;
        private string _reading;
        private string _roma;

        public Word(string surface, Hinshi hinshi, string reading = null, string roma = null) {
            _surface = surface;
            _reading = reading;
            _roma = roma;
            _hinshi = hinshi;
        }

        public Word(string surface, string reading, string hinshi) {
            _surface = surface;
            _reading = reading;

            switch (hinshi) {
                case "名詞":
                    _hinshi = Hinshi.Meishi;
                    break;
                case "動詞":
                    _hinshi = Hinshi.Doshi;
                    break;
                case "形容詞":
                    _hinshi = Hinshi.Keiyoshi;
                    break;
                case "形容動詞":
                    _hinshi = Hinshi.Keiyodoshi;
                    break;
                case "副詞":
                    _hinshi = Hinshi.Fukushi;
                    break;
                case "連体詞":
                    _hinshi = Hinshi.Rentaishi;
                    break;
                case "接続詞":
                    _hinshi = Hinshi.Setsuzokushi;
                    break;
                case "助動詞":
                    _hinshi = Hinshi.Jodoshi;
                    break;
                case "助詞":
                    _hinshi = Hinshi.Joshi;
                    break;
                default:
                    _hinshi = Hinshi.Other;
                    break;
            }
        }

        public Hinshi GetHinshi() => _hinshi;

        public string ToKanji() => _surface;

        public string ToHiragana() => _reading;

        public string ToRoma() {
            if (!string.IsNullOrEmpty(_roma)) {
                return _roma;
            }

            var builder = new StringBuilder();
            var source = ToHiragana();
            for (var i = 0; i < source.Length; ++i) {
                if (source[i] == '、') {
                    builder.Append(",");
                    continue;
                }
                if (source[i] == '。') {
                    builder.Append(".");
                    continue;
                }
                if (source[i] == 'ー') {
                    builder.Append("^");
                    continue;
                }
                if (source[i] == 'っ') {
                    var next = Romanization.Roma[source[i + 1].ToString()];
                    builder.Append(next[0]);
                    continue;
                }
                if (source[i] == 'ん') {
                    var current = Romanization.Roma[source[i].ToString()];
                    builder.Append(current);

                    if (i + 1 < source.Length) {
                        var next = Romanization.Roma[source[i + 1].ToString()];
                        if (next[0] == 'y') {
                            builder.Append("'");
                        }
                    }

                    continue;
                }

                {
                    var current = source[i].ToString();
                    string temp;
                    if (i + 1 < source.Length &&
                        (source[i + 1] == 'ゃ' || source[i + 1] == 'ゅ' || source[i + 1] == 'ょ')) {
                        temp = Romanization.Roma[current + source[i + 1]];
                        i++;
                    } else {
                        temp = Romanization.Roma[current];
                    }
                    builder.Append(temp);
                }
            }

            return builder.ToString();
        }

        public bool ContainsAlpha() {
            return _reading.Any(c => ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'));
        }

        public override string ToString() => _surface;
    }
}