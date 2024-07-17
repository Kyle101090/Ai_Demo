using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DijkstraAlgorithm : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject startTilePrefab;
    public GameObject targetTilePrefab;
    public GameObject pathTilePrefab;
    public GameObject walkableTilePrefab;
    public GameObject nonwalkableTilePrefab;
    public Color walkableColor = Color.white;
    public Color unwalkableColor = Color.black;
    public Color startTileColor = Color.green;
    public Color targetTileColor = Color.red;
    public Color pathTileColor = Color.yellow;
    private Dictionary<Vector2Int, GameObject> gridTiles;
    private PriorityQueue<GameObject> queue = new PriorityQueue<GameObject>();
    private const string TilePrefix = "Tile_";

    public void SetGridTiles(Dictionary<Vector2Int, GameObject> tiles)
    {
        gridTiles = tiles;
    }

    public List<GameObject> SolveMaze(GridManager gridManager, Dictionary<Vector2Int, GameObject> gridTiles, GameObject startTile, GameObject targetTile)
    {
        this.gridTiles = gridTiles;

        if (startTile == null || targetTile == null)
        {
            Debug.LogError("Start or target tile is missing!");
            return null;
        }

        Dictionary<GameObject, float> distance = new Dictionary<GameObject, float>();
        Dictionary<GameObject, GameObject> previous = new Dictionary<GameObject, GameObject>();

        foreach (KeyValuePair<Vector2Int, GameObject> kvp in gridTiles)
        {
            GameObject tileObject = kvp.Value;
            distance[tileObject] = Mathf.Infinity;
            previous[tileObject] = null;
        }

        PriorityQueue<GameObject> queue = new PriorityQueue<GameObject>();
        queue.Enqueue(startTile, 0f);
        distance[startTile] = 0f;

        while (queue.Count > 0)
        {
            GameObject currentTile = queue.Dequeue();
            Debug.Log("Visiting node: " + currentTile.name);

            foreach (GameObject neighbor in GetNeighbors(currentTile))
            {
                if (neighbor == null) continue;

                float newDistance = distance[currentTile] + 1;
                if (newDistance < distance[neighbor])
                {
                    distance[neighbor] = newDistance;
                    previous[neighbor] = currentTile;
                    queue.Enqueue(neighbor, newDistance);
                }
            }
        }

        List<GameObject> path = new List<GameObject>();
        GameObject tileInPath = targetTile;

        while (tileInPath != null)
        {
            path.Add(tileInPath);
            tileInPath = previous[tileInPath];
        }

        path.Reverse();

        Debug.Log("Tiles in the shortest path:");
        foreach (GameObject tile in path)
        {
            Debug.Log(tile.name);
        }

        HighlightPath(path);
        Debug.Log("Maze solving algorithm completed.");

        return path; // Ensure to return the path
    }

    private Color GetTileColor(GameObject tile)
    {
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            return renderer.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on tile GameObject!");
            return Color.white; // Return default color if SpriteRenderer component is missing
        }
    }


    private bool IsStartTile(GameObject tile)
    {
        return GetTileColor(tile) == startTileColor;
    }

    private bool IsTargetTile(GameObject tile)
    {
        return GetTileColor(tile) == targetTileColor;
    }

    private void LogGridTiles()
    {
        Debug.Log("GridTiles Dictionary Keys:");
        foreach (var kvp in gridTiles)
        {
            Debug.Log($"Key: {kvp.Key}, Tile: {kvp.Value.name}");
        }
    }

    public void HighlightPath(List<GameObject> path)
    {
        foreach (var kvp in gridTiles)
        {
            if (kvp.Value.GetComponent<SpriteRenderer>().color == pathTileColor)
            {
                kvp.Value.GetComponent<SpriteRenderer>().color = walkableColor;
            }
        }
        foreach (GameObject tile in path)
        {
            tile.GetComponent<SpriteRenderer>().color = pathTileColor;
        }
    }


    private List<GameObject> GetNeighbors(GameObject tile)
    {
        List<GameObject> neighbors = new List<GameObject>();
        Vector2Int coordinates = GetGridCoordinates(tile);

        if (gridTiles.ContainsKey(coordinates + Vector2Int.up) && !IsUnwalkableTile(gridTiles[coordinates + Vector2Int.up]))
            neighbors.Add(gridTiles[coordinates + Vector2Int.up]);

        if (gridTiles.ContainsKey(coordinates + Vector2Int.down) && !IsUnwalkableTile(gridTiles[coordinates + Vector2Int.down]))
            neighbors.Add(gridTiles[coordinates + Vector2Int.down]);

        if (gridTiles.ContainsKey(coordinates + Vector2Int.left) && !IsUnwalkableTile(gridTiles[coordinates + Vector2Int.left]))
            neighbors.Add(gridTiles[coordinates + Vector2Int.left]);

        if (gridTiles.ContainsKey(coordinates + Vector2Int.right) && !IsUnwalkableTile(gridTiles[coordinates + Vector2Int.right]))
            neighbors.Add(gridTiles[coordinates + Vector2Int.right]);

        return neighbors;
    }

    private bool IsUnwalkableTile(GameObject tile)
    {
        return GetTileColor(tile) == unwalkableColor;
    }


    private Vector2Int GetGridCoordinates(GameObject tile)
    {
        string tileName = tile.name;

        // Check if the name starts with the tile prefix
        if (!tileName.StartsWith(TilePrefix))
        {
            Debug.LogError("Invalid tile name format: " + tileName);
            return Vector2Int.zero;
        }

        string[] nameParts = tileName.Substring(TilePrefix.Length).Split('_');
        if (nameParts.Length < 2)
        {
            Debug.LogError("Invalid tile name format: " + tileName);
            return Vector2Int.zero;
        }

        int xCoord, yCoord;
        if (!int.TryParse(nameParts[0], out xCoord) || !int.TryParse(nameParts[1], out yCoord))
        {
            Debug.LogError("Invalid tile name format: " + tileName);
            return Vector2Int.zero;
        }

        return new Vector2Int(xCoord, yCoord);
    }


}