using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyList : MonoBehaviour {
	public List<Enemy> eneimes = new List<Enemy>();

	public TMP_Text enemyText;

	private void Start() {
		enemyText.text = "Enemies Left: " + eneimes.Count;
	}

	private void Update() {
		enemyText.text = "Enemies Left: " + eneimes.Count;

		if(eneimes.Count == 0 || Input.GetKeyDown(KeyCode.Z)) {
			Debug.Log("Win");
			eneimes = new List<Enemy>();

			SceneSwitcher.instance.A_LoadNextScene(2f);
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			eneimes = new List<Enemy>();
		}
	}

	public void enableEnemies() {
		foreach(Enemy enemy in eneimes) {
			enemy.enabled = true;
		}
	}

	public void disableEnemies() {
		foreach (Enemy enemy in eneimes) {
			enemy.enabled = false;
		}
	}
}
