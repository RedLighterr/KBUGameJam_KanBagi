using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public float health = 100;
	[SerializeField] float followDistance = 10f;
	PlayerMovement playerMovement;

	private void Start()
	{
		playerMovement = FindObjectOfType<PlayerMovement>();
	}

	private void Update()
	{
		if (playerMovement != null)
		{
			Vector2 playerXY = playerMovement.transform.position;
			float distanceX = math.distance(this.transform.position.x, playerXY.x);
			float distanceY = math.distance(this.transform.position.y, playerXY.y);
			Vector2 distance = new Vector2(distanceX, distanceY);

			if (distance.magnitude <= followDistance)
			{
				Debug.Log("Takip Baþladý");
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag.Contains("Sword"))
		{
			health -= 10;
			if (health <= 0)
			{
				Destroy(this.gameObject);
			}
		}
	}
}
