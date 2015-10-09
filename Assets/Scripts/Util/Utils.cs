using UnityEngine;

namespace Util {
	public class Utils {

		public static int randomInt(int min, int max) {
			return Mathf.FloorToInt(Random.Range (min, max));
		}

		public static int randomInt() {
			return randomInt (0, 100);
		}

		public static int randomInt(int max) {
			return randomInt (0, max);
		}

		public static bool nextBoolean() {
			return Random.Range (0, 1) > 0.5f;
		}
	}
}

