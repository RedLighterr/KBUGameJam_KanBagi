using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed = 5f;
	public float runMultiplier = 1.3f;
	float moveMultiplier = 1f;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Animator animator;

	Vector2 movement;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		// Input al
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		// Animator parametrelerini ayarla
		animator.SetFloat("Horizontal", movement.x);
		animator.SetFloat("Vertical", movement.y);
		animator.SetFloat("Speed", movement.sqrMagnitude);
	}

	void FixedUpdate()
	{
		// Hareketi uygula
		ApplyMove();
	}

	void ApplyMove()
	{
		float multiplier = 1;
		animator.speed = 1;

		if (Input.GetKey(KeyCode.LeftShift))
		{
			multiplier = runMultiplier;
		}
		else
		{
			multiplier = moveMultiplier;
		}

		animator.speed = animator.speed * multiplier;
		rb.MovePosition(rb.position + movement.normalized * moveSpeed * multiplier * Time.fixedDeltaTime);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag.Contains("InterractionArea"))
		{
			AreaType areaType = collision.gameObject.GetComponent<InterractionArea>().areaType;
			TriggerArea(areaType);
		}
	}

	void TriggerArea(AreaType areaType)
	{
		switch (areaType)
		{
			case AreaType.Graveyard:
				break;
			case AreaType.StartDoor:
				break;
			default:
				break;
		}
	}
}
