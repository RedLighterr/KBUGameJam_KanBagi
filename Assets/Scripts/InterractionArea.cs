using UnityEngine;

public enum AreaType
{
	Graveyard,
	StartDoor,
	Empty
}

public class InterractionArea : MonoBehaviour
{
    [Header("Area Specs")]
    [SerializeField] public AreaType areaType = AreaType.Graveyard;
}
