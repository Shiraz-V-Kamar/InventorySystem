using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Gun,
    Bullets,
    MedicKit
}
[CreateAssetMenu(menuName ="Scriptable Object/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public ItemType Type;
    public string Name;
    public string Description;
    public bool IsStackable;
    public bool IsUnique;
    public Sprite Image;

}
