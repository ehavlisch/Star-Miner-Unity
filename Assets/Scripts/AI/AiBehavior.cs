using UnityEngine;
using System.Collections;

namespace Ai {
	public class AiBehavior {
		private Object movementStyle;
		private Object attackOrders;
		private Object target;
		
		public Object getMovementStyle() {
			return movementStyle;
		}
		public void setMovementStyle(Object movementStyle) {
			this.movementStyle = movementStyle;
		}
		public Object getAttackOrders() {
			return attackOrders;
		}
		public void setAttackOrders(Object attackOrders) {
			this.attackOrders = attackOrders;
		}
		public Object getTarget() {
			return target;
		}
		public void setTarget(Object target) {
			this.target = target;
		}
	}
}
