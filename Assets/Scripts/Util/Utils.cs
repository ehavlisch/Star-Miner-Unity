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

        public static T getMember<T>(T[] array) {
            return array[randomInt(array.Length)];
        }

        public static T getItemByPercents<T>(T[] items, int[] percentages) {
            if (items == null || percentages == null || items.Length != percentages.Length) {
                Debug.LogWarning("Mismatched items and percentages arrays.");
                return default(T);
            }

            if(items.Length == 1) {
                return items[0];
            }

            int totalPercent = 0;

            for (int i = 0; i < items.Length; i++) {
                if (percentages[i] <= 0) {
                    Debug.LogWarning("Percentage passed was 0 or negative.");
                    return default(T);
                }
                totalPercent += percentages[i];
            }

            int random = Utils.randomInt(totalPercent);
            totalPercent = 0;
            for (int i = 0; i < items.Length; i++) {
                if (random <= totalPercent + percentages[i]) {
                    return items[i];
                }
                totalPercent += percentages[i];
            }
            Debug.LogWarning("getItemByPercents did not match any results.");
            return default(T);
        }
    }
}

