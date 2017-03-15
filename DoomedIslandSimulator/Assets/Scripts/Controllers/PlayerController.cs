using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private PlayerData Pd;
    private Tile.Sides Direction;
    private Tile DestinationTile;

    private void Awake() {
        Pd = GetComponent<PlayerData>();
    }

    private void Update() {
        if (Pd.IsPerformingAction)
            return;
        if (CheckForMovement())
            return;

    }

    private bool CheckForMovement() {
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
        Pd.IsPerformingMovingAction = true;
        float step = (Pd.MovementSpeed / DestinationTile.MovementCost) * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, DestinationTile.Position, step);
        Debug.Log((Vector3.Distance(transform.position, DestinationTile.Position)));
        if (Vector3.Distance(transform.position, DestinationTile.Position) == 0) {
            Pd.CurrentTile = DestinationTile;
        }
        return true;
    }
}
