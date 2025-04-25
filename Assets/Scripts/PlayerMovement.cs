using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed = 5f;
	public float runMultiplier = 1.3f;
	float moveMultiplier = 1f;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Animator animator;

	[SerializeField]
	GameObject youCanInterractObject;

	Vector2 movement;

	// Interractable Area Attributes
	bool isAreaGraveYard = false;
	bool isAreaStartDoor = false;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		// Etkileþimi uygula
		ApplyInterractions();

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

	void ApplyInterractions()
	{
		if (isAreaStartDoor)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
				SceneManager.LoadScene(1);
			}
		}
		else if (!isAreaStartDoor) youCanInterractObject.SetActive(false);

		if (isAreaGraveYard)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
				Debug.Log("Graveyard");
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag.Contains("InterractionArea"))
		{
			AreaType areaType = collision.gameObject.GetComponent<InterractionArea>().areaType;
			TriggerArea(areaType);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag.Contains("InterractionArea"))
		{
			TriggerArea(AreaType.Empty);
		}
	}

	void TriggerArea(AreaType areaType)
	{
		switch (areaType)
		{
			case AreaType.Graveyard:
				isAreaGraveYard = true;
				isAreaStartDoor = false;
				break;
			case AreaType.StartDoor:
				isAreaGraveYard = false;
				isAreaStartDoor = true;
				break;
			case AreaType.Empty:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				break;
			default:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				break;
		}
	}
}
