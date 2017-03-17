using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SlotContainer : MonoBehaviour, IDropHandler {
    public Item CurrentItem { get; private set; }
    public GameObject ItemContainer { get; private set; }

    private static GameObject ItemContainerPrefab;
    public static PlayerData Pd;

    private void Awake() {
        ItemContainerPrefab = Resources.Load("Prefabs/ItemContainer") as GameObject;
        Pd = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerData>();
    }

    public void OnDrop(PointerEventData eventData) {
        ItemInput droppedItem = eventData.pointerDrag.GetComponent<ItemInput>();
        SlotContainer otherSlot = droppedItem.GetComponent<ItemInput>().SlotParent.GetComponent<SlotContainer>();
        if (otherSlot == this)
            return;
        //if (!CraftingSlot && !otherSlot.CraftingSlot || CraftingSlot && otherSlot.CraftingSlot) {
        SlotToSlot(droppedItem, otherSlot);
        /*} else if (CraftingSlot && !otherSlot.CraftingSlot) {
            SlotCraftTransfer(droppedItem, otherSlot, PlayerData.CraftingInventory, PlayerData.NumCraftingSlots,
                PlayerData.CraftingSlots, PlayerData.CraftingItems, Pd.GetInventory());
        } else {
            SlotCraftTransfer(droppedItem, otherSlot, Pd.GetInventory(), PlayerData.NumItemSlots,
                PlayerData.Slots, PlayerData.Items, PlayerData.CraftingInventory);
        }
        */
    }

    /*  protected void SlotCraftTransfer(ItemInput droppedItem, SlotInput otherSlot, Dictionary<string, Item> InventoryAdd, int numSlots,
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
    }*/

    protected void SlotToSlot(ItemInput droppedItem, SlotContainer otherSlot) {
        //Transferring the item (if it exists) from this slot to the other
        if (this.CurrentItem != null) {
            Item incomingItem = otherSlot.CurrentItem;
            GameObject incomingItemContainer = otherSlot.ItemContainer;
            int incomingItemSlot = incomingItem.Slot;
            GameObject incomingSlotParent = incomingItemContainer.GetComponent<ItemInput>().SlotParent;

            Item thisItem = CurrentItem;
            GameObject thisItemContainer = ItemContainer;
            int thisSlot = thisItem.Slot;
            GameObject thisSlotParent = thisItemContainer.GetComponent<ItemInput>().SlotParent;

            otherSlot.CurrentItem = thisItem;
            otherSlot.CurrentItem.Slot = thisSlot;
            thisItemContainer.GetComponent<ItemInput>().SlotParent = incomingSlotParent;
            incomingItemContainer.GetComponent<ItemInput>().SlotParent = thisSlotParent;
            this.CurrentItem = incomingItem;
            this.CurrentItem.Slot = incomingItemSlot;
        } else {
            otherSlot.CurrentItem.Slot = Pd.SlotContainers.IndexOf(this);
            this.CurrentItem = otherSlot.CurrentItem;
            droppedItem.SlotParent = this.gameObject;
            this.ItemContainer = otherSlot.ItemContainer;
            otherSlot.CurrentItem = null;
            otherSlot.ItemContainer = null;
            
        }


    }

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
            ItemContainer.GetComponent<ItemInput>().SlotParent = this.gameObject;
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
