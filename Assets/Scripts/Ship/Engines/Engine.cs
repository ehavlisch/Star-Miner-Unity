using UnityEngine;

namespace Ship
{
	
	public class Engine : Cargo {
		public float efficiency;
		public float force;
		
		public float forceLat;
		public float forceRotate;

		// Optional stuff
		public string mainSound;
		public string ventSound;
		
		public Engine(float efficiency, float force, float mass, float forceLat, float forceRotate, float volume, float value, string name) {
			this.efficiency = efficiency;
			this.force = force;
			this.mass = mass;
			this.forceLat = forceLat;
			this.forceRotate = forceRotate;
			
			this.volume = volume;
			this.value = value;
			this.name = name;
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
	}

	public class GenericEngine : Engine {
		
		public GenericEngine() : base (100, 1600, 80, 800, 60, 100, 1000, "Generic Engine") {
			this.mainSound = "Audio/Ship/Engine/loopEngine1";
		}
	}
}

