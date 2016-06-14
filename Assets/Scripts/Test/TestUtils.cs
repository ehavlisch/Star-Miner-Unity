

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Tests {
    public class TestUtils {

        private static TestUtils testUtils;

        public static TestUtils Instance {
            get {
                if (testUtils == null) {
                    testUtils = new TestUtils();
                }
                return testUtils;
            }
        }

        private Dictionary<string, Stopwatch> stopwatchMap;

        private TestUtils() {
            stopwatchMap = new Dictionary<string, Stopwatch>();
        }

        public void startTimer(string name) {
            Stopwatch stopwatch = new Stopwatch();
            Stopwatch existing;
            if (stopwatchMap.TryGetValue(name, out existing)) {
                UnityEngine.Debug.LogWarning("Stopwatch " + name + " already exists.");
                stopwatchMap.Remove(name);
            }
            stopwatchMap.Add(name, stopwatch);
            stopwatch.Start();
        }

        public long endTimer(string name) {
            Stopwatch stopwatch;
            if (stopwatchMap.TryGetValue(name, out stopwatch)) {
                stopwatch.Stop();
                stopwatchMap.Remove(name);
                return stopwatch.ElapsedMilliseconds;
            } else {
                UnityEngine.Debug.LogWarning("Stopwatch not found for name: " + name);
                return 0;
            }
        }
    }
}