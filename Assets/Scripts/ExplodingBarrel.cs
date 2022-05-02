using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour {
	public int phase = 1;
	public int hp = 1;
	float currentHp;

	public float explosionRadius;
	public float damage;

	public GameObject explosion;

	bool invoked;

	public void Start() {
		currentHp = hp;
	}

	public void Update() {
		if (phase == 2) {
			explode();
		}
	}

	public void explode() {
		Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRadius);
		int time = 0;
		foreach(Collider collision in collisions) {
			Character character = collision.gameObject.GetComponent<Character>();

			if (character != null) {
				character.hit(damage);
			}

			ExplodingBarrel barrel = collision.gameObject.GetComponent<ExplodingBarrel>();
			if (barrel != null && this.gameObject != collision.gameObject) {
				barrel.barrelExploded();
			}
			Debug.Log(++time);
		}

		AudioManager.instance.Play("Explosion");
		Destroy(this.gameObject);

		GameObject temp = GameObject.Instantiate(explosion, transform.position, Quaternion.LookRotation(Vector3.up));
		Destroy(temp, 5f);
	}

	public void hit(float damage) {
		currentHp -= damage;

		if(currentHp <= 0)
			phase++;
	}

	public void barrelExploded() {
		if(!invoked)
			Invoke("explode", 0.3f);

		invoked = true;
	}
}
