using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {

    public Transform gunBarrel;
    public Camera cam;
    public GameObject eyeLevel;
    public ParticleSystem muzzleFlash;
    public GameObject gunHit;

    public float damage;
    public float range;

    public bool isHoldToShoot;
    public float bulletInterval;
    float bulletTimer;

    public int maxBullets;
    public int bullets;
    public GameObject bulletPrefab;
    public float bulletForce;

    public float reloadTime;
    float currentReloadTime;
    bool isReloading;

    public bool isAI;
    public bool isAiShooting;
    public GameObject gunCharacter;

    public float xSpread;
    public float ySpread;

	private void Awake() {
        bullets = maxBullets;
    }

	private void Start() {
        currentReloadTime = reloadTime;
        bulletTimer = bulletInterval;
	}

    void Update() {
        bulletTimer += Time.deltaTime;

        if(!isAI && Input.GetKeyDown(KeyCode.R)) {
            bullets = 0;
            currentReloadTime = 0;
            isReloading = true;
            AudioManager.instance.Play("Reload");
        }

 
            bool isShooting = (Input.GetMouseButtonDown(0) && !isHoldToShoot) || (Input.GetMouseButton(0) && isHoldToShoot) || isAiShooting;

            if (isShooting && bulletTimer >= bulletInterval) {
                if (isAiShooting && bullets > 0)
                    aiShoot();
                else if (!isAI && bullets > 0)
                    shoot();
                else if (reloadTime <= currentReloadTime) {
                    currentReloadTime = 0;
                    isReloading = true;

                    if (!isAI)
                        AudioManager.instance.Play("Reload");
                }

                bulletTimer = 0;
                isAiShooting = false;
            }

            if (isReloading)
                reload();

    }

    void shoot() {
        muzzleFlash.Play();
        AudioManager.instance.Play("Shoot");

        RaycastHit hit;
        Vector3 bulletDirection;

        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, range)) {
			//Debug.DrawRay(gunBarrel.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue, 5);

			GameObject temp = GameObject.Instantiate(gunHit, hit.point, Quaternion.LookRotation(hit.normal));
			Destroy(temp, 2f);

            Character character = hit.collider.GetComponent<Character>();
            if (character != null && gunCharacter != hit.collider.gameObject) {
                character.hit(damage);
            }

            ExplodingBarrel barrel = hit.collider.GetComponent<ExplodingBarrel>();
            if (barrel != null) {
                barrel.hit(damage);
            }
        }

		bullets--;
    }

    public void aiShoot() {
        muzzleFlash.Play();
        //AudioManager.instance.Play("Shoot");

        RaycastHit hit;
        GameObject temp = Instantiate(eyeLevel, eyeLevel. transform.position, eyeLevel.transform.rotation);
        Transform tempTrans = temp.transform;
        tempTrans.position += tempTrans.TransformDirection(Vector3.right) * Random.Range(-xSpread * .01f, xSpread * .01f);
        tempTrans.position += tempTrans.TransformDirection(Vector3.up) * Random.Range(-ySpread * .01f, ySpread * .01f);


        if (Physics.Raycast(tempTrans.position, tempTrans.TransformDirection(Vector3.forward), out hit, range)) {
            Debug.DrawRay(tempTrans.position, tempTrans.TransformDirection(Vector3.forward) * range, Color.blue, 5);

            GameObject gunShotEffect = GameObject.Instantiate(gunHit, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(gunShotEffect, 2f);

            Character character = hit.collider.GetComponent<Character>();
            if (character != null && gunCharacter != hit.collider.gameObject) {
                character.hit(damage);
            }

            ExplodingBarrel barrel = hit.collider.GetComponent<ExplodingBarrel>();
            if (barrel != null) {
                barrel.hit(damage);
            }
        }
        Destroy(temp);

        bullets--;
        //Debug.Log("Enemy Bullets: " + bullets);
    }

    void reload() {
        currentReloadTime += Time.deltaTime;

		if (currentReloadTime >= reloadTime) {
            bullets = maxBullets;
            isReloading = false;
		}
    }
}