using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public enum TileType {
        Grass = 0,
        Sand = 1,
        Mountain = 2,
        Water = 3
    }
    public enum Sides {
        Top = 1,
		Left = 1 << 1,
		Right = 1 << 2,
		Bottom = 1 << 3
    }
    private static JSONNode DataNode;
    public static JSONNode TileNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/TileData.json");
                DataNode = JSON.Parse(jsonString);
            }
            return DataNode;
        }
    }

    public int Id { get; set; }
    public int Index { get; set; }
    public string Name { get; private set; }
    public int TileDepth { get; private set; }
    public bool IsWalkable { get; private set; }
    public int MovementCost { get; private set; }

    public Dictionary<Sides, Tile> Neighbours { get; set; }
    public int AutoTileId { get; set; }
    public Vector3 Position { get; private set; }
    //public List<Item>

    public bool IsDiscovered { get; set; }
    public bool IsRevealed { get; set; }

    public int NumDigs { get; set; }

    public GameObject CurrentGameObject;
    
    public Tile(TileType type, int idx, Vector3 pos) : this((int)type, idx, pos) {
    }

    public Tile(int type, int idx, Vector3 position) {
        SetTileType(type);
        Neighbours = new Dictionary<Sides, Tile>();
        IsDiscovered = false;
        IsRevealed = false;
        CurrentGameObject = null;
        Index = idx;
        this.Position = position;
    }

    public void SetTileType(int type) {
        JSONNode DepthVariance = TileNode["DepthVariance"];
        JSONNode TileType = TileNode["Tiles"][type.ToString()];
        Debug.Assert(TileType.Count > 0, type + " does not exist as a tile type.");
        Id = TileType["Id"];
        Name = TileType["Name"];
        TileDepth = TileType["Depth"] + Random.Range(DepthVariance["Min"], DepthVariance["Max"]);
        IsWalkable = TileType["IsWalkable"];
        MovementCost = TileType["MovementCost"];
        AutoTileId = -1;
    }

    public bool AddNeighbour(Sides s, Tile t) {
        if (Neighbours.Count >= 4)
            return false;
        Neighbours.Add(s, t);
        return true;
    }

    public bool IsAdjacent(Tile t) {
        return Neighbours.ContainsValue(t);
    }

    public bool IsSameType(Tile t) {
        return Id == t.Id;
    }

    public void CalculateAutoTileID() {
        int sum = 0;
        for (int i = 0; i < Neighbours.Count; i++) { }
        foreach (KeyValuePair<Sides, Tile> neighbour in Neighbours) { 
            JSONNode autoTlleNeighbours = TileNode["Tiles"][Id]["AutoTileNeighbours"];
            JSONNode sides = TileNode["Sides"];
            for (int i = 0; i < autoTlleNeighbours.Count; i++) {               
                if ((int)autoTlleNeighbours[i] == neighbour.Value.Id) {
                    //Debug.Log("Neighbour key:" + (int)neighbour.Key);
                    sum += (int)neighbour.Key;
                }
            }
        }        
        AutoTileId = sum;
        //Debug.Log("Id: " + Id + " AutoTileID: " + AutoTileId);
    }
}
