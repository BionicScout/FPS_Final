using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : Character {
	public float currentHP;
	public float maxHP;

	public ProgressBar pb;

	public float healthRegenDelay;
	float regenDelayTimer;
	public float hpHealPerSecond;

	public GunScript gun;
	public TMP_Text ammoText;


	private void Start() {
		currentHP = maxHP;
		pb.BarValue = currentHP;

		ammoText.text = gun.bullets + " / " + gun.maxBullets;
	}

	private void Update() {
	//Health Regen
		regenDelayTimer += Time.deltaTime;

		if(regenDelayTimer >= healthRegenDelay) {
			currentHP = Mathf.Clamp((hpHealPerSecond * Time.deltaTime) + currentHP, 0, maxHP);
			pb.BarValue = currentHP;
		}

		//Bullet Text Update
		ammoText.text = gun.bullets + " / " + gun.maxBullets;
		
	}

	public override void hit(float damage) {
		Debug.Log("Player Damage: " + damage);
		currentHP -= damage;
		pb.BarValue = currentHP;

		regenDelayTimer = 0;

		if(currentHP <= 0) {
			AudioManager.instance.Play("Death");

			SceneSwitcher.instance.A_LoadScene(6);
		}
		else
			AudioManager.instance.Play("Hit");


	}
}
