using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Events;

namespace Dialogues {
	public class Dialogue {
		
		private string message;
		
		private List<EventOption> options;
		
		private bool loop = false;
		private string[] loopResponses;
		private string[] loopMessages;
		private EventOption loopOption;
		
		public Dialogue(string message, List<EventOption> options) {
			this.message = message;
			this.options = options;
		}
		
		public EventOption selectOption(int optionId) {
			return options[optionId];
		}
		
		public string getMessage() {
			if(loop) {
				string currentMessage = message;
				message = EventFactory.getRandomString(loopResponses);

				options.Remove(loopOption);
				loopOption = new EventOption(loopOption.getOptionLabel(), EventFactory.getRandomString(loopMessages), this);
				options.Add(loopOption);
				
				return currentMessage;
			}
			return message;
		}
		
		public EventOption getOption(int optionId) {
			if(optionId >= options.Count || optionId < 0) {
				if(optionId == options.Count) {
					return new EventOption("", "", "", ActionResult.EXIT);
				} else {
					throw new EventException("OptionId out of range");
				}
			} else {
				return options[optionId];
			}
		}
		
		public void setupLoop(string repeatLabel, string[] loopMessages, string[] loopResponses) {
			options = new List<EventOption> ();
			loopOption = new EventOption(repeatLabel, EventFactory.getRandomString(loopMessages), this);
			options.Add(loopOption);
			this.loopResponses = loopResponses;
			this.loopMessages = loopMessages;
			loop = true;
		}
		
		public List<EventOption> getOptions() {
			return options;
		}
	}
}