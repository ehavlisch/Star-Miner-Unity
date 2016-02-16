using UnityEngine;
using System.Collections;

using Events;

namespace Dialogues {
	public class DialogueTree {
		
		private Dialogue currentDialogue;
		private Dialogue rootDialogue;
		
		public DialogueTree(Dialogue diag) {
			rootDialogue = diag;
			currentDialogue = rootDialogue;
		}
		
		public Dialogue getCurrentDialogue() {
			return currentDialogue;
		}
		
		public EventResultType runOption(int optionId) {
			return currentDialogue.getOption(optionId).getEventResultType();
		}
		
		public Dialogue getRootDialogue() {
			return rootDialogue;
		}
		
		public void setRootDialogue(Dialogue rootDialogue) {
			this.rootDialogue = rootDialogue;
		}
		
		public void setCurrentDialogue(Dialogue currentDialogue) {
			this.currentDialogue = currentDialogue;
		}
	}
}
