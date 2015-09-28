using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Industry {

	public class Recipe {
		private List<Resource> input;
		private List<Resource> output;
		
		private Dictionary<Resource, int> inputRatios;
		private Dictionary<Resource, int> outputRatios;
		private Dictionary<Resource, int> outputPercents;
		private int rate;
		
		public Recipe(int inputSize, int outputSize, int rate) {
			input = new List<Resource>(inputSize);
			output = new List<Resource>(outputSize);
			inputRatios = new Dictionary<Resource, int>();
			outputRatios = new Dictionary<Resource, int>();
			outputPercents = new Dictionary<Resource, int>();
			this.rate = rate;
		}
		
		public void addInputResource(Resource resource, int ratio) {
			input.Add(resource);
			inputRatios.Add(resource, ratio);
		}
		
		public void addOutputResource(Resource resource, int ratio, int percent) {
			output.Add(resource);
			outputRatios.Add(resource, ratio);
			outputPercents.Add(resource, percent);
		}
		
		public string tostring() {
			StringBuilder sb = new StringBuilder();
			sb.Append("Input: ");
			foreach(Resource r in input) {
				sb.Append(r.getName()).Append(" ");
			}
			sb.Append(" -> ");
			foreach(Resource r in output) {
				sb.Append(r.getName()).Append(" ");
			}
			return sb.Tostring();
		}
		
		public List<Resource> getInputs() {
			return input;
		}
		
		public List<Resource> getOutputs() {
			return output;
		}
		
		public int getInputRatio(Resource r) {
			return inputRatios.TryGetValue(r);
		}
		
		public int getRate() {
			return rate;
		}
		
		public int getOutputRatio(Resource r) {
			return outputRatios.TryGetValue(r);
		}	
		
		public int getOutputPercent(Resource r) {
			int value = null;
			outputPercents.TryGetValue(r, value);
			return value;
		}
	}

}
