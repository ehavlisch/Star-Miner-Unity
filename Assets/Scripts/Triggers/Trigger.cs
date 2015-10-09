using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Triggers {
	public class Trigger {
		
		private string triggerName;
		
		private string comsMessage;
		private Object triggerType;
		private List<Object> worldObjects;
		
		public Trigger(string triggerName, Object triggerType) {
			this.triggerName = triggerName;
			this.triggerType = triggerType;
		}
		
		public string getComsMessage() {
			return comsMessage;
		}
		
		public void setComsMessage(string comsMessage) {
			this.comsMessage = comsMessage;
		}
		
		public Object getTriggerType() {
			return triggerType;
		}
		
		public void setTriggerType(Object triggerType) {
			this.triggerType = triggerType;
		}
		
		public List<Object> getWorldObjects() {
			return worldObjects;
		}
		
		public void setWorldObjects(List<Object> worldObjects) {
			this.worldObjects = worldObjects;
		}
		
		public string toString() {
			return triggerName;
		}
		
	}
}
