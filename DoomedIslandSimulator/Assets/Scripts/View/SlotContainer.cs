using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotContainer : MonoBehaviour/*, IDropHandler*/ {
    public Item CurrentItem { get; private set; }
    public GameObject ItemContainer { get; private set; }

    private static GameObject ItemContainerPrefab;
    public static PlayerData Pd;

    private void Awake() {
        ItemContainerPrefab = Resources.Load("Prefabs/ItemContainer") as GameObject;
        Pd = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerData>();
    }

    /*public void OnDrop(PointerEventData eventData) {
        if (PlayerData.Slots[SlotID] == null)
            return;
        Debug.Assert(PlayerData.Slots[SlotID].transform.childCount == 0 || PlayerData.Slots[SlotID].transform.childCount == 1);
        ItemInput droppedItem = eventData.pointerDrag.GetComponent<ItemInput>();
        SlotInput otherSlot = droppedItem.GetComponent<ItemInput>().OriginalParent.GetComponent<SlotInput>();
        if (otherSlot == this)
            return;
        if (!CraftingSlot && !otherSlot.CraftingSlot || CraftingSlot && otherSlot.CraftingSlot) {
            SlotToSlot(droppedItem, otherSlot);
        } else if (CraftingSlot && !otherSlot.CraftingSlot) {
            SlotCraftTransfer(droppedItem, otherSlot, PlayerData.CraftingInventory, PlayerData.NumCraftingSlots,
                PlayerData.CraftingSlots, PlayerData.CraftingItems, Pd.GetInventory());
        } else {
            SlotCraftTransfer(droppedItem, otherSlot, Pd.GetInventory(), PlayerData.NumItemSlots,
                PlayerData.Slots, PlayerData.Items, PlayerData.CraftingInventory);
        }

    }

    protected void SlotCraftTransfer(ItemInput droppedItem, SlotInput otherSlot, Dictionary<string, Item> InventoryAdd, int numSlots,
        List<GameObject> slotsContainer, List<GameObject> itemsContainer, Dictionary<string, Item> InventoryRemove) {
        Item it = otherSlot.StoredItem;
        var type = it.GetType();
        var obj = (Item)Activator.CreateInstance(type, it);
        obj.ActiveContainer = null;
        obj.SetQuantity(1);
        Pd.AddItem(
            obj,
            InventoryAdd,
            numSlots,
            slotsContainer,
            itemsContainer,
            true
            );
        Pd.RemoveItem(it, 1, InventoryRemove);
    }

    protected void SlotToSlot(ItemInput droppedItem, SlotInput otherSlot) {
        if (PlayerData.Slots[this.SlotID].transform.childCount == 1) {
            otherSlot.StoredItem = this.StoredItem;
            otherSlot.StoredItem.Slot = otherSlot.SlotID;
            GameObject thisItem = PlayerData.Slots[this.SlotID].GetComponentInChildren<ItemInput>().gameObject;
            thisItem.transform.SetParent(otherSlot.transform);
            thisItem.transform.localPosition = Vector2.zero;
        } else {
            otherSlot.StoredItem.Slot = this.SlotID;
            otherSlot.StoredItem = null;
        }
        droppedItem.transform.SetParent(this.transform);
        droppedItem.transform.localPosition = Vector2.zero;
        this.StoredItem = droppedItem.GetComponent<ItemInput>().Item;
        this.StoredItem.Slot = this.SlotID;
    }*/



    public bool AddItem(Item it) {
        if (CurrentItem == null) {
            CurrentItem = it;
            it.Slot = Pd.SlotContainers.IndexOf(this);
            Transform invPanel = this.transform.parent.transform.parent;
            ItemContainer = GameObject.Instantiate(ItemContainerPrefab);
            ItemContainer.transform.SetParent(invPanel, false);
            ItemContainer.transform.position = this.transform.position;
            ItemContainer.GetComponent<Image>().sprite = it.Icon;
            Debug.Assert(ItemContainer.transform.childCount == 1);
            ItemContainer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = it.Quantity.ToString();
            return true;
        }
        return false;
    }

    public Item RemoveItem(Item.Items it) {
        if (CurrentItem != null) {
            Item temp = CurrentItem;
            CurrentItem = null;
            return temp; 
        }
        return null;
    }

}
