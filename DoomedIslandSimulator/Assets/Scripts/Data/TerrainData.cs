using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainData : MonoBehaviour {
    private static JSONNode DataNode;
    public static JSONNode TerrainNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/TerrainData.json");
                DataNode = JSON.Parse(jsonString);
            }
            return DataNode;
        }
    }


    //WONKY
    public static int GetIndexFromPosition(Vector3 pos, Grid Grid) {
        pos -= new Vector3(Grid.TileSize / 2, -Grid.TileSize / 2);
        Vector3 Offset = Grid.transform.position;
        pos -= Offset;
        pos = new Vector3(pos.x / Grid.TileSize, pos.y / Grid.TileSize, pos.z);
        int idx = (int)(Mathf.Round(-pos.y) * Grid.NumColumns + Mathf.Round(pos.x));
       // Debug.Log("Column:" + Mathf.Round(pos.x) + ", Row: " + Mathf.Round(-pos.y) + ", " + idx);
        return idx;
    }

    private static Vector3 GetPositionFromIndex(int tileIdx, Grid Grid) {
        int currentRow = tileIdx / Grid.NumColumns;
        int currentColumn = tileIdx % Grid.NumRows;
        Vector3 Offset = Grid.transform.position;
        Vector3 position = new Vector3(
            currentColumn * Grid.TileSize, -currentRow * Grid.TileSize, Grid.transform.position.z)
            + Offset
            + new Vector3(Grid.TileSize / 2, -Grid.TileSize / 2);
        return position;
    }

    private Grid Grid { get; set; }
    public Tile[] Tiles { get; private set; }
    public int TileResolution { get; private set; }
    private Dictionary<int, Color[][]> TerrainColors = new Dictionary<int, Color[][]>();
    [SerializeField]
    private Texture2D[] TileTextures;


    private void Awake() {
        Grid = this.GetComponent<Grid>();
        Debug.Assert(Grid != null);
        
        Tiles = new Tile[Grid.NumTiles];
        TileResolution = TerrainNode["TileResolution"];
        CreateTerrain();
        CreateTexture();
        UpdateTileView();
    }

	private void Start () {
        /*for (int i = 0; i < Tiles.Length; i++) {
            Debug.Log("Id:" + Tiles[i].Id + " Neighbour Length: " + Tiles[i].Neighbours.Count);
        }*/
       /* foreach (KeyValuePair<Tile.Sides, Tile> t in Tiles[4].Neighbours) {
            Debug.Log(t.Key + " " + t.Value.Id);
        }
        */
    }

	private void Update () {
		
	}

    private void CreateTerrain() {
        JSONNode borderRatio = TerrainNode["BorderRatios"];
        //Create top row
        for (int i = 0; i < Grid.NumColumns; i++ ) {
            CreateSandOrWater(i, i, i - 1, borderRatio, Tile.Sides.Left, Tile.Sides.Right);
        }
        //Create left column
        for (int i = 1; i < Grid.NumRows; i++) {
            CreateSandOrWater(i, i * Grid.NumColumns, (i - 1) * Grid.NumColumns, borderRatio, Tile.Sides.Top, Tile.Sides.Bottom);
        }
        //Create bottom row
        if (Grid.NumRows > 1) {
            for (int i = 1; i < Grid.NumColumns; i++) {
                int idx = (Grid.NumRows - 1) * (Grid.NumColumns) + i;
                CreateSandOrWater(i, idx, idx - 1, borderRatio, Tile.Sides.Left, Tile.Sides.Right);
            }
        }
        //Create right column
        if (Grid.NumColumns > 1) {
            for (int i = 1; i < Grid.NumRows - 1; i++) {
                int idx = ((i + 1) * Grid.NumColumns) - 1;
                CreateSandOrWater(i, idx, idx - Grid.NumColumns, borderRatio, Tile.Sides.Top, Tile.Sides.Bottom);
            }
        }
        if (Tiles.Length > 1 && Grid.NumRows > 1) {
            int index = (Grid.NumRows * Grid.NumColumns) - 1;
            Tiles[index].AddNeighbour(Tile.Sides.Top, Tiles[index - Grid.NumColumns]);
            Tiles[index - Grid.NumColumns].AddNeighbour(Tile.Sides.Bottom, Tiles[index]);
        }


        JSONNode borderAllowances = TerrainNode["BorderAllowances"]; 
        if (Tiles.Length >= 9) {
            int smallCols = (Grid.NumColumns - 2);
            int smallRows = (Grid.NumRows - 2);
            for (int i = 0; i < smallCols * smallRows; i++) {
                int currentRow = 1 + (i / smallCols);
                int currentColumn = 1 + (i % smallRows);
                int idx = (Grid.NumColumns * currentRow) + currentColumn;
                //Get the left neighbour
                Dictionary<Tile.Sides, Tile> neigh = new Dictionary<Tile.Sides, Tile>();
                if (Tiles[idx - 1] != null) {
                    neigh.Add(Tile.Sides.Left, Tiles[idx - 1]);
                }
                //Get the Top Neighbour
                if (Tiles[idx - Grid.NumColumns] != null)
                    neigh.Add(Tile.Sides.Top, Tiles[idx - Grid.NumColumns]);
                //Get the bottom Neighbour
                if (Tiles[idx + Grid.NumColumns] != null)
                    neigh.Add(Tile.Sides.Bottom, Tiles[idx + Grid.NumColumns]);
                //Get the right Neighbour
                if (Tiles[idx + 1] != null)
                    neigh.Add(Tile.Sides.Right, Tiles[idx + 1]);
                 Dictionary<int, int> possibleTileTypes = new Dictionary<int, int>();
                foreach (KeyValuePair<Tile.Sides, Tile> t in neigh) {
                    for (int j = 0; j < borderAllowances[(t.Value.Id).ToString()].Count; j++) {
                        int key = borderAllowances[(t.Value.Id).ToString()][j];
                        if (!(possibleTileTypes.ContainsKey(key))) {
                            possibleTileTypes.Add(key, 1);
                        } else {
                            possibleTileTypes[key] += 1;
                        }
                    }

                }
                List<int> allowedTileTypes = new List<int>();
                int highestV = 0;
                int highestK = 0;
                foreach (KeyValuePair<int, int> type in possibleTileTypes) {
                    if (highestV <= type.Value) {
                        highestV = type.Value;
                        highestK = type.Key;
                    }
                    if (type.Value == neigh.Count) {
                        allowedTileTypes.Add(type.Key);
                    }
                }
                if (allowedTileTypes.Count == 0) {
                    foreach (KeyValuePair<Tile.Sides, Tile> t in neigh) {
                        allowedTileTypes.Add(highestK);
                    }
                }
                int randomType = allowedTileTypes[Random.Range(0, allowedTileTypes.Count)];
                Vector3 position = GetPositionFromIndex(idx, Grid);
                Tiles[idx] = new Tile(randomType, currentRow * currentColumn + currentColumn, position);
                Tiles[idx].Neighbours = neigh;
                if (Tiles[idx - 1] != null)
                    Tiles[idx - 1].Neighbours.Add(Tile.Sides.Right, Tiles[idx]);
                if (Tiles[idx - Grid.NumColumns] != null)
                    Tiles[idx - Grid.NumColumns].Neighbours.Add(Tile.Sides.Bottom, Tiles[idx]);
                if (Tiles[idx + Grid.NumColumns] != null)
                    Tiles[idx + Grid.NumColumns].Neighbours.Add(Tile.Sides.Top, Tiles[idx]);
                if (Tiles[idx + 1] != null)
                    Tiles[idx + 1].Neighbours.Add(Tile.Sides.Left, Tiles[idx]);
            }
        }
        for (int i = 0; i < Tiles.Length; i++) {
            Tiles[i].CalculateAutoTileID();
        }
    }

    private void CreateSandOrWater(int i, int tileIdx, int nIdx, JSONNode bRatio, Tile.Sides nDirection, Tile.Sides tDirection) {
        Vector3 position = GetPositionFromIndex(tileIdx, Grid);
        if (Random.Range(0.0f, 100.0f) <= bRatio[((int)Tile.TileType.Sand).ToString()]) {
            Tiles[tileIdx] = new Tile(Tile.TileType.Sand, tileIdx, position);
        } else {
            Tiles[tileIdx] = new Tile(Tile.TileType.Water, tileIdx, position);
        }
        if (i > 0) {
            Tiles[tileIdx].AddNeighbour(nDirection, Tiles[nIdx]);
            Tiles[nIdx].AddNeighbour(tDirection, Tiles[tileIdx]);
        }
    }

    private void CreateTexture() {
        TerrainColors.Add((int)Tile.TileType.Grass, GetColorsFromTexture2D(this.TileTextures[(int)Tile.TileType.Grass], this.TileResolution));
        TerrainColors.Add((int)Tile.TileType.Sand, GetColorsFromTexture2D(this.TileTextures[(int)Tile.TileType.Sand], this.TileResolution));
        TerrainColors.Add((int)Tile.TileType.Mountain, GetColorsFromTexture2D(this.TileTextures[(int)Tile.TileType.Mountain], this.TileResolution));
        TerrainColors.Add((int)Tile.TileType.Water, GetColorsFromTexture2D(this.TileTextures[(int)Tile.TileType.Water], this.TileResolution));
    }

    public void UpdateTileView() {
        Texture2D texture = new Texture2D(Grid.NumColumns * TileResolution, Grid.NumRows * TileResolution);
        for (int y = 0; y < Grid.NumRows; y++) {
            for (int x = 0; x < Grid.NumColumns; x++) {
                Tile tile = Tiles[y * Grid.NumColumns + x];
                Color[] autoTile = TerrainColors[tile.Id][tile.AutoTileId];
                texture.SetPixels(
                    x * this.TileResolution,
                    Grid.NumRows * this.TileResolution - y * this.TileResolution - this.TileResolution,
                    this.TileResolution,
                    this.TileResolution,
                    autoTile
                );
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        Grid.GetComponent<MeshRenderer>().sharedMaterials[0].mainTexture = texture;

        Debug.Log("Done Texture!");
    }

    private static Color[][] GetColorsFromTexture2D(Texture2D texture, int tileResolution) {
        int columns = texture.width / tileResolution;
        int rows = texture.height / tileResolution;

        Color[][] tiles = new Color[columns * rows][];

        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {
                tiles[y * columns + x] = texture.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
            }
        }
        return tiles;
    }
}
