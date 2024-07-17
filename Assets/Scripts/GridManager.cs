using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] public Color startTileColor = Color.green;
    [SerializeField] public Color targetTileColor = Color.red;
    [SerializeField] public Color unwalkableColor = Color.black;
    [SerializeField] public Color pathTileColor = Color.yellow;
    [SerializeField] public Color walkableColor = Color.white;
    public GameObject walkableTilePrefab;
    public GameObject nonWalkableTilePrefab;
    public GameObject startTilePrefab;
    public GameObject targetTilePrefab;
    public GameObject pathTilePrefab;
    public Transform gridParent;
    public InputField xInputField;
    public InputField yInputField;
    public GameObject canvasObject;
    public GameObject secondCanvas;
    public GameObject thirdCanvas;
    private const string TilePrefix = "Tile_";
    public float cellSize = 1.0f;
    public Camera gridCamera;
    public GameObject[,] grid { get; private set; }
    public bool startTileSet = false;
    public bool targetTileSet = false;
    private Vector2Int startCoordinates;
    private Vector2Int targetCoordinates;
    public bool StartTileSet { get { return startTileSet; } }
    public bool TargetTileSet { get { return targetTileSet; } }
    public Dictionary<Vector2Int, GameObject> gridTiles = new Dictionary<Vector2Int, GameObject>();

    public GameObject dfsAgentPrefab;
    private List<DFSAgentMovement> dfsAgents;
    public InputField dfsAgentCountInput;
    private DFSAgentMovement dfsAgentMovement;
    public Button dfsAiAgentButton;
    private DFSAlgorithm dfsAlgorithm;
    public DFSUIManager dfsUIManager;

    public GameObject bfsAgentPrefab;
    private List<BFSMovement> bfsAgents;
    public InputField bfsAgentCountInput;
    private BFSMovement bfsMovement;
    public Button bfsAiAgentButton;
    private BFSAlgorithm bfsAlgorithm;
    public BFSUIManager bfsUIManager;

    public GameObject dijkstraAgentPrefab;
    private List<DijkstraMovement> dijkstraAgents;
    public InputField dijkstraAgentCountInput;
    private DijkstraMovement dijkstraMovement;
    public Button dijkstraAgentButton;
    private DijkstraAlgorithm dijkstraAlgorithm;
    public DijkstraUIManager dijkstraUIManager;

    void Start()
    {
        dfsAlgorithm = GetComponent<DFSAlgorithm>();
        if (dfsAlgorithm == null)
        {
            Debug.LogError("DFSAlgorithm component is missing");
        }

        bfsAlgorithm = GetComponent<BFSAlgorithm>();
        if (bfsAlgorithm == null)
        {
            Debug.LogError("BFSAlgorithm component not found");
        }

        dijkstraAlgorithm = GetComponent<DijkstraAlgorithm>();
        if (dijkstraAlgorithm == null)
        {
            Debug.LogError("DijkstraAlgorithm component is missing!");
        }
    }


    public void OnConfirmButtonClicked()
    {
        int xSize = int.Parse(xInputField.text);
        int ySize = int.Parse(yInputField.text);
        CreateGrid(xSize, ySize);
        AdjustCamera();
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
        secondCanvas.SetActive(true);
        thirdCanvas.SetActive(true);
    }

    public void OnHideButtonClicked()
    {
        if (secondCanvas != null)
        {
            secondCanvas.SetActive(!secondCanvas.activeSelf);
        }
    }

    public void ResetGame()
    {
        // Reset the grid tiles
        foreach (var tile in gridTiles.Values)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }

            // Clear grid-related data
        gridTiles.Clear();
        grid = null;

        // Reset start and target tile flags
        startTileSet = false;
        targetTileSet = false;

        // Reset UI input fields and canvas visibility
        xInputField.text = "";
        yInputField.text = "";
        canvasObject.SetActive(true);
        secondCanvas.SetActive(false);
        thirdCanvas.SetActive(false);

        // Reset camera or any other elements as needed
        AdjustCamera();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void AdjustCamera()
    {
        GameObject tile_0_0 = GameObject.Find("Tile_0_0");

        if (tile_0_0 != null)
        {
            Vector3 targetPosition = tile_0_0.transform.position;
            targetPosition.z = -10f;
            gridCamera.transform.position = targetPosition;
        }
        else
        {
            Debug.LogWarning("Tile_0_0 not found! Camera adjustment failed.");
        }
    }

    public void CreateGrid(int xSize, int ySize)
    {
        grid = new GameObject[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject tile = Instantiate(walkableTilePrefab, new Vector3(x * cellSize, y * cellSize, 0), Quaternion.identity, gridParent);
                tile.name = $"Tile_{x}_{y}";
                grid[x, y] = tile;
                tile.AddComponent<GridInputHandler>();
                gridTiles[new Vector2Int(x, y)] = tile;
            }
        }
    }

    public void UpdateTile(Vector2Int coordinates, GameObject tile)
    {
        gridTiles[coordinates] = tile;
    }

    public void SetStartTile(Vector2Int coordinates)
    {
        if (!startTileSet && !targetTileSet)
        {
            SetTileType(coordinates, startTilePrefab);
            startTileSet = true;
            startCoordinates = coordinates;
            UpdateTile(coordinates, GetTileAtCoordinates(coordinates));
        }
    }

    public void SetTargetTile(Vector2Int coordinates)
    {
        if (!targetTileSet && startTileSet)
        {
            SetTileType(coordinates, targetTilePrefab);
            targetTileSet = true;
            targetCoordinates = coordinates;
            UpdateTile(coordinates, GetTileAtCoordinates(coordinates));
        }
    }

    public void SetTileToNonWalkable(Vector2Int coordinates)
    {
        if (startTileSet || targetTileSet)
        {
            SetTileType(coordinates, nonWalkableTilePrefab);
            UpdateTile(coordinates, GetTileAtCoordinates(coordinates));
        }
    }

    private void SetTileType(Vector2Int coordinates, GameObject tilePrefab)
    {
        GameObject existingTile = GetTileAtCoordinates(coordinates);
        if (existingTile != null)
        {
            SpriteRenderer tileRenderer = existingTile.GetComponent<SpriteRenderer>();
            if (tileRenderer != null)
            {
                if (tilePrefab == startTilePrefab)
                {
                    tileRenderer.color = startTileColor;
                    existingTile.name = $"Tile_{coordinates.x}_{coordinates.y}";
                }
                else if (tilePrefab == targetTilePrefab)
                {
                    tileRenderer.color = targetTileColor;
                    existingTile.name = $"Tile_{coordinates.x}_{coordinates.y}";
                }
                else if (tilePrefab == nonWalkableTilePrefab)
                {
                    tileRenderer.color = unwalkableColor;
                    existingTile.name = $"Tile_{coordinates.x}_{coordinates.y}";
                }
                else
                {
                    Debug.LogError("Invalid tile prefab provided!");
                }
            }
            else
            {
                Debug.LogError("SpriteRenderer component not found on tile GameObject!");
            }
        }
        else
        {
            Debug.LogError("Tile GameObject not found at coordinates: " + coordinates);
        }
    }

    public GameObject GetTileAtCoordinates(Vector2Int coordinates)
    {
        GameObject tile = GameObject.Find($"Tile_{coordinates.x}_{coordinates.y}");
        if (tile == null)
        {
            Debug.LogWarning($"Tile not found at coordinates: {coordinates}");
        }
        return tile;
    }

    public void StartMazeSolvingWithDFS()
    {
        if (startTileSet && targetTileSet)
        {
            GameObject startTile = GetTileAtCoordinates(startCoordinates);
            GameObject targetTile = GetTileAtCoordinates(targetCoordinates);

            if (startTile != null && targetTile != null)
            {
                DFSAlgorithm dfsAlgorithm = GetComponent<DFSAlgorithm>();
                if (dfsAlgorithm != null)
                {
                    List<GameObject> shortestPath = dfsAlgorithm.FindShortestPath(startTile, targetTile);
                    if (shortestPath != null)
                    {
                        dfsAlgorithm.HighlightPath(shortestPath);
                        Debug.Log("Maze solving algorithm completed.");
                    }
                }
                else
                {
                    Debug.LogError("DFSAlgorithm component not found!");
                }
            }
            else
            {
                Debug.LogError("Start or target tile not found!");
            }
        }
        else
        {
            Debug.LogError("Start or target tile is not set!");
        }
    }

    public void StartAIAgents() // for DFS
    {
        int numberOfAgents = int.Parse(dfsAgentCountInput.text);

        if (dfsAlgorithm.computedPath != null && dfsAlgorithm.computedPath.Count > 0)
        {
            GameObject startTile = GetTileAtCoordinates(startCoordinates);
            if (startTile != null)
            {
                Vector3 startPosition = startTile.transform.position;

                for (int i = 0; i < numberOfAgents; i++)
                {
                    GameObject agent = Instantiate(dfsAgentPrefab, startPosition, Quaternion.identity);
                    DFSAgentMovement agentMovement = agent.GetComponent<DFSAgentMovement>();

                    if (agentMovement != null)
                    {
                        agentMovement.SetPath(dfsAlgorithm.computedPath);
                        agentMovement.moveSpeed = dfsUIManager.GetMoveSpeed();
                        agentMovement.avoidanceRadius = dfsUIManager.GetAvoidanceRadius();
                        agentMovement.avoidanceStrength = dfsUIManager.GetAvoidanceStrength();
                        agentMovement.initialPositionOffset = dfsUIManager.GetInitialPositionOffset();
                    }
                    else
                    {
                        Debug.LogError("DFSAgentMovement component missing on the prefab!");
                    }
                }
            }
            else
            {
                Debug.LogError("Start tile not found!");
            }
        }
        else
        {
            Debug.LogError("No valid path available for agents!");
        }
    }

    private Vector3 GetStartTilePosition()
    {
        GameObject startTile = GetTileAtCoordinates(startCoordinates);
        return startTile != null ? startTile.transform.position : Vector3.zero;
    }

 //   public void StartDFSAgentMovement(List<GameObject> path)
 //   {
 //       if (dfsAgentPrefab == null)
 //       {
 //           Debug.LogError("DFS Agent Prefab not assigned!");
 //           return;
 //       }

