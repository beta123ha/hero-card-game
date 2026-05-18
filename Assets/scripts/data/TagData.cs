using UnityEngine;

[CreateAssetMenu(fileName = "NewTag", menuName = "Game Data/Tag")]
public class TagData : ScriptableObject
{
    public string tagName;

    [TextArea(2, 4)]
    public string description;
}