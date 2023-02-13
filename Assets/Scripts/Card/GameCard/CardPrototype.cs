using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardPrototype", menuName = "Card/Game Card")]
public class CardPrototype : ScriptableObject
{
    public string cardName;
    public string description;

    public int maxHP;
    public int attack;

    public Sprite artwork;
}
