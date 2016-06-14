using System.Collections.Generic;
using System.Text;

namespace Economy {

	public class Recipe {		
		private Dictionary<int, int> inputRatios;
        private Dictionary<int, int> outputRatios;
        private Dictionary<int, int> outputPercents;
		private int rate;
		
		public Recipe(int rate) {
			inputRatios = new Dictionary<int, int>();
			outputRatios = new Dictionary<int, int>();
			outputPercents = new  Dictionary<int, int>();
			this.rate = rate;
		}
		
		public void addInputResource(int resourceId, int ratio) {
            int value;
            if(inputRatios.TryGetValue(resourceId, out value)) {
                inputRatios[resourceId] += ratio;
            } else {
                inputRatios[resourceId] = ratio;
            }
		}
		
		public void addOutputResource(int resourceId, int ratio, int percent) {
            int value;
            if (outputRatios.TryGetValue(resourceId, out value)) {
                outputRatios[resourceId] += ratio;
                outputPercents[resourceId] += percent;
            } else {
                outputRatios[resourceId] = ratio;
                outputPercents[resourceId] = percent;
            }
		}
		
		public string tostring() {
			StringBuilder sb = new StringBuilder();
			sb.Append("Input: ");
			foreach(KeyValuePair<int, int> entry in inputRatios) {
				sb.Append(entry.Key).Append("(").Append(entry.Value).Append(") ");
			}
			sb.Append(" -> ");
			foreach(KeyValuePair<int, int> entry in outputRatios) {
                sb.Append(entry.Key).Append("(").Append(entry.Value).Append(") ");
            }
            return sb.ToString();
		}
		
		public int getInputRatio(int resourceId) {
            int value = 0;
            inputRatios.TryGetValue(resourceId, out value);
            return value;
		}
		
		public int getRate() {
			return rate;
		}
		
		public int getOutputRatio(int resourceId) {
            int value = 0;
            outputRatios.TryGetValue(resourceId, out value);
            return value;

        }

        public int getOutputPercent(int resourceId) {
            int value = 0;
            outputPercents.TryGetValue(resourceId, out value);
            return value;
		}

        public List<int> getInputs() {
            return new List<int>(inputRatios.Keys);
        }

        public List<int> getOutputs() {
            return new List<int>(outputRatios.Keys);
        }
	}

}
