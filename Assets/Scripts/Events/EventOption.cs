using System;
using System.Collections;
using System.Collections.Generic;

using DialogueNS;
using Ai;
using Triggers;

namespace Events {
	public class EventOption {
		
		// Message for the button
		private string optionLabel;
		private string optionMessage;
		
		private string actionMessage;
		private List<ActionResult> actionResults;
		
		private Dialogue dialogueResult;
		
		private EventResultType eventResultType;
		
		private bool suppressDialogue = false;
		
		// Variables for updating
		private int number;
		private List<AiBehavior> aiBehaviors;
		private List<Trigger> triggers;
		
		
		/*
		 * Constructors
		 */
		public EventOption(string optionLabel, string optionMessage, Dialogue dialogueResult) {
			this.optionLabel = optionLabel;
			this.optionMessage = optionMessage;
			this.dialogueResult = dialogueResult;
			eventResultType = EventResultType.Dialogue;
		}
		
		public EventOption(string optionLabel, string optionMessage, params ActionResult[] actionResult) {
			this.optionLabel = optionLabel;
			this.optionMessage = optionMessage;
			this.actionResults = new List<ActionResult>();
			foreach(ActionResult ar in actionResult) {
				actionResults.Add(ar);
			}
			eventResultType = EventResultType.Action;
		}
		
		public EventOption(string optionLabel, string optionMessage, string actionMessage, params ActionResult[] actionResult) {
			this.optionLabel = optionLabel;
			this.optionMessage = optionMessage;
			this.actionMessage = actionMessage;
			this.actionResults = new List<ActionResult>();
			foreach(ActionResult ar in actionResult) {
				actionResults.Add(ar);
			}
			eventResultType = EventResultType.Action;
		}
		
		public EventOption(string optionLabel, string optionMessage, Dialogue dialogueResult, string actionMessage, params ActionResult[] actionResult) {
			this.optionLabel = optionLabel;
			this.optionMessage = optionMessage;
			this.dialogueResult = dialogueResult;
			this.actionMessage = actionMessage;
			this.actionResults = new List<ActionResult>();
			foreach(ActionResult ar in actionResult) {
				actionResults.Add(ar);
			}
			eventResultType = EventResultType.DialogueAction;
			suppressDialogue = true;
		}
		
		/*
		 * Getters/Setters
		 */
		
		public string getOptionLabel() {
			return optionLabel;
		}
		
		public string getOptionMessage() {
			return optionMessage;
		}
		
		public string getActionMessage() {
			return actionMessage;
		}
		
		public void setActionMessage(string actionMessage) {
			this.actionMessage = actionMessage;
		}
		
		public List<ActionResult> getActionResults() {
			return actionResults;
		}
		
		public void setActionResults(List<ActionResult> actionResults) {
			this.actionResults = actionResults;
		}
		
		public Dialogue getDialogueResult() {
			return dialogueResult;
		}
		
		public void setDialogueResult(Dialogue dialogueResult) {
			this.dialogueResult = dialogueResult;
		}
		
		public void setOptionLabel(string optionLabel) {
			this.optionLabel = optionLabel;
		}
		
		public void setOptionMessage(string optionMessage) {
			this.optionMessage = optionMessage;
		}
		
		public EventResultType getEventResultType() {
			return eventResultType;
		}
		
		public void setEventResultType(EventResultType eventResultType) {
			this.eventResultType = eventResultType;
		}
		
		public int getNumber() {
			return number;
		}
		
		public void setNumber(int number) {
			this.number = number;
		}
		
		public List<AiBehavior> getAiBehaviors() {
			return aiBehaviors;
		}
		
		public void setAiBehaviors(List<AiBehavior> aiBehaviors) {
			this.aiBehaviors = aiBehaviors;
		}
		
		public bool isSuppressDialogue() {
			return suppressDialogue;
		}
		
		public void setSuppressDialogue(bool suppressDialogue) {
			this.suppressDialogue = suppressDialogue;
		}
		
		public void addTrigger(Trigger trigger) {
			if(triggers == null) {
				triggers = new List<Trigger>();
			}
			triggers.Add(trigger);
		}
		
		public List<Trigger> getTriggers() {
			return triggers;
		}
		
		public void setTriggers(List<Trigger> triggers) {
			this.triggers = triggers;
		}
	}
}