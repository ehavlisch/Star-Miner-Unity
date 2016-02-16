using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Dialogues;
using Ai;
using Triggers;

namespace Events {
	public class RandomEvent {
		
		// public to avoid warning
		public string eventName;
		
		private DialogueTree dialogueTree;
		
		private bool coms = true;
		
		// The object that triggers the event
		private Object worldObject;
		private IntVector2 location;
		private string id;
		
		private bool closed = false;
		
		public RandomEvent(string eventName, DialogueTree dialogueTree, string id) {
			this.eventName = eventName;
			this.dialogueTree = dialogueTree;
			this.id = id;
			
			// placeholder
			location = new IntVector2(0, 0);
		}
		
		public void runEvent() {
			if(closed) {
				throw new EventException("Attempting to run closed event");
			}
			if(coms) {
				Debug.Log ("(Coms open)");
			}
			runDialogue(false);
		}
		
		public void selectAction(int optionId) {
			if(closed) {
				throw new EventException("Attempting to run closed event");
			}
			
			EventOption option = dialogueTree.getCurrentDialogue().getOption(optionId);
			if(option.getOptionMessage().Length > 0) {
				Debug.Log (option.getOptionMessage());
			}
			EventResultType eventResultType = dialogueTree.runOption(optionId);
			
			switch(eventResultType) {
			case EventResultType.DialogueAction: {
				dialogueTree.setCurrentDialogue(option.getDialogueResult());
				runAction(option);
				runDialogue(option.isSuppressDialogue());
				break;
			}
			case EventResultType.Dialogue: {
				dialogueTree.setCurrentDialogue(option.getDialogueResult());
				runDialogue(false);
				break;
			}
			case EventResultType.Action: {
				bool exit = runAction(option);
				if(!exit) {
					break;
				}
				endDialogue();
				break;
			}
			case EventResultType.Exit: {
				endDialogue();
				break;
			}
			default: {
				endDialogue();
				break;
			}
			}
		}

		private void endDialogue() {
			closed = true;
			if(coms) {
				Debug.Log("(Coms closed)");
			}
		}
		
		private void runDialogue(bool suppressMessage) {
			if(!suppressMessage) {
				string message = dialogueTree.getCurrentDialogue().getMessage();
				
				Debug.Log (message);
			}
			
			foreach(EventOption option in dialogueTree.getCurrentDialogue().getOptions()) {
				Debug.Log("- " + option.getOptionLabel());
			}
			if(coms) {
				Debug.Log("- Close coms");
			} else {
				Debug.Log("- Leave");
			}
		}
		
		private bool runAction(EventOption option) {
			if(option.getActionMessage().Length > 0) {
				Debug.Log(option.getActionMessage());
			}
			foreach(ActionResult actionResult in option.getActionResults()) {
				switch(actionResult) {
				case ActionResult.DROP_CARGO: {
					IntVector2 dropLocation = new IntVector2(location.x + 5, location.y - 5);
					Debug.Log("TODO: Instantiate object at: " + dropLocation + " and exit event");
					break;
				}
				case ActionResult.SEND_FUEL: {
					Debug.Log("(Sent " + option.getNumber() + " fuel to " + id + ")");
					Debug.Log("TODO: Deduct 5 fuel from player if possible");
					break;
				}
				case ActionResult.UPDATE_AI_BEHAVIOR: {
					if(worldObject != null) {
						foreach(AiBehavior aiBehavior in option.getAiBehaviors()) {
							Debug.Log(aiBehavior);
							//worldObject.addBehavior(aiBehavior);
						}
					}
					Debug.Log("TODO: Add ai behavior to event world object");
					break;
				}
				case ActionResult.ADD_TRIGGER: {
					foreach(Trigger t in option.getTriggers()) {
						Debug.Log("TODO: Add trigger to listener: " + t);
					}
					break;
				}
				case ActionResult.EXIT: {
					return true;
				}
				default: {
					Debug.Log("BUG: Missing actionResult from option");
					return true;
				}
				}
			}
			return true;
		}
		
		public bool isClosed() {
			return closed;
		}
		
		public void setComs(bool coms) {
			this.coms = coms;
		}
		
		
	}
}