using Economy;
using UnityEngine;

namespace Ship { 
	public class Engine : Cargo {
		protected float efficiency;
		protected float force;
		
		protected float forceLat;
		protected float forceRotate;

		// Optional stuff
		protected string mainSound;
		protected string ventSound;
		
		public Engine(float efficiency, float force, float mass, float forceLat, float forceRotate, float volume, float value, string name) : base(CargoType.ENGINE, mass, volume, name, value, "engine") {
			this.efficiency = efficiency;
			this.force = force;
			this.forceLat = forceLat;
			this.forceRotate = forceRotate;
		}

		public AudioSource getMainSound(GameObject gameObject) {
			if(mainSound != null) {
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.clip = Resources.Load(mainSound) as AudioClip;
				audioSource.volume = 0.1f;
				audioSource.loop = true;
				audioSource.Play();
				return audioSource;
			} else {
				return null;
			}
		}
		
		public float getForce() {
			return force;
		}
		
		public float getForceLat() {
			return forceLat;
		}
		
		public float getEfficiency() {
			return efficiency;
		}
		
		public float getForceRotate() {
			return forceRotate;
		}
	}

	public class GenericEngine : Engine {
		
		public GenericEngine() : base (100, 1600, 80, 800, 60, 100, 1000, "Generic Engine") {
			this.mainSound = "Audio/Ship/Engine/loopEngine1";
		}
	}
}

