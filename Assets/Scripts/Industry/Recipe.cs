using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Industry {

	public class Recipe {
		private List<int?> input;
		private List<int?> output;
		
		private IntegerMap inputRatios;
		private IntegerMap outputRatios;
		private IntegerMap outputPercents;
		private int rate;
		
		public Recipe(int inputSize, int outputSize, int rate) {
			input = new List<int?>(inputSize);
			output = new List<int?>(outputSize);
			inputRatios = new IntegerMap();
			outputRatios = new IntegerMap();
			outputPercents = new IntegerMap();
			this.rate = rate;
		}
		
		public void addInputResource(int resourceId, int ratio) {
			input.Add(resourceId);
			inputRatios.add(resourceId, ratio);
		}
		
		public void addOutputResource(int resourceId, int ratio, int percent) {
			output.Add(resourceId);
			outputRatios.add(resourceId, ratio);
			outputPercents.add(resourceId, percent);
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
			return inputRatios.get(resourceId);
		}
		
		public int getRate() {
			return rate;
		}
		
		public int getOutputRatio(int resourceId) {
			return outputRatios.get (resourceId);
		}	
		
		public int getOutputPercent(int resourceId) {
			return outputPercents.get (resourceId);
		}
	}

}
