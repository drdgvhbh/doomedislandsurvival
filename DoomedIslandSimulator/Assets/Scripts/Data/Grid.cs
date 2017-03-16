using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Grid : MonoBehaviour {
    private static JSONNode DataNode;
    private static int TrianglesInSquare = 3;
    private static int VertsInTri = 3;
    public static JSONNode GridNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/GridData.json");
                DataNode = JSON.Parse(jsonString);
            }
            return DataNode;
        }
    }

    public int NumRows { get; private set; }
    public int NumColumns { get; private set; }
    public float TileSize { get; private set; }
    public int NumTiles { get; private set; }

    [SerializeField]
    private bool IsFOW; 

    private void OnApplicationQuit() {
        DataNode = null;
    }

    private void Awake() {
        JSONNode prop = GridNode["Properties"];
        NumRows = prop["NumRows"];
        NumColumns = prop["NumColumns"];
        TileSize = prop["TileSize"];
        NumTiles = NumRows * NumColumns;
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        CreateGrid();
        this.transform.position = new Vector3(-NumRows / 2 * TileSize - TileSize / 2, NumColumns / 2 * TileSize + TileSize / 2, this.transform.position.z);
    }

    private void Start() {
    }

    private void CreateGrid() {
        int vSizeX = NumColumns + 1;
        int vSizeY = NumRows + 1;
        int numVerts = vSizeX * vSizeY;
        int numTris = NumTiles * TrianglesInSquare;

        //Intialization
        Vector3[] verts = new Vector3[numVerts];
        Vector3[] norms = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];
        Color32[] colors = new Color32[numVerts];
        int[] trisAsVerts = new int[numTris * VertsInTri];

        for (int y = 0; y < vSizeY; y++) {
            for (int x = 0; x < vSizeX; x++) {
                verts[y * vSizeX + x] = new Vector3(x * this.TileSize, -y * this.TileSize, 0);
                norms[y * vSizeX + x] = Vector3.back;
                uv[y * vSizeX + x] = new Vector2((float)x / NumColumns, 1f - (float)y / NumRows);
                if (IsFOW) {
                    colors[y * vSizeX + x] = new Color32(0, 0, 0, 255);
                }
            }
        }

        for (int y = 0; y < NumRows; y++) {
            for (int x = 0; x < NumColumns; x++) {
                int squareIndex = y * NumColumns + x;
                int triOffset = squareIndex * (TrianglesInSquare + VertsInTri);
                trisAsVerts[triOffset + 0] = y * vSizeX + x;
                trisAsVerts[triOffset + 1] = y * vSizeX + vSizeX + x + 1;
                trisAsVerts[triOffset + 2] = y * vSizeX + vSizeX + x;

                trisAsVerts[triOffset + 3] = y * vSizeX + x + 1;
                trisAsVerts[triOffset + 4] = y * vSizeX + vSizeX + x + 1;
                trisAsVerts[triOffset + 5] = y * vSizeX + x;
            }
        }

        Debug.Log(this.GetComponentInParent<Transform>().name + " initialized.");

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = trisAsVerts;
        mesh.normals = norms;
        mesh.uv = uv;
        mesh.colors32 = colors;

        this.GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

}
