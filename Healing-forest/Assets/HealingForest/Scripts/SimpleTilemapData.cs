using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HF
{
    public class SimpleTilemapData : MonoBehaviour
    {
        public Grid grid;
        public Tilemap tilemap;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f))
                {
                    Vector3 clickedPosition = hitInfo.point;

                    Vector3Int cellPositionFromGrid = grid.WorldToCell(clickedPosition);
                    Vector3Int cellPositionFromTilemap = tilemap.WorldToCell(clickedPosition);

                    Debug.Log("Clicked Position: " + clickedPosition);
                    Debug.Log("Cell Position from Grid: " + cellPositionFromGrid);
                    Debug.Log("Cell Position from Tilemap: " + cellPositionFromTilemap);

                    TileBase clickedTileBase = tilemap.GetTile(cellPositionFromTilemap);
                    if (clickedTileBase != null)
                    {
                        Debug.Log("Clicked Tile Base Name: " + clickedTileBase.name);
                    }
                    else
                    {
                        Debug.Log("There is Nothing in clicked Position.");
                    }


                }
            }

        }
    }
}
