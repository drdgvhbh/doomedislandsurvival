using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    public enum Items {
        Gem,
        Pickaxe,
        Placeholder,
        Shovel,
        Stick,
        Stone,
        Tent,
        Wood,
        Spear,
        Coconut,
        Fabric,
        Radar,
        Berry,
        Cocoberry
    }
    private static JSONNode DataNode;
    public static JSONNode ItemNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/ItemData.json");
                DataNode = (JSON.Parse(jsonString))["Items"];
            }
            return DataNode;
        }
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public int MaximumQuantity { get; private set; }
    public Sprite Icon { get; private set; }
    public int IconIndex { get; private set; }
    public float StaminaCost { get; private set; }
    public float ChannelDuration { get; private set; }
    public bool Consumable { get; private set; }
    public string[] Locations { get; private set; }
    public string[] UsableIn { get; private set; }

    public int Quantity { get; set; }
    public int Slot { get; set; }

    public Item(Items it) : this(it, 1) {
    }

    public Item(Items it, int quantity) {
        JSONNode thisNode = ItemNode[it.ToString()];
        Name = thisNode["Name"];
        MaximumQuantity = thisNode["MaximumQuantity"];
        Slot = -1;
        Icon = Resources.LoadAll<Sprite>(thisNode["Icon"])[thisNode["IconIndex"]];
        StaminaCost = thisNode["StaminaCost"];
        ChannelDuration = thisNode["ChannelDuration"];
        Consumable = thisNode["Consumable"];
        //this.NourishmentReplenishment = ItemNode["NourishmentReplenishment"];
        Quantity = quantity;
    }
}
