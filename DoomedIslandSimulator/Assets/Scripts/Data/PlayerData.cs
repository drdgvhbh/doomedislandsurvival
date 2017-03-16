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
    public Tile CurrentTile { get; private set; }
    public float MovementSpeed { get; set; }

    private float Health;
    private float MaximumHealth;
    private float Stamina;
    private float MaximumStamina;

    private float Nourishment;
    private float MaximumNourishment;
    private int NourishmentLevel;

    private HashSet<Tile> DiscoveredTiles;

    public bool IsPerformingAction { get; set; }
    public bool IsPerformingMovingAction { get; set; }

    private void Awake() {
        VisionRange = PlayerNode["VisionRange"];
        JSONNode stats = PlayerNode["Stats"];
        MovementSpeed = stats["MovementSpeed"];
        Tiles = GameGrid.GetComponent<TerrainData>().Tiles;
        DiscoveredTiles = new HashSet<Tile>();
    }

    private void Start() {
        CurrentTile = Tiles[Random.Range(0, Tiles.Length)];
        this.transform.position = CurrentTile.Position;
        IsPerformingAction = false;
        DiscoverTiles();
    }

    public void CalculateCurrentTIle() {
        int idx = TerrainData.GetIndexFromPosition(this.transform.position, GameGrid.GetComponent<Grid>());
        if (Tiles[idx].IsWalkable) {
            CurrentTile = Tiles[idx];
        }
    }

    public void DiscoverTiles() {
        GetRevealedTiles();
        GetComponent<PlayerFogOfWar>().UpdateFogOfWar();
    }

    private HashSet<Tile> GetRevealedTiles() {
        int size = VisionRange;
        if (size < 0)
            return new HashSet<Tile>();
        HashSet<Tile> RevealedTiles = new HashSet<Tile>();
        CurrentTile.IsDiscovered = true;
        DiscoveredTiles.Add(CurrentTile);
        RevealedTiles.Add(CurrentTile);
        CurrentTile.IsRevealed = true;
        HashSet<Tile> temp = new HashSet<Tile>();
        for (int i = 0; i < size; i++) {
            temp = new HashSet<Tile>(RevealedTiles);
            foreach (Tile t in temp) {
               Tile left = AddDiscoveredTile(t, Tile.Sides.Left, RevealedTiles);
               Tile right = AddDiscoveredTile(t, Tile.Sides.Right, RevealedTiles);
               Tile top = AddDiscoveredTile(t, Tile.Sides.Top, RevealedTiles);
               Tile bottom = AddDiscoveredTile(t, Tile.Sides.Bottom, RevealedTiles);

                if (left != null) {
                    AddDiscoveredTile(left, Tile.Sides.Bottom, RevealedTiles);
                } else if (bottom != null) {
                    AddDiscoveredTile(bottom, Tile.Sides.Left, RevealedTiles);
                }

                if (bottom != null) {
                    AddDiscoveredTile(bottom, Tile.Sides.Right, RevealedTiles);
                } else if (right != null) {
                    AddDiscoveredTile(right, Tile.Sides.Bottom, RevealedTiles);
                }

                if (right != null) {
                    AddDiscoveredTile(right, Tile.Sides.Top, RevealedTiles);
                } else if (top != null) {
                    AddDiscoveredTile(top, Tile.Sides.Right, RevealedTiles);
                }

                if (top != null) {
                    AddDiscoveredTile(top, Tile.Sides.Left, RevealedTiles);
                } else if (left != null) {
                    AddDiscoveredTile(left, Tile.Sides.Top, RevealedTiles);
                }
            }
        }
        return RevealedTiles;
    }

    private Tile AddDiscoveredTile(Tile t, Tile.Sides side, HashSet<Tile> revealed) {
        if (t != null && t.Neighbours.ContainsKey(side)) {
            DiscoveredTiles.Add(t.Neighbours[side]);
            revealed.Add(t.Neighbours[side]);
            t.Neighbours[side].IsDiscovered = true;
            t.Neighbours[side].IsRevealed = true;
            return t.Neighbours[side];
        }
        return null;
    }
}
