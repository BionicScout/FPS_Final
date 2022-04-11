using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public CharacterController cc;
	public Camera cam;
	public float mouseSenseitivity;

	float currentRotX;
	float currentRotY;

	private void Start() {
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void FixedUpdate() {
	//Camera Movement
		float mouseX = Input.GetAxis("Mouse X") * mouseSenseitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSenseitivity * Time.deltaTime;

		currentRotX -= mouseY;
		currentRotX = Mathf.Clamp(currentRotX, -90f, 90f);

		cam.transform.localRotation = Quaternion.Euler(currentRotX, 0, 0);
		transform.Rotate(Vector3.up * mouseX);
	}
}