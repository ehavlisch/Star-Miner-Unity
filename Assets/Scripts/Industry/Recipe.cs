using System.Collections.Generic;
using System.Text;

namespace Economy {

	public class Recipe {
		private List<int?> input;
		private List<int?> output;
		
		private Dictionary<int, int> inputRatios;
        private Dictionary<int, int> outputRatios;
        private Dictionary<int, int> outputPercents;
		private int rate;
		
		public Recipe(int inputSize, int outputSize, int rate) {
			input = new List<int?>(inputSize);
			output = new List<int?>(outputSize);
			inputRatios = new Dictionary<int, int>();
			outputRatios = new Dictionary<int, int>();
			outputPercents = new  Dictionary<int, int>();
			this.rate = rate;
		}
		
		public void addInputResource(int resourceId, int ratio) {
			input.Add(resourceId);
            int value;
            if(inputRatios.TryGetValue(resourceId, out value)) {
                inputRatios[resourceId] += ratio;
            } else {
                inputRatios[resourceId] = ratio;
            }
		}
		
		public void addOutputResource(int resourceId, int ratio, int percent) {
			output.Add(resourceId);
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
			foreach(int? resourceId in input) {
				sb.Append(resourceId).Append(" ");
			}
			sb.Append(" -> ");
			foreach(int? resourceId in output) {
				sb.Append(resourceId).Append(" ");
			}
			return sb.ToString();
		}
		
		public List<int?> getInputs() {
			return input;
		}
		
		public List<int?> getOutputs() {
			return output;
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
	}

}
