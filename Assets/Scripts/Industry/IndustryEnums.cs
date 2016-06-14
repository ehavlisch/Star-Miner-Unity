using UnityEngine;
using System.Collections;

namespace Economy {
	public enum IndustryGroup {
		REFINERY, PRODUCTION, SCIENTIFIC, OIL, MILITARY, ALLOYS, MATERIALS, PLANETARY
	}

	public enum IndustryStatus {
		OUT_OF_RESOURCES, RUNNING, STARTUP, DISABLED
	}
	
	public enum IndustryRunResult {
		RUNNING, PRODUCED, HALTED, DISABLED
	}
	
	public enum IndustryUpgradeType {
		Rate, Efficiency, Other
	}

	public static class IndustryGroups {
			
		public static float getIndustryPriceModifier(this IndustryGroup ig) {
			switch(ig) {
		case IndustryGroup.ALLOYS:
				return 5;
		case IndustryGroup.MATERIALS:
				return 3;
		case IndustryGroup.MILITARY:
				return 7;
		case IndustryGroup.OIL:
				return 8;
		case IndustryGroup.PRODUCTION:
				return 4;
		case IndustryGroup.REFINERY:
				return 5;
		case IndustryGroup.SCIENTIFIC:
				return 2;
			default:
				return 0;
			}
		}
	}
}
