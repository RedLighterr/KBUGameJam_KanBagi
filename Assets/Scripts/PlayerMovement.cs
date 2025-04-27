using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed = 5f;
	public float runMultiplier = 1.3f;
	float moveMultiplier = 1f;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Animator animator;

	bool isPlayerCanMove = false;
	int currentDialogIndex = 0;

	[SerializeField]
	string[] objectives;
	int currentObjectiveIndex = 0;
	int deadRatCount = 4;

	[Header("UI Elements")]
	[SerializeField] GameObject youCanInterractObject;
	[SerializeField] GameObject textBox;
	[SerializeField] GameObject objectiveBanner;
	[SerializeField] TextMeshProUGUI objectiveText;
	[SerializeField] TextMeshProUGUI textBoxText;

	Vector2 movement;

	[Header("Other")]
	[SerializeField] GameObject sword;

	// Interractable Area Attributes
	bool isAreaGraveYard = false;
	bool isAreaStartDoor = false;
	bool isAreaStarterDialog = false;
	bool isAreaDeadRat = false;
	bool isAreaChestRoomDoor = false;
	bool isAreaSword = false;

	bool canSpawnFootStep = true;
	bool canSpawnClickSound = true;
	bool canSpawnDoorSound = true;
	bool isWaitForEKeyAnim = false;

	bool isInChestRoom = false;

	bool isObtainedSword = false;
	bool canAttack = true;

	bool isPlayerGetInToChestRoom = false;
	int chestRoomOpened = 0;

	[SerializeField] GameObject interractedObject;

	[SerializeField] GameObject[] footstepSounds;
	[SerializeField] GameObject[] keyPressedSounds;
	[SerializeField] GameObject[] doorSounds;

	[SerializeField] Transform[] locations;
	[SerializeField] Bounds chestRoomBounds;
	[SerializeField] Bounds castleBounds;

	private void Start()
	{
		if (objectiveText != null)
		{
			objectiveText.text = objectives[currentObjectiveIndex];
		}

		Camera.main.GetComponent<CameraFollow>().cameraBounds = castleBounds;

		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		// Etkileþimi uygula
		ApplyInterractions();

		if (!isPlayerCanMove) return;

		if (Input.GetMouseButtonDown(0) && canAttack && isObtainedSword)
		{
			SwordAttack();
		}

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
		if (!isPlayerCanMove) return;

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

		if (movement.normalized != Vector2.zero)
		{
			if (canSpawnFootStep)
			{
				SpawnFootsteps(multiplier);
			}
		}
	}

	void ApplyInterractions()
	{
		if (!isWaitForEKeyAnim)
		{
			WaitForEKeyAnim();
		}

		if (Input.GetKeyUp(KeyCode.J))
		{
			objectiveBanner.SetActive(!objectiveBanner.activeSelf);
			if (canSpawnClickSound) { SpawnClickSounds(); }
		}

		if (textBox.activeSelf)
		{
			youCanInterractObject.SetActive(false);
			isPlayerCanMove = false;
		}

		if (isAreaStartDoor)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
				if (canSpawnClickSound) { SpawnClickSounds(); }
				isPlayerCanMove = false;
				SceneManager.LoadScene(1);
			}
		}

		if (isAreaGraveYard)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
				if (canSpawnClickSound) { SpawnClickSounds(); }
				string[] dialogs = { "Demek bunlar atalarým.",
					"Bunu gerçekten yapmak zorunda mýyým?!",
					"Her neyse, þu kaleye bir göz atalým." };

				textBox.SetActive(true);

				TextBoxFunction(dialogs);
			}
			else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
			{
				if (canSpawnClickSound) { SpawnClickSounds(); }
				string[] dialogs = { "Demek bunlar atalarým.",
					"Bunu gerçekten yapmak zorunda mýyým?!",
					"Her neyse, þu kaleye bir göz atalým." };

				currentDialogIndex += 1;
				TextBoxFunction(dialogs);
			}
		}

		if (isAreaChestRoomDoor)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E) && canSpawnDoorSound)
			{
				SpawnDoorSounds();
				isPlayerGetInToChestRoom = true;
				youCanInterractObject.SetActive(false);
			}
		}

		if (isAreaDeadRat && !isPlayerGetInToChestRoom)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
				if (canSpawnClickSound) { SpawnClickSounds(); }
				if (interractedObject != null && interractedObject.name.Contains("DeadRat"))
				{
					Destroy(interractedObject.gameObject);
					deadRatCount--;
					if (deadRatCount <= 0)
					{
						currentObjectiveIndex++;
						if (currentObjectiveIndex < objectives.Length)
						{
							objectiveText.text = objectives[currentObjectiveIndex];
						}
					}
				}
				youCanInterractObject.SetActive(false);
			}
		}
		
		if (isPlayerGetInToChestRoom && chestRoomOpened == 0)
		{
			chestRoomOpened = 1;
			currentObjectiveIndex++;
			currentObjectiveIndex++;
			objectiveText.text = objectives[currentObjectiveIndex];
		}

		if (isAreaSword)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
				if (canSpawnClickSound) { SpawnClickSounds(); }
				if (interractedObject != null && interractedObject.name.Contains("Sword"))
				{
					currentObjectiveIndex++;
					objectiveText.text = objectives[currentObjectiveIndex];
					isObtainedSword = true;
					interractedObject.SetActive(false);
				}
				youCanInterractObject.SetActive(false);
			}
		}

		if (isAreaStarterDialog)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E) && currentDialogIndex == 0)
			{
				if (canSpawnClickSound) { SpawnClickSounds(); }
				string[] dialogs = { "Demek geldin. Ailenin mirasý yüzünden bu kaleyi korumak zorundasýn!",
					"Atalarýnýn mezarý hemen kalenin yanýnda. Sen de bir gün orada yatacaksýn hahahaha!!" };

				textBox.SetActive(true);

				TextBoxFunction(dialogs);
			}
			else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
			{
				if (canSpawnClickSound) { SpawnClickSounds(); }
				string[] dialogs = { "Demek geldin. Ailenin mirasý yüzünden bu kaleyi korumak zorundasýn!",
					"Atalarýnýn mezarý hemen kalenin yanýnda. Sen de bir gün orada yatacaksýn hahahaha!!" };

				currentDialogIndex += 1;
				TextBoxFunction(dialogs);
			}
		}

		if (!isAreaStartDoor && !isAreaGraveYard && !isAreaStarterDialog && !isAreaDeadRat && !isAreaChestRoomDoor && !isAreaSword)
		{
			currentDialogIndex = 0;
			youCanInterractObject.SetActive(false);
			textBox.SetActive(false);
		}

		if (!textBox.activeSelf)
		{
			currentDialogIndex = 0;
			isPlayerCanMove = true;
		}
	}

	async void SwordAttack()
	{
		canAttack = false;
		sword.SetActive(true);
		if (movement.normalized.y > 0)
		{
			sword.transform.localRotation = new Quaternion(0, 0, 0, 0);
			sword.transform.localPosition = new Vector2(0, 1);
		}
		if (movement.normalized.y < 0)
		{
			sword.transform.localRotation = new Quaternion(0, 0, 180, 0);
			sword.transform.localPosition = new Vector2(0, -1);
		}
		if (movement.normalized.x > 0)
		{
			sword.transform.localRotation = new Quaternion(0, 0, -90, 90);
			sword.transform.localPosition = new Vector2(1, 0);
		}
		if (movement.normalized.x < 0)
		{
			sword.transform.SetLocalPositionAndRotation(new Vector2(-1, 0), new Quaternion(0, 0, 90, 90));
		}
		if (movement.normalized == Vector2.zero)
		{
			sword.transform.localRotation = new Quaternion(0, 0, 180, 0);
			sword.transform.localPosition = new Vector2(0, -1);
		}
		await UniTask.WaitForSeconds(0.5f);
		sword.SetActive(false);
		canAttack = true;
	}

	async void SpawnFootsteps(float multiplier)
	{
		canSpawnFootStep = false;
		Instantiate(footstepSounds[Random.Range(0, footstepSounds.Length)], this.transform.position, Quaternion.identity);
		await UniTask.WaitForSeconds(0.3f/multiplier);

		canSpawnFootStep = true;
	}

	async void SpawnClickSounds()
	{
		canSpawnClickSound = false;
		Instantiate(keyPressedSounds[Random.Range(0, keyPressedSounds.Length)], this.transform.position, Quaternion.identity);
		await UniTask.WaitForSeconds(0.1f);
		canSpawnClickSound = true;
	}

	async void SpawnDoorSounds()
	{
		canSpawnDoorSound = false;
		interractedObject.GetComponent<Animator>().SetTrigger("triggerDoor");
		Instantiate(doorSounds[Random.Range(0, doorSounds.Length)], this.transform.position, Quaternion.identity);
		float seconds = interractedObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length;
		await UniTask.WaitForSeconds(seconds);
		if (!isInChestRoom)
		{
			this.transform.position = locations[0].position;
			Camera.main.GetComponent<CameraFollow>().cameraBounds = chestRoomBounds;
			isInChestRoom = true; 
		}
		else if (isInChestRoom)
		{
			this.transform.position = locations[1].position;
			Camera.main.GetComponent<CameraFollow>().cameraBounds = castleBounds;
			isInChestRoom = false;
		}
		canSpawnDoorSound = true;
	}

	async void WaitForEKeyAnim()
	{
		isWaitForEKeyAnim = true;
		await UniTask.WaitForSeconds(youCanInterractObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length + 0.1f);
		youCanInterractObject.gameObject.transform.localPosition = new Vector2(0, -1);
		isWaitForEKeyAnim = false;
	}

	void TextBoxFunction(string[] dialogs)
	{
		if (currentDialogIndex >= dialogs.Length) { currentDialogIndex = 0; textBox.SetActive(false); return; }

		textBoxText.text = dialogs[currentDialogIndex];
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag.Contains("InterractionArea"))
		{
			AreaType areaType = collision.gameObject.GetComponent<InterractionArea>().areaType;
			TriggerArea(areaType);
			interractedObject = collision.gameObject;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag.Contains("InterractionArea"))
		{
			TriggerArea(AreaType.Empty);
			interractedObject = null;
		}
	}

	void CollectableThings(AreaType areaType, GameObject gameObject)
	{
		TriggerArea(areaType);

	}

	void TriggerArea(AreaType areaType)
	{
		switch (areaType)
		{
			case AreaType.Graveyard:
				isAreaGraveYard = true;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				isAreaChestRoomDoor = false;
				isAreaSword = false;
				break;
			case AreaType.StartDoor:
				isAreaGraveYard = false;
				isAreaStartDoor = true;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				isAreaChestRoomDoor = false;
				isAreaSword = false;
				break;
			case AreaType.StarterDialog:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = true;
				isAreaDeadRat = false;
				isAreaChestRoomDoor = false;
				isAreaSword = false;
				break;
			case AreaType.DeadRat:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = true;
				isAreaChestRoomDoor = false;
				isAreaSword = false;
				break;
			case AreaType.ChestRoomDoor:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				isAreaChestRoomDoor = true;
				isAreaSword = false;
				break;
			case AreaType.Sword:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				isAreaChestRoomDoor = false;
				isAreaSword = true;
				break;
			case AreaType.Empty:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				isAreaChestRoomDoor = false;
				isAreaSword = false;
				break;
			default:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				isAreaChestRoomDoor = false;
				isAreaSword = false;
				break;
		}
	}
}
