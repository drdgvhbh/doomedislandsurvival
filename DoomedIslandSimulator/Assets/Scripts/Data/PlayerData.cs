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
    private GameObject GameGrid;
    private Tile[] Tiles;

    private void Awake() {
        VisionRange = PlayerNode["VisionRange"];
        Tiles = GameGrid.GetComponent<TerrainData>().Tiles;
        this.transform.position = Tiles[Random.Range(0, Tiles.Length)].Position;
    }

}
