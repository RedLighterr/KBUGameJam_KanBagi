using UnityEngine;

public enum AreaType
{
	Graveyard,
	StartDoor
}

public class InterractionArea : MonoBehaviour
{
    [Header("Area Specs")]
    [SerializeField] public AreaType areaType = AreaType.Graveyard;
}
