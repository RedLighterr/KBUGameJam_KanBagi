using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
	public Transform target;
	Transform prevTargetTransform;
	bool targetSwitch = false;
	public float followSpeed = 5f;
	public Bounds cameraBounds; // Takip edilecek sýnýr kutusu

	private Camera cam;
	private float halfHeight;
	private float halfWidth;

	void Start()
	{
		cam = GetComponent<Camera>();
		halfHeight = cam.orthographicSize;
		halfWidth = halfHeight * cam.aspect;

		if (target == null) return;
		prevTargetTransform = target;
	}

	void LateUpdate()
	{
		if (target == null) return;

		Vector3 targetPos = Vector3.Lerp(transform.position, prevTargetTransform.position, followSpeed * 0.01f);
		
		// Sýnýrlar içinde kalmasý için kýrp (clamp)
		float clampedX = Mathf.Clamp(targetPos.x, cameraBounds.min.x + halfWidth, cameraBounds.max.x - halfWidth);
		float clampedY = Mathf.Clamp(targetPos.y, cameraBounds.min.y + halfHeight, cameraBounds.max.y - halfHeight);

		transform.position = new Vector3(clampedX, clampedY, transform.position.z);
	}
}