//        // Instantiate the DFS agent and set its path
//        GameObject agentInstance = Instantiate(dfsAgentPrefab, GetStartTilePosition(), Quaternion.identity);
//        dfsAgentMovement = agentInstance.GetComponent<DFSAgentMovement>();
//        if (dfsAgentMovement != null)
//        {
//            dfsAgentMovement.SetPath(path);
//        }
//        else
//        {
//            Debug.LogError("DFSAgentMovement component not found on the agent prefab!");
//        }
//    }

    public void StartMazeSolvingwithBFS()
    {
        Debug.Log($"Start Coordinates: {startCoordinates}");
        Debug.Log($"Target Coordinates: {targetCoordinates}");
        if (startTileSet && targetTileSet)
        {
            GameObject startTile = GetTileAtCoordinates(startCoordinates);
            GameObject targetTile = GetTileAtCoordinates(targetCoordinates);

            if (startTile != null && targetTile != null)
            {
                BFSAlgorithm bfsAlgorithm = GetComponent<BFSAlgorithm>(); 
                if (bfsAlgorithm != null)
                {
                    List<GameObject> shortestPath = bfsAlgorithm.FindShortestPath(startTile, targetTile);
                    if (shortestPath != null)
                    {
                        bfsAlgorithm.HighlightPath(shortestPath);
                        Debug.Log("Maze solving algorithm completed.");
                    }
                }
                else
                {
                    Debug.LogError("BFSAlgorithm component not found!");
                }
            }
            else
            {
                Debug.LogError("Start or target tile not found!");
            }
        }
        else
        {
            Debug.LogError("Start or target tile is not set!");
        }
    }

    public void StartAIAgentsWithBFS()
    {
        int numberOfAgents = int.Parse(bfsAgentCountInput.text);

        if (bfsAlgorithm.computedPath != null && bfsAlgorithm.computedPath.Count > 0)
        {
            GameObject startTile = GetTileAtCoordinates(startCoordinates);
            if (startTile != null)
            {
                Vector3 startPosition = startTile.transform.position;

                for (int i = 0; i < numberOfAgents; i++)
                {
                    GameObject agent = Instantiate(bfsAgentPrefab, startPosition, Quaternion.identity);
                    BFSMovement agentMovement = agent.GetComponent<BFSMovement>();

                    if (agentMovement != null)
                    {
                        agentMovement.SetPath(bfsAlgorithm.computedPath);
                        agentMovement.moveSpeed = bfsUIManager.GetMoveSpeed();
                        agentMovement.avoidanceRadius = bfsUIManager.GetAvoidanceRadius();
                        agentMovement.avoidanceStrength = bfsUIManager.GetAvoidanceStrength();
                        agentMovement.initialPositionOffset = bfsUIManager.GetInitialPositionOffset();
                    }
                    else
                    {
                        Debug.LogError("BFSMovement component missing on the prefab!");
                    }
                }
            }
            else
            {
                Debug.LogError("Start tile not found!");
            }
        }
        else
        {
            Debug.LogError("No valid path available for agents!");
        }
    }

    public void StartBFSAgentMovement(List<GameObject> path)
    {
        if (bfsAgentPrefab == null)
        {
            Debug.LogError("bfs Agent Prefab not assigned!");
            return;
        }

        GameObject agentInstance = Instantiate(bfsAgentPrefab, GetStartTilePosition(), Quaternion.identity);
        bfsMovement = agentInstance.GetComponent<BFSMovement>();
        if (bfsMovement != null)
        {
            bfsMovement.SetPath(path);
        }
        else
        {
            Debug.LogError("BFSMovement component not found on the agent prefab!");
        }
    }

    public void StartMazeSolvingWithDijkstra()
    {
        Debug.Log($"Start Coordinates: {startCoordinates}");
        Debug.Log($"Target Coordinates: {targetCoordinates}");

        if (startTileSet && targetTileSet)
        {
            GameObject startTile = GetTileAtCoordinates(startCoordinates);
            GameObject targetTile = GetTileAtCoordinates(targetCoordinates);

            if (startTile != null && targetTile != null)
            {
                if (dijkstraAlgorithm != null) 
                { 

                    dijkstraAlgorithm.SetGridTiles(gridTiles); 
                    List<GameObject> path = dijkstraAlgorithm.SolveMaze(this, gridTiles, startTile, targetTile);

                    if (path != null && path.Count > 0)
                    {
                        dijkstraAlgorithm.HighlightPath(path);
                        StartAIAgentsWithDijkstra(path);
                    }
                    else
                    {
                        Debug.LogError("No path found.");
                    }
                }
                else
                {
                    Debug.LogError("DijkstraAlgorithm component is missing!");
                }
            }
            else
            {
                Debug.LogError("Start or target tile not found");
            }
        }
        else
        {
            Debug.LogError("Start or target tile is not set");
        }
    }

    private void StartAIAgentsWithDijkstra(List<GameObject> path)
    {
        int numberOfAgents = int.Parse(dijkstraAgentCountInput.text);

        if (path != null && path.Count > 0)
        {
            GameObject startTile = GetTileAtCoordinates(startCoordinates);
            if (startTile != null)
            {
                Vector3 startPosition = startTile.transform.position;

                for (int i = 0; i < numberOfAgents; i++)
                {
                    GameObject agent = Instantiate(dijkstraAgentPrefab, startPosition, Quaternion.identity);
                    DijkstraMovement agentMovement = agent.GetComponent<DijkstraMovement>();

                    if (agentMovement != null)
                    {
                        agentMovement.SetPath(path);
                        agentMovement.moveSpeed = dijkstraUIManager.GetMoveSpeed();
                        agentMovement.avoidanceRadius = dijkstraUIManager.GetAvoidanceRadius();
                        agentMovement.avoidanceStrength = dijkstraUIManager.GetAvoidanceStrength();
                        agentMovement.initialPositionOffset = dijkstraUIManager.GetInitialPositionOffset();
                    }
                    else
                    {
                        Debug.LogError("DijkstraMovement component missing on the prefab!");
                    }
                }
            }
            else
            {
                Debug.LogError("Start tile not found!");
            }
        }
        else
        {
            Debug.LogError("No valid path available for agents!");
        }
    }
}
