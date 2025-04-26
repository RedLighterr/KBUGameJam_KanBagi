using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float followSpeed = 2f;
	public Bounds cameraBounds; // Takip edilecek sýnýr kutusu
	
	private Camera cam;
	private float halfHeight;
	private float halfWidth;

	void Start()
	{
		cam = GetComponent<Camera>();
		halfHeight = cam.orthographicSize;
		halfWidth = halfHeight * cam.aspect;
	}

	void FixedUpdate()
	{
		if (target == null) return;

		Vector3 targetPos = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);
		
		// Sýnýrlar içinde kalmasý için kýrp (clamp)
		float clampedX = Mathf.Clamp(targetPos.x, cameraBounds.min.x + halfWidth, cameraBounds.max.x - halfWidth);
		float clampedY = Mathf.Clamp(targetPos.y, cameraBounds.min.y + halfHeight, cameraBounds.max.y - halfHeight);

		transform.position = new Vector3(clampedX, clampedY, transform.position.z);
	}
}
