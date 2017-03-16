using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFogOfWar : MonoBehaviour {
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

    private static int VertsInASquare = 6;

    // 0 -> 0 tiles, 1 -> 1 tiles, 2 -> 9 tiles, 3 -> 25 tiles ...
    private int VisionRange;
    [SerializeField]
    private GameObject FOWGrid;
    private Grid Grid;
    [SerializeField]
    private GameObject GameGrid;
    private Tile[] Tiles;


    private void Awake() {
        VisionRange = this.GetComponent<PlayerData>().VisionRange;
        Grid = FOWGrid.GetComponent<Grid>();
        Tiles = GameGrid.GetComponent<TerrainData>().Tiles;
    }

	private void Update () {
    }

    public void UpdateFogOfWar() {
        VisionRange = this.GetComponent<PlayerData>().VisionRange;
        //Assume plane is at the point the vector (0, 0, -1)
        RaycastHit hit;
        Ray ray = new Ray(transform.position + new Vector3(0, 0, -2), Vector3.forward);
        Physics.Raycast(ray, out hit, 1);
        Debug.Assert(hit.collider != null && hit.collider.gameObject.layer == 8, "Player is not on the grid!");
        MeshCollider meshCollider = (MeshCollider)hit.collider;
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Color32[] colors = mesh.colors32;
        Transform hitTransform = hit.collider.transform;

        float numVertsInTri = 3;
        int currTriIdx = hit.triangleIndex;

        for (int i = 0; i < (Tiles.Length * 2); i = i + 2) {
            Vector3[] p = new Vector3[6];
            for (int j = 0; j < numVertsInTri; j++) {
                if (Tiles[i / 2].IsDiscovered) {
                    p[j] = hitTransform.TransformPoint(vertices[triangles[i * 3 + j]]);
                    p[j + 3] = hitTransform.TransformPoint(vertices[triangles[(i + 1) * 3 + j]]);
                    colors[triangles[i * 3 + j]] = new Color32(0, 0, 0, 105);
                    colors[triangles[(i + 1) * 3 + j]] = new Color32(0, 0, 0, 105);
                } else {
                    colors[triangles[i * 3 + j]] = new Color32(0, 0, 0, 255);
                    colors[triangles[(i + 1) * 3 + j]] = new Color32(0, 0, 0, 255);
                }
            }
            Debug.DrawLine(p[0], p[1], Color.red);
            Debug.DrawLine(p[1], p[2], Color.red);
            Debug.DrawLine(p[2], p[0], Color.red);
            Debug.DrawLine(p[3], p[4], Color.red);
            Debug.DrawLine(p[4], p[5], Color.red);
            Debug.DrawLine(p[5], p[3], Color.red);
        }
        
        HashSet<int> triIndices = GetVisionIndices(currTriIdx);

        Debug.DrawRay(transform.position + new Vector3(0, 0, -2), Vector3.forward, Color.green);

        foreach (int idx in triIndices) {
            Vector3[] p = new Vector3[6];
            for (int j = 0; j < numVertsInTri; j++) {
                p[j] = hitTransform.TransformPoint(vertices[triangles[idx * 3 + j]]);
                p[j + 3] = hitTransform.TransformPoint(vertices[triangles[(idx + 1) * 3 + j]]);
                colors[triangles[idx * 3 + j]] = new Color32(0, 0, 0, 0);
                colors[triangles[(idx + 1) * 3 + j]] = new Color32(0, 0, 0, 0);
            }
            Debug.DrawLine(p[0], p[1]);
            Debug.DrawLine(p[1], p[2]);
            Debug.DrawLine(p[2], p[0]);
            Debug.DrawLine(p[3], p[4]);
            Debug.DrawLine(p[4], p[5]);
            Debug.DrawLine(p[5], p[3]);
        }
        mesh.colors32 = colors;
    }

    private HashSet<int> GetVisionIndices(int triIdx) {
        int size = VisionRange;
        if (size < 0)
            return new HashSet<int>();
        HashSet<int> triIndices = new HashSet<int>();
        triIdx = GetEvenIndex(triIdx);
        triIndices.Add(triIdx);
        HashSet<int> temp = new HashSet<int>();
        for (int i = 0; i < size; i++) {
            temp = new HashSet<int>(triIndices);
            foreach (int idx in temp) {
                int left = AddTileIndex(idx, triIndices, Tile.Sides.Left);
                int right = AddTileIndex(idx, triIndices, Tile.Sides.Right);
                int top = AddTileIndex(idx, triIndices, Tile.Sides.Top);
                int bottom = AddTileIndex(idx, triIndices, Tile.Sides.Bottom);

                if (left != -1) {
                    AddTileIndex(left, triIndices, Tile.Sides.Bottom);
                } else if (bottom != -1) {
                    AddTileIndex(bottom, triIndices, Tile.Sides.Left);
                }

                if (bottom != -1) {
                    AddTileIndex(bottom, triIndices, Tile.Sides.Right);
                } else if (right != -1) {
                    AddTileIndex(right, triIndices, Tile.Sides.Bottom);
                }

                if (right != -1) {
                    AddTileIndex(right, triIndices, Tile.Sides.Top);
                } else if (top != -1) {
                    AddTileIndex(top, triIndices, Tile.Sides.Right);
                }

                if (top != -1) {
                    AddTileIndex(top, triIndices, Tile.Sides.Left);
                } else if (left != -1) {
                    AddTileIndex(left, triIndices, Tile.Sides.Top);
                }              
            }
        }
        return triIndices;
    }

    private int AddTileIndex(int idx, HashSet<int> indices, Tile.Sides side) {
        if (idx == -1)
            return -1;
        idx = GetEvenIndex(idx);
        switch (side) {
            case Tile.Sides.Top:
                idx = idx - Grid.NumColumns * 2;         
                if (idx < 0)
                    return -1;
                indices.Add(idx);
                return idx;
            case Tile.Sides.Left:
                if (idx % Grid.NumColumns == 0)
                    return -1;
                indices.Add(idx - 2);
                return idx - 2;
            case Tile.Sides.Right:
                int curRow = (idx / Grid.NumColumns) + 1;
                if (idx == (curRow * Grid.NumColumns) - 2)
                    return -1;
                indices.Add(idx + 2);
                return idx + 2;
            case Tile.Sides.Bottom:
                idx = idx + Grid.NumColumns * 2;
                if (idx >= Grid.NumTiles * 2)
                    return -1;
                indices.Add(idx);
                return idx;
        }
        return -1;
    }

    private int GetEvenIndex(int idx) {
        if (idx % 2 != 0) {
            return idx - 1;
        } else {
            return idx;
        }
    }
}
