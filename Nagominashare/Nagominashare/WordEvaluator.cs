namespace Nagominashare {
    abstract class WordEvaluator {
        private static WordEvaluator instance = null;
        public static WordEvaluator GetInstance() {
            return instance ?? (instance = new GoogleWordEvaluator());
        }

        protected WordEvaluator() { }
        public abstract long Evaluate(string word);
    }
}