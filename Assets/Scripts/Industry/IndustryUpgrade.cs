using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Industry {
	public class IndustryUpgrade {
		private string name;
		private IndustryUpgradeType upgradeType;
		private int? efficiency;
		private int? rate;
		private int? other;
		float price;
		
		private int? tier;
		private int? tierCategory;
		
		public static List<IndustryUpgrade> generateUpgrades(IndustryUpgradeType upgradeType, int? numberUpgrades, string upgradeName, int? baseNumber, int? numberStep, float basePrice, float priceStep, bool multiplyPrice, int? tierCategory) {
			List<IndustryUpgrade> upgrades = new List<IndustryUpgrade>();
			double price = basePrice;
			int? rate = null;
			int? efficiency = null;
			int? other = null;
			for(int i = 0; i < numberUpgrades; i++) {
				IndustryUpgrade upgrade = new IndustryUpgrade();
				upgrade.setPrice(Mathf.Floor(price));
				if(multiplyPrice) {
					price *= priceStep;
				} else {
					price += priceStep;
				}
				upgrade.setUpgradeType(upgradeType);
				switch(upgradeType) {
				case IndustryUpgradeType.Rate: {
					if(rate == null) {
						rate = baseNumber;
					}
					upgrade.setRate(rate);
					rate += numberStep;
					break;
				}
				case IndustryUpgradeType.Efficiency: {
					if(efficiency == null) {
						efficiency = baseNumber;
					}
					upgrade.setEfficiency(efficiency);
					efficiency += numberStep;
					break;
				}
				case IndustryUpgradeType.Other: 
				default:
					if(other == null) {
						other = baseNumber;
					}
					upgrade.setOther(other);
					other += numberStep;
				}
				
				if(i > 0) {
					upgrade.setName(upgradeName + " " + toRomanNumerals(i));
				} else {
					upgrade.setName(upgradeName);
				}
				
				if(tierCategory != null) {
					upgrade.setTierCategory(tierCategory);
					upgrade.setTier(i);
				}
				
				upgrades.add(upgrade);
			}
			return upgrades;
		}
		
		public static string toRomanNumerals(int? number) {
			StringBuilder s = new StringBuilder();
			if(number >= 40) {
				//System.out.println("Maybe should have added support for more than 40 roman numerals...");
			}
			while (number >= 10) {
				s.Append("X");
				number -= 10;
			}
			while (number >= 9) {
				s.Append("IX");
				number -= 9;
			}
			while (number >= 5) {
				s.Append("V");
				number -= 5;
			}
			while (number >= 4) {
				s.Append("IV");
				number -= 4;
			}
			while (number >= 1) {
				s.Append("I");
				number -= 1;
			}
			return s.Tostring();
		}
		
		public string tostring() {
			return name + " (" + price + ")";
		}
		
		public IndustryUpgradeType getUpgradeType() {
			return upgradeType;
		}
		
		public void setUpgradeType(IndustryUpgradeType upgradeType) {
			this.upgradeType = upgradeType;
		}
		
		public int? getEfficiency() {
			return efficiency;
		}
		
		public void setEfficiency(int? efficienty) {
			this.efficiency = efficienty;
		}
		
		public int? getRate() {
			return rate;
		}
		
		public void setRate(int? rate) {
			this.rate = rate;
		}
		
		public int? getOther() {
			return other;
		}
		
		public void setOther(int? other) {
			this.other = other;
		}
		
		public double getPrice() {
			return price;
		}
		
		public void setPrice(double price) {
			this.price = price;
		}
		
		public string getName() {
			return name;
		}
		
		public void setName(string name) {
			this.name = name;
		}
		
		public int? getTier() {
			return tier;
		}
		
		public void setTier(int? tier) {
			this.tier = tier;
		}
		
		public int? getTierCategory() {
			return tierCategory;
		}
		
		public void setTierCategory(int? tierCategory) {
			this.tierCategory = tierCategory;
		}
	}
}