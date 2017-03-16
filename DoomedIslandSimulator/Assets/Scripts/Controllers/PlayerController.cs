using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private PlayerData Pd;
    private Tile.Sides Direction;
    private Tile DestinationTile;

    [SerializeField]
    private GameObject GridObj;
    private Grid Grid;
    private void Awake() {
        Pd = GetComponent<PlayerData>();
        Grid = GridObj.GetComponent<Grid>();
    }

    private void Update() {
        if (Pd.IsPerformingAction)
            return;
        if (CheckForMovement())
            return;

    }

    private bool CheckForMovement() {
        DestinationTile = Pd.CurrentTile;
        if (Input.GetAxisRaw("Vertical") > 0) {
            Direction = Tile.Sides.Top;
            if (Pd.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Top))
                DestinationTile = Pd.CurrentTile.Neighbours[Tile.Sides.Top];
        } else if (Input.GetAxisRaw("Vertical") < 0) {
            Direction = Tile.Sides.Bottom;
            if (Pd.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Bottom))
                DestinationTile = Pd.CurrentTile.Neighbours[Tile.Sides.Bottom];
        } else if (Input.GetAxisRaw("Horizontal") > 0) {
            Direction = Tile.Sides.Right;
            if (Pd.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Right)) 
                DestinationTile = Pd.CurrentTile.Neighbours[Tile.Sides.Right];
        } else if (Input.GetAxisRaw("Horizontal") < 0) {
            Direction = Tile.Sides.Left;
            if (Pd.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Left))
                DestinationTile = Pd.CurrentTile.Neighbours[Tile.Sides.Left];
        } else {                                                                     
            Pd.IsPerformingMovingAction = false;
            return false;
        }
        float step = Mathf.Abs(Pd.MovementSpeed / Pd.CurrentTile.MovementCost) * Time.deltaTime;

        if (DestinationTile.IsWalkable) {
            transform.position = Vector3.MoveTowards(transform.position, DestinationTile.Position, step);
        } else if (Vector3.Distance(DestinationTile.Position, Pd.CurrentTile.Position) > Grid.TileSize / 2) {
            transform.position = Vector3.MoveTowards(transform.position, Pd.CurrentTile.Position, step);
        }
        Pd.DiscoverTiles();
        Pd.CalculateCurrentTIle();
        Pd.IsPerformingMovingAction = true;
        return true;
    }
}
