using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character {

	public NavMeshAgent agent;
	GameObject player;
	public bool inAttackRange, inSightRange;

	public float attackRange, sightRange, patrolRadius;
	public LayerMask playerMask;
	Vector3 targetLoc, lastFrameLoc;

	public GunScript gun;
	public float rotationSpeed;

	public int maxHP = 100;
	float currentHP;

	EnemyList enemyList;

	private void Awake() {
		enemyList = FindObjectOfType<EnemyList>();
		enemyList.eneimes.Add(this);
	}

	void Start() {
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.Find("Player");
		currentHP = maxHP;
		patrol();
		lastFrameLoc = transform.position;
	}

	void Update() {
		inSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
		inAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

		//Check if behind cover
		if (inSightRange)
			inSightRange = !isBehindCover();

	//Action State
		if (!inSightRange) {
			patrol();
		}
		else if (inAttackRange) {
			attack();
		}
		else if (!inAttackRange) {
			chase();
		}
		lastFrameLoc = transform.position;
	}

	bool isBehindCover() {
		Collider[] players = Physics.OverlapSphere(transform.position, sightRange, playerMask);
		if (players.Length == 0)
			return true;

			foreach (Collider player in players) {
				RaycastHit hit;
				Vector3 direction = player.gameObject.transform.position - transform.position;

				if (!Physics.Raycast(transform.position, direction.normalized, out hit, sightRange))
					continue;

				if (hit.collider.gameObject == player.gameObject) {
					return false;
				}
			}

		return true;
	}

	public void patrol() {
		bool isAtTarget = true;

		if (targetLoc != null) {
			agent.SetDestination(targetLoc);
			isAtTarget = false;

			Vector3 adjustedTarget = targetLoc.x * Vector3.right + targetLoc.z * Vector3.forward;
			Vector3 adjustedCurrent = transform.position.x * Vector3.right + transform.position.z * Vector3.forward;

			//Debug.Log(Vector3.Distance(adjustedCurrent, adjustedTarget));

			if (Vector3.Distance(adjustedCurrent, adjustedTarget) <= 0.5f) {
				isAtTarget = true;
				//Debug.Log("At target");
			}
		}

		if (isAtTarget || Vector3.Distance(transform.position, lastFrameLoc) == 0) {
			getTargetLoc();

			agent.SetDestination(targetLoc);
			//Debug.Log("NewLoc");
		}
	}

	void getTargetLoc() {

			float xOffset = 0;
			while (patrolRadius / 2 >= Mathf.Abs(xOffset)) {
				xOffset = Random.Range(-patrolRadius, patrolRadius);
			}

			float zOffset = 0;
			while (patrolRadius / 2 >= Mathf.Abs(zOffset)) {
				zOffset = Random.Range(-patrolRadius, patrolRadius);
			}

			targetLoc = (transform.position.x + xOffset) * Vector3.right;
			targetLoc += (transform.position.z + zOffset) * Vector3.forward;

	}

	public void chase() {
		agent.SetDestination(player.transform.position);
		targetLoc = player.transform.position;
		//Debug.Log("Chasing");
	}

	public void attack() {
		gun.isAiShooting = true;

	//Stop Moving
		agent.SetDestination(transform.position);
		targetLoc = player.transform.position;

		//Rotate Towards Player
		Vector3 playerLoc = player.transform.position - transform.position;
		Vector3 temp = Vector3.RotateTowards(transform.forward, playerLoc.z * Vector3.forward + playerLoc.x * Vector3.right, rotationSpeed * Time.deltaTime, 0);
		transform.rotation = Quaternion.LookRotation(temp);
	}

	public override void hit(float damage) {
		currentHP -= damage;
		//Debug.Log("Enemy Health: " + currentHP);

		if (currentHP <= 0) {
			//Debug.Log("Enemy Dead");
			enemyList.eneimes.Remove(this);
			Destroy(this.gameObject);
		}
	}
}
