using UnityEngine;

[CreateAssetMenu(fileName = "PartObjectData", menuName = "Scriptable Objects/PartObjectData")]
public class PartObjectData : ScriptableObject
{
    [Header("기본 정보")]
    public string part_ID;
    public string part_name;
    public PartSlot partSlot;


    [Header("오브젝트(프리팹)")]
    public GameObject partPrefabs;
    public SpriteRenderer partIcon; //아이콘 이미지 활용할지

}
