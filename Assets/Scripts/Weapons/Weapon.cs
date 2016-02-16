using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public float energyCost;
	
	public bool ammo;
	public int ammoType;
	
	public float damage;
	
	public float reloadSpeed;
	
	public float force;
	public float recoilForce;
	
	public GameObject laserBolt;
	
	public float nextFire;
	
	public float projectileSpeed;
}
