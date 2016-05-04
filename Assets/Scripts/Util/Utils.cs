using UnityEngine;

namespace Util {
	public class Utils {

		public static int randomInt(int min, int max) {
			return Mathf.FloorToInt(Random.Range (min, max));
		}

		public static int randomInt(int max) {
			return randomInt (0, max);
		}

		public static bool nextBoolean() {
			return Random.Range (0, 1) > 0.5f;
		}

        public static long randomLong(long min, long max) {
            return System.Convert.ToInt64(Mathf.Floor(Random.Range(min, max)));
        }

        public static string readableLong(long number) {
            if(number > 1000000000000) {
                float trillion = number / 1000000000000;
                return trillion + "T";
            } else if(number > 1000000000) {
                float billion = number / 1000000000;                
                return billion + "B";
            } else if(number > 1000000) {
                float million = number / 1000000;
                return million + "M";
            } else if(number > 1000) {
                float thousand = number / 1000;
                return thousand + "K";
            } else {
                return number + "";
            }
        }
    }
}

