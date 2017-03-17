using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePanel : MonoBehaviour {
    [SerializeField]
    private GameObject ActiveSlot;

    public static List<GameObject> ActiveContainer;
    private void Awake() {
        ActiveContainer = new List<GameObject>();
        CreateActivePanel(6);
    }

    public void CreateActivePanel() {
        GameObject activeSlot = GameObject.Instantiate(ActiveSlot);
        activeSlot.transform.SetParent(this.transform, false);
        ActiveContainer.Add(activeSlot);
    }
    public void CreateActivePanel(int quantity) {
        for (int i = 0; i < quantity; i++) {
            CreateActivePanel();
        }
    }
}
