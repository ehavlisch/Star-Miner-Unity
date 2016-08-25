using Economy;
using UnityEngine;

namespace Ship { 
	public class Engine : Cargo {
        public int engineId;
        // Higher efficiency is better
        public float efficiency;
        public float force;

        public float forceLat;
        public float forceRotate;

        // Optional stuff
        public string mainSound;
        public string ventSound;
		
		public Engine(int engineId, float efficiency, float force, float mass, float forceLat, float forceRotate, float volume, float value, string name) : base(CargoType.ENGINE, mass, volume, name, value, "engine") {
            this.engineId = engineId;
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
	}
}

