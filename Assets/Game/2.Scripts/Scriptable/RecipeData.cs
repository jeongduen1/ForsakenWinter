using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Create Recipe/Recipe")]
public class RecipeData : ScriptableObject
{
    [SerializeField]
    private int id;
    public int ID { get { return id; } }
    [SerializeField]
    private string recipeName;
    public string RecipeName { get { return craftingItem.ItemName; } }
    public List<ItemData> items = new List<ItemData>();
    public ItemData craftingItem;
}
