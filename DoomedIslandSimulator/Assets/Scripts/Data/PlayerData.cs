using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {
    private static JSONNode DataNode;
    public static JSONNode PlayerNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/PlayerData.json");
                DataNode = JSON.Parse(jsonString);
            }
            return DataNode;
        }
    }

    public int VisionRange { get; private set; }
    [SerializeField]
    public GameObject GameGrid;
    private Tile[] Tiles;
    public Tile CurrentTile { get; set; }
    public float MovementSpeed { get; set; }

    public bool IsPerformingAction { get; set; }
    public bool IsPerformingMovingAction { get; set; }

    private void Awake() {
        VisionRange = PlayerNode["VisionRange"];
        JSONNode stats = PlayerNode["Stats"];
        MovementSpeed = stats["MovementSpeed"];
        Tiles = GameGrid.GetComponent<TerrainData>().Tiles;
        CurrentTile = Tiles[Random.Range(0, Tiles.Length)];
        this.transform.position = CurrentTile.Position;
        IsPerformingAction = false;
    }

}
