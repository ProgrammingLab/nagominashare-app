using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using Java.Security;
using Nagominashare.DajareGenerator;
using Nagominashare.Speaking;
using Nagominashare.VoiceRecogning;

namespace Nagominashare {
    class DajareModel {
        public delegate void DajareEventHandler(IDajare dajare);

        private delegate void RecognizeEventHandler(List<IWord> words);

        public event DajareEventHandler OnDajare;

        public event DajareEventHandler OnDajared;

        public event DajareEventHandler OnDajareFailed;

        public StateEnum CurrentStateEnum => CurrentState?.GetStateEnum() ?? StateEnum.Uninitialized;

        public static DajareModel Instance { get; } = new DajareModel();

        private static readonly TaskFactory TaskFactory = new TaskFactory();

        private State CurrentState { get; set; }
        private IGenerator _dajareGenerator;
        private IRecognizer _recognizer;

        public void Start(IGenerator dajareGenerator, IRecognizer recognizer) {
            if (dajareGenerator == null) {
                throw new ArgumentNullException(nameof(dajareGenerator));
            }
            if (recognizer == null) {
                throw new ArgumentNullException(nameof(recognizer));
            }

            if (CurrentState != null && !(CurrentState is PauseState)) {
                throw new InvalidOperationException();
            }

            _dajareGenerator = dajareGenerator;
            _recognizer = recognizer;

            CurrentState = new StandbyState(_recognizer);
            CurrentState.Handle();
        }

        public async void Pause() {
            if (CurrentState is PauseState) {
                throw new InvalidOperationException();
            }

            if (CurrentState is DajareState) {
                CurrentState = new StandbyState(_recognizer);
            }
            else if (CurrentState is StandbyState) {
                CurrentState = new PauseState();
            }

            await CurrentState.Handle();
        }

        public void StartGenerating() {
            if (!(CurrentState is StandbyState)) {
                throw new InvalidOperationException();
            }

            var standbyState = (StandbyState) CurrentState;
            OnRecognize(standbyState.StopRecognizing());
        }

        public int RecognizedCount() {
            if (!(CurrentState is StandbyState)) {
                throw  new InvalidOperationException();
            }

            var standbyState = (StandbyState) CurrentState;
            return standbyState.RecognizedCount();
        }

        private async void OnRecognize(List<IWord> words) {
            if (!(CurrentState is StandbyState)) {
                throw new InvalidOperationException();
            }

            CurrentState = new DajareState(_dajareGenerator, OnDajare, OnDajared, OnDajareFailed, words);
            await CurrentState.Handle();
            CurrentState = null;
            Start(_dajareGenerator, _recognizer);
        }


        public enum StateEnum {
            Uninitialized,
            Standby,
            Pause,
            Dajare
        }

        private abstract class State {
            public abstract Task Handle();

            public abstract StateEnum GetStateEnum();
        }

        private class StandbyState : State {
            private readonly IRecognizer _recognizer;
            private volatile bool _stopFlag;
            private List<IWord> _words = new List<IWord>();

            public StandbyState(IRecognizer recognizer) {
                _recognizer = recognizer;
            }

            public override Task Handle() {
                return TaskFactory.StartNew(() => {
                    try {
                        var recordTime = TimeSpan.FromSeconds(3);

                        while (true) {
                            if (_stopFlag) return;
                            var recorder = new AudioRecorder();
                            Log.Debug("recording", "start");
                            var recordingTask = recorder.StartRecord(recordTime);
                            recordingTask.Wait();
                            Log.Debug("recording", "end");
                            if (recordingTask.IsFaulted) {
                                Log.Error("recording", "˜^‰¹Ž¸”s");
                                throw new InvalidOperationException("˜^‰¹‚ÉŽ¸”s‚µ‚Ü‚µ‚½");
                            }
                            var buffer = recordingTask.Result;

                            var extractingTask = _recognizer.ExtractWords(buffer);
                            extractingTask.Wait();
                            if (extractingTask.IsFaulted) {
                                Log.Error("recording", "‰¹º”FŽ¯‚ÉŽ¸”s");
                                throw new InvalidOperationException("‰¹º”FŽ¯‚ÉŽ¸”s‚µ‚Ü‚µ‚½");
                            }

                            lock (_words) {
                                if (_stopFlag) return;
                                _words.AddRange(extractingTask.Result);
                                PrintWords(_words);
                            }
                        }
                    }
                    catch (Exception e) {
                        Log.Debug("recording", e.ToString());
                        throw;
                    }
                });
            }

            public override StateEnum GetStateEnum() => StateEnum.Standby;

            public List<IWord> StopRecognizing() {
                lock (_words) {
                    _stopFlag = true;
                    return _words;
                }
            }

            public int RecognizedCount() => _words.Count;

            [Conditional("DEBUG")]
            private static void PrintWords(List<IWord> words) {
                Log.Debug("recognizing", $"count:{words.Count}");
                foreach (var word in words) {
                    Log.Debug("recognizing", word.ToString());
                }
            }
        }

        private class PauseState : State {
            public override Task Handle() => Task.CompletedTask;

            public override StateEnum GetStateEnum() => StateEnum.Pause;
        }

        private class DajareState : State {
            private readonly IGenerator _generator;
            private readonly DajareEventHandler _onDajareHandler;
            private readonly DajareEventHandler _onDajaredHandler;
            private readonly DajareEventHandler _onDajareFailedHandler;
            private readonly List<IWord> _words; 

            public DajareState(IGenerator generator, DajareEventHandler onDajareHandler, DajareEventHandler onDajaredEventHandler, DajareEventHandler onDajareFailedHandler, List<IWord> words) {
                _generator = generator;
                _onDajareHandler = onDajareHandler;
                _onDajaredHandler = onDajaredEventHandler;
                _onDajareFailedHandler = onDajareFailedHandler;
                _words = words;
            }

            public override async Task Handle() {
                var dajare = await _generator.Generate(_words);
                if (dajare == null) {
                    _onDajareFailedHandler?.Invoke(null);
                    return;
                }
                //var dajare = new Dajare();
                //dajare.Add(new Word("‚Ù‚°", Hinshi.Meishi, "‚Ù‚°"));
                PrintDajare(dajare);
                _onDajareHandler?.Invoke(dajare);
                var speaker = new Speaker();
                var task = speaker.Speak("‚Æ‚Æ‚Ì‚¢‚Ü‚µ‚½");
                await task;
                await speaker.Speak(dajare.ToString());
                await Task.Delay(TimeSpan.FromSeconds(114514));
                _onDajaredHandler?.Invoke(dajare);
            }

            public override StateEnum GetStateEnum() => StateEnum.Dajare;

            [Conditional("DEBUG")]
            private static void PrintDajare(IDajare dajare) {
                Log.Debug("dajare", dajare.ToString());
            }
        }
    }
}