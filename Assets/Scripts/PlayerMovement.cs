using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public CharacterController cc;
	public Camera cam;

	public float mouseSenseitivity;
	float currentRotX;

	public float speed;
	public float wallRunSpeed;
	public float wallJumpForce;

	public float velocityX;
	public float velocityZ;
	Vector3 potentialVelocity;
	public float wallJumpSlowdown = -9.81f;

	public float gravity = -9.81f;
	public float restingGravity;
	public float velocityY;
	public float jumpHeight;

	public Transform groundCheck;
	public float groundCheckRadius;
	public LayerMask groundMask;

	public Transform rightWallCheck;
	public Transform leftWallCheck;
	public float wallCheckRadius;
	GameObject wall;

	float currentTimer;
	float jumpInterval = 0.2f;

	bool canDoubleJump;
	bool isGround;
	bool onWall;
	bool wallJump;

	private void Start() {
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void FixedUpdate() {
		isGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
		onWall = Physics.CheckSphere(rightWallCheck.position, wallCheckRadius, groundMask);
		onWall = onWall || Physics.CheckSphere(leftWallCheck.position, wallCheckRadius, groundMask);

	//Camera Movement
		cameraMovement();

	//Player Movement
		verticalMovement();
		movement();
	}

	void cameraMovement() {
		float mouseX = Input.GetAxis("Mouse X") * mouseSenseitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSenseitivity * Time.deltaTime;

		currentRotX -= mouseY;
		currentRotX = Mathf.Clamp(currentRotX, -90f, 90f);

		cam.transform.localRotation = Quaternion.Euler(currentRotX, 0, 0);
		transform.Rotate(Vector3.up * mouseX);
	}

	void movement() {
		velocityX = Input.GetAxis("Right");
		velocityZ = Input.GetAxis("Vertical");

		Vector3 velocity = (transform.right * velocityX + transform.forward * velocityZ) * speed * Time.deltaTime;

		//Debug.Log(wallJump);

		if (wallJump) {
			//Vector3 oppositeWallDir = getWallDirection();

			Vector3 oppositeWallDir = transform.TransformDirection(Vector3.right); 
			if (Physics.CheckSphere(rightWallCheck.position, wallCheckRadius, groundMask)) {
				oppositeWallDir = transform.TransformDirection(Vector3.left);
			}

			potentialVelocity = (oppositeWallDir + Vector3.forward * .05f) * wallJumpForce;

			wallJump = false;
		} else if (onWall && currentTimer >= jumpInterval) {
			velocity = velocity / speed * wallRunSpeed;
		}

	//Decrease Wall Jump Velocity
		if (potentialVelocity.magnitude > 0.1) {
			cc.Move(potentialVelocity * Time.deltaTime);
		}

		potentialVelocity = Vector3.Lerp(potentialVelocity, Vector3.zero, 5 * Time.deltaTime);

		cc.Move(velocity);
	}

	Vector3 getWallDirection() {
	//Get Wall
		Collider[] walls = Physics.OverlapSphere(rightWallCheck.position, wallCheckRadius, groundMask);
		if (walls.Length == 0)
			walls = Physics.OverlapSphere(leftWallCheck.position, wallCheckRadius, groundMask);

		wall = walls[0].gameObject;

	//Set Player Rotation to local Wall cords
		Vector3 dir = transform.position - wall.transform.position;
		dir = Quaternion.Euler(-wall.transform.eulerAngles.y * Vector3.up) * dir;
		Vector3 point = dir + wall.transform.position;

	//Get Direction
		Vector3 localVel = Vector3.zero;

		if (point.x - wall.transform.position.x > 0.5f * wall.transform.localScale.x) {
			localVel = Vector3.forward;
		}
		else if (wall.transform.position.x - point.x > 0.5f * wall.transform.localScale.x) {
			localVel = Vector3.back;
		}
		else if (point.z - wall.transform.position.z > 0.5f * wall.transform.localScale.z) {
			localVel = Vector3.right;
		}
		else if (wall.transform.position.z - point.z > 0.5f * wall.transform.localScale.z) {
			localVel = Vector3.left;
		}

		return Quaternion.Euler(wall.transform.eulerAngles.y * Vector3.up) * localVel;
	}

	void verticalMovement() {
		if (isGround) {
			velocityY = restingGravity * Time.deltaTime;
			canDoubleJump = true;
		} 

		if (onWall && currentTimer >= jumpInterval) {
			velocityY = -gravity * Mathf.Pow(Time.deltaTime, 3);
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			jump();
		}

		velocityY += gravity * Mathf.Pow(Time.deltaTime, 2);

		cc.Move(Vector3.up * velocityY);

		currentTimer += Time.deltaTime;
	}

	void jump() {
		if (onWall) {
			wallJump = true;
			canDoubleJump = true;
		}

		if ((isGround || onWall) && currentTimer >= jumpInterval) {
			velocityY = Mathf.Sqrt(jumpHeight * -2 * gravity) * Time.deltaTime;
			currentTimer = 0;
		}

		if (!(isGround || onWall) && canDoubleJump && currentTimer >= jumpInterval) {
			velocityY = Mathf.Sqrt(jumpHeight * -2 * gravity) * Time.deltaTime;
			canDoubleJump = false;
			Debug.Log("Jump");
		}
	}
}