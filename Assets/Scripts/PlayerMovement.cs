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

	// Interractable Area Attributes
	bool isAreaGraveYard = false;
	bool isAreaStartDoor = false;
	bool isAreaStarterDialog = false;
	bool isAreaDeadRat = false;

	bool canSpawnFootStep = true;

	[SerializeField] GameObject interractedObject;

	[SerializeField] GameObject[] footstepSounds;

	private void Start()
	{
		if (objectiveText != null)
		{
			objectiveText.text = objectives[currentObjectiveIndex];
		}

		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		// Etkileþimi uygula
		ApplyInterractions();

		if (!isPlayerCanMove) return;

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
		if (Input.GetKeyUp(KeyCode.J))
		{
			objectiveBanner.SetActive(!objectiveBanner.activeSelf);
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
				isPlayerCanMove = false;
				SceneManager.LoadScene(1);
			}
		}

		if (isAreaGraveYard)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
				string[] dialogs = { "Demek bunlar atalarým.",
					"Bunu gerçekten yapmak zorunda mýyým?!",
					"Her neyse, þu kaleye bir göz atalým." };

				textBox.SetActive(true);

				TextBoxFunction(dialogs);
			}
			else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
			{
				string[] dialogs = { "Demek bunlar atalarým.",
					"Bunu gerçekten yapmak zorunda mýyým?!",
					"Her neyse, þu kaleye bir göz atalým." };

				currentDialogIndex += 1;
				TextBoxFunction(dialogs);
			}
		}

		if (isAreaDeadRat)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E))
			{
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

		if (isAreaStarterDialog)
		{
			youCanInterractObject.SetActive(true);
			if (Input.GetKeyUp(KeyCode.E) && currentDialogIndex == 0)
			{
				string[] dialogs = { "Demek geldin. Ailenin mirasý yüzünden bu kaleyi korumak zorundasýn!",
					"Atalarýnýn mezarý hemen kalenin yanýnda. Sen de bir gün orada yatacaksýn hahahaha!!" };

				textBox.SetActive(true);

				TextBoxFunction(dialogs);
			}
			else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
			{
				string[] dialogs = { "Demek geldin. Ailenin mirasý yüzünden bu kaleyi korumak zorundasýn!",
					"Atalarýnýn mezarý hemen kalenin yanýnda. Sen de bir gün orada yatacaksýn hahahaha!!" };

				currentDialogIndex += 1;
				TextBoxFunction(dialogs);
			}
		}

		if (!isAreaStartDoor && !isAreaGraveYard && !isAreaStarterDialog && !isAreaDeadRat)
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

	async void SpawnFootsteps(float multiplier)
	{
		canSpawnFootStep = false;
		Instantiate(footstepSounds[Random.Range(0, footstepSounds.Length)], this.transform.position, Quaternion.identity);
		await UniTask.WaitForSeconds(0.3f/multiplier);

		canSpawnFootStep = true;
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
				break;
			case AreaType.StartDoor:
				isAreaGraveYard = false;
				isAreaStartDoor = true;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				break;
			case AreaType.StarterDialog:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = true;
				isAreaDeadRat = false;
				break;
			case AreaType.DeadRat:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = true;
				break;
			case AreaType.Empty:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				break;
			default:
				isAreaGraveYard = false;
				isAreaStartDoor = false;
				isAreaStarterDialog = false;
				isAreaDeadRat = false;
				break;
		}
	}
}
