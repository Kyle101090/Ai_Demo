using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSAlgorithm : MonoBehaviour
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
    private const string TilePrefix = "Tile_";

    public List<GameObject> computedPath;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("BFSGridManager not found in the scene!");
        }
    }

    public List<GameObject> FindShortestPath(GameObject startTile, GameObject targetTile)
    {
        Vector2Int startCoordinates = GetGridCoordinates(startTile);
        Vector2Int targetCoordinates = GetGridCoordinates(targetTile);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startCoordinates);

        Dictionary<Vector2Int, Vector2Int> parentMap = new Dictionary<Vector2Int, Vector2Int>();
        parentMap[startCoordinates] = startCoordinates;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            Debug.Log("Visiting node: " + current);

            if (current == targetCoordinates)
            {
                computedPath = ReconstructPath(parentMap, startCoordinates, targetCoordinates);
                return computedPath;
            }

            foreach (Vector2Int neighborCoordinates in GetNeighbors(current))
            {
                if (!parentMap.ContainsKey(neighborCoordinates))
                {
                    queue.Enqueue(neighborCoordinates);
                    parentMap[neighborCoordinates] = current;
                }
            }
        }
        Debug.Log("No shortest path is avaliable!");
        //return null;
        computedPath = null; // Clear the path if no solution is found
        return computedPath;
    }

    private List<GameObject> ReconstructPath(Dictionary<Vector2Int, Vector2Int> parentMap, Vector2Int start, Vector2Int target)
    {
        List<GameObject> path = new List<GameObject>();
        Vector2Int current = target;

        while (current != start)
        {
            path.Add(GetTileAtCoordinates(current));
            current = parentMap[current];
        }

        path.Reverse();

        Debug.Log("Shortest path:");
        foreach (GameObject tile in path)
        {
            Debug.Log(tile.name);
        }

        return path;

    }

    public void HighlightPath(List<GameObject> newPath)
    {
        Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

        foreach (var kvp in gridManager.gridTiles)
        {
            SpriteRenderer renderer = kvp.Value.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                originalColors[kvp.Value] = renderer.color;
                renderer.color = walkableColor; 
            }
        }

        foreach (GameObject tile in newPath)
        {
            SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = pathTileColor; 
            }
        }

        foreach (var kvp in originalColors)
        {
            if (!newPath.Contains(kvp.Key) && kvp.Value != pathTileColor)
            {
                kvp.Key.GetComponent<SpriteRenderer>().color = kvp.Value;
            }
        }
    }

    private bool IsValidTile(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < gridManager.grid.GetLength(0) &&
               coordinates.y >= 0 && coordinates.y < gridManager.grid.GetLength(1);
    }

    private List<Vector2Int> GetNeighbors(Vector2Int coordinates)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        int x = coordinates.x;
        int y = coordinates.y;

        Vector2Int upCoordinates = new Vector2Int(x, y + 1);
        if (IsValidTile(upCoordinates) && !IsUnwalkableTile(GetTileAtCoordinates(upCoordinates)))
            neighbors.Add(upCoordinates);

        Vector2Int downCoordinates = new Vector2Int(x, y - 1);
        if (IsValidTile(downCoordinates) && !IsUnwalkableTile(GetTileAtCoordinates(downCoordinates)))
            neighbors.Add(downCoordinates);

        Vector2Int leftCoordinates = new Vector2Int(x - 1, y);
        if (IsValidTile(leftCoordinates) && !IsUnwalkableTile(GetTileAtCoordinates(leftCoordinates)))
            neighbors.Add(leftCoordinates);

        Vector2Int rightCoordinates = new Vector2Int(x + 1, y);
        if (IsValidTile(rightCoordinates) && !IsUnwalkableTile(GetTileAtCoordinates(rightCoordinates)))
            neighbors.Add(rightCoordinates);

        return neighbors;
    }

    private bool IsUnwalkableTile(GameObject tile)
    {
        return GetTileColor(tile) == unwalkableColor;
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

    private GameObject GetTileAtCoordinates(Vector2Int coordinates)
    {
        return gridManager.GetTileAtCoordinates(coordinates);
    }
}

