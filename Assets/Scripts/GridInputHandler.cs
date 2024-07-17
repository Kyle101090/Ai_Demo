using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInputHandler : MonoBehaviour
{
    private GridManager gridManager;
    private int clickCount = 0;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    void OnMouseDown()
    {
        if (gridManager != null)
        {
            Vector2Int coordinates = GetGridCoordinates();

            if (clickCount == 0)
            {
                gridManager.SetStartTile(coordinates);
                clickCount++;
            }
            else if (clickCount == 1)
            {
                gridManager.SetTargetTile(coordinates);
                clickCount++;
            }
            else
            {
                gridManager.SetTileToNonWalkable(coordinates);
            }
        }
    }

    private Vector2Int GetGridCoordinates()
    {
        Vector3 position = transform.position;
        int x = Mathf.RoundToInt(position.x / gridManager.cellSize);
        int y = Mathf.RoundToInt(position.y / gridManager.cellSize);
        return new Vector2Int(x, y);
    }
}