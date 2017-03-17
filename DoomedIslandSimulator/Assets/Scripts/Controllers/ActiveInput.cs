using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveInput : MonoBehaviour, IPointerDownHandler  {
    private GameObject Player;
    public int Slot { get; set; }
    public Item Item { get; set; }
    //private PlayerInput Pi;
    [SerializeField]
    private GameObject Button;


    public static Dictionary<int, KeyCode> Hotkeys = new Dictionary<int, KeyCode>() {
        { 0, KeyCode.Z },
        { 1, KeyCode.X },
        { 2, KeyCode.C },
        { 3, KeyCode.V },
        { 4, KeyCode.B },
        { 5, KeyCode.N }
    };
    private void Awake() {
        this.Slot = -1;
    }

    private void Start() {
        Player = GameObject.Find("Player");
        //this.Pi = Player.GetComponent<PlayerInput>();
    }

    private void Update() {
        if (this.Slot == -1)
            return;
        if (Input.GetKeyDown(Hotkeys[Slot])) {
            ExecuteEvents.Execute(Button, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            try {
               // Pi.Actions[Item.GetName()]();
            }
            catch (KeyNotFoundException e) {
             //   Debug.Log("No implementation for usage of the item " + Item.GetName() + "!");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Item != null) {
           // Pi.Actions[Item.GetName()]();
        }

    }
}
