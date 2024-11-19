using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Create Item/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private int id;
    public int ID { get { return id; } }
    [SerializeField]
    private string itemName;
    public string ItemName { get { return itemName; } }
    [SerializeField]
    private string itemType;
    public string ItemType { get { return itemType; } }
    [SerializeField]
    private string itemDec;
    public string ItemDec { get { return itemDec; } }
    public Sprite Icon;
}
