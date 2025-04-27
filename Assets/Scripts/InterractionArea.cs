using UnityEngine;

public enum AreaType
{
	Graveyard,
	StartDoor,
	StarterDialog,
	KingChair,
	DeadRat,
	ChestRoomDoor,
	Sword,
	Empty
}

public class InterractionArea : MonoBehaviour
{
    [Header("Area Specs")]
    [SerializeField] public AreaType areaType = AreaType.Graveyard;
}
