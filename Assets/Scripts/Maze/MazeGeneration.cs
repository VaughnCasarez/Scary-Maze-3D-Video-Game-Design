using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

//Author: Arshiya Rahman
//This script runs Cellular Automata rules to generate a floor plan
//this is also a slow script, I couldn't figure out optimizations :(
public class MazeGeneration : MonoBehaviour
{
    [Header("Generation Variables")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float wall_chance = 0.3f;
    [SerializeField] public int random_seed = 1;
    [SerializeField] private int iterations = 10;

    [SerializeField] public int width = 25;
    [SerializeField] public int length = 25;
    [SerializeField] public int goalX = 13;
    [SerializeField] private float key_chance = 0.005f;
    [SerializeField] private float pumpkin_chance = 0.05f;
    [SerializeField] private float scarecrow_chance = 0.05f;
    [SerializeField] private float candy_chance = 0.005f;
    [SerializeField] private int maxPumpkins = 3;
    [SerializeField] private int maxScarecrows = 3;
    [SerializeField] private float minSpacingDistance = 5f;

    [Header("Object References")]
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject grass_prefab;
    [SerializeField] private GameObject corn_prefab;
    [SerializeField] private GameObject corn_with_lantern_prefab;
    [SerializeField] private GameObject key_prefab;
    [SerializeField] private GameObject gate_prefab;
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject pumpkin_prefab;
    [SerializeField] private GameObject timer_candy_prefab;
    [SerializeField] private GameObject extra_life_candy_prefab;
    [SerializeField] private GameObject bullet_reload_candy_prefab;
    [SerializeField] private GameObject scarecrow_prefab;
    public GameObject winBox;
    public GameObject player;
    public GameObject key;
    #region book-keeping
    private int[,] floor_map;
    private Dictionary<int, List<int[]>> room_assignments;
    private List<int[]> placed_object_positions;
    private int num_rooms;
    private bool keyNotPlaced = true;
    private bool timerCandyPlaced = false;
    private bool lifeCandyPlaced = false;
    private bool bulletCandyPlaced = false;
    private int numPumpkins = 0;
    private int numScarecrows = 0;
    public Action MazeGenerated;
  
    #endregion

    public void BeginMaze(int seed)
    {
        random_seed = seed;
        BeginFloorPlanning();
        MazeGenerated?.Invoke();
    }
    // called by MeshGenerator to get the floor plan
    public void BeginFloorPlanning() {
        UnityEngine.Random.InitState(random_seed);
        floor_map = new int[width, length];
        PlaceFloors();
        SmoothFloors();
        ConnectRooms();
        DrawMaze();
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    //initial placements of walls and floors
    //wall = -1
    //floor = anything else
    private void PlaceFloors() {
        //place walls on the borders
        for (int r = 0; r < width; r++)
        {
            floor_map[r, 0] = -1;
            floor_map[r, length - 1] = -1;
        }

        for (int c = 0; c < length; c++)
        {
            floor_map[0, c] = -1;
            floor_map[width - 1, c] = -1;
        }

        //randomly place walls on the inside
        for (int r = 2; r < width - 2; r++)
        {
            for (int c = 2; c < length - 2; c++)
            {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < wall_chance)
                {
                    floor_map[r, c] = -1;
                }
            }
        }
    }

    //calls Automate a certain number of times to smooth walls and connect rooms
    private void SmoothFloors() {
        for (int i = 0; i < iterations; i++)
        {
            Automate();
        }
    }

    //once rooms are finalized, determine what cells belong to what room and connect them
    private void ConnectRooms() {
        var dfs_results = RunDFS();
        room_assignments = dfs_results.Groups;
        num_rooms = dfs_results.RoomCount;
        BreakWalls();
    }

    //the actual cellular automata step
    private void Automate() {
        //the new state
        int[,] new_floor = new int[width, length];
        for (int r = 2; r < width - 2; r++)
        {
            for (int c = 2; c < length - 2; c++)
            {
                int neighboring_walls = CountNeighboringWalls(r, c);
                //walls with 4 or more neighboring walls stay walls
                if (floor_map[r, c] == -1 && neighboring_walls >= 4)
                {
                    new_floor[r, c] = -1;
                }
                //anything with 5 or more neighboring walls become walls
                else if (neighboring_walls >= 5)
                {
                    new_floor[r, c] = -1;
                } 
                //everything else is a floor
                else {
                    new_floor[r, c] = 0;
                }
            }
        }

        //deep copy the new state
        for (int r = 2; r < width - 2; r++)
        {
            for (int c = 2; c < length - 2; c++)
            {
                floor_map[r, c] = new_floor[r, c];
            }
        }
        

    }

    //how many walls are there in the 3x3 space centerd at x, y?
    private int CountNeighboringWalls(int x, int y) {
        int num = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int nx = x + i;
                int ny = y + j;
                if (nx == x && ny == y)
                {
                    continue;
                }

                if (OutOfBounds(nx, ny))
                {
                    num++;
                    continue;
                }
                if (floor_map[nx, ny] == -1)
                {
                    num++;
                }
            }
        }
        
        return num;
    }

    //use DFS-like algorithm to check connectivity between rooms
    private (Dictionary<int, List<int[]>> Groups, int RoomCount) RunDFS() {
        //basic DFS please don't make me explain 
        int cur_id = 0;
        for (int r = 1; r < width - 1; r++) {
            for (int c = 1; c < length - 1; c++) {
                if (floor_map[r, c] == 0) {
                    cur_id++;
                    Search(r, c, cur_id);
                }
            }
        }
        //we put the room assignments into a dictionary with <room key, associated cells> for easier access later on
        Dictionary<int, List<int[]>> groups = new Dictionary<int, List<int[]>>();
        for (int r = 0; r < width; r++) {
            for (int c = 0; c < length; c++) {
                int[] coords = new int[2];
                coords[0] = r;
                coords[1] = c;
                if (groups.ContainsKey(floor_map[r, c])) {
                    groups[floor_map[r, c]].Add(coords);
                } else {
                    List<int[]> rooms = new List<int[]>();
                    groups.Add(floor_map[r, c], rooms);
                    groups[floor_map[r, c]].Add(coords);
                }
            }
        }
        return (groups, cur_id);
    }
    //DFS helper pls don't make me explain
    private void Search(int x, int y, int cur_id) {
        if (floor_map[x, y] == 0) {
            floor_map[x, y] = cur_id;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (i == 0 && j == 0) {
                        continue;
                    }
                    int nx = x + i;
                    int ny = y + j;
                    
                    if (OutOfBounds(nx, ny))
                    {
                        continue;
                    }
                    if (floor_map[nx, ny] == 0) {
                        Search(nx, ny, cur_id);
                    }
                        
                }
            }
        }
    }
    
    //connect walls between empty rooms until all rooms are accessible
    private void BreakWalls()
    {
        // Keep reconnecting until there's only one room left
        var dfs = RunDFS();
        room_assignments = dfs.Groups;
        num_rooms = dfs.RoomCount;

        int largestRoom = GetLargestRoom();
        int maxIterations = width * length; 
        int iter = 0;

        while (num_rooms > 1 && iter++ < maxIterations)
        {
            largestRoom = GetLargestRoom();

            // Find any room that isn't the largest
            int targetRoom = -1;
            foreach (var room in room_assignments)
            {
                if (room.Key <= 0 || room.Key == largestRoom) {
                    continue;
                }
                if (room.Value.Count > 0) {
                    targetRoom = room.Key; 
                    break; 
                }
            }
            if (targetRoom == -1) {
                break;
            }

            // Pick random cells from each and carve between them
            int[] source = room_assignments[largestRoom][UnityEngine.Random.Range(0, room_assignments[largestRoom].Count)];
            int[] target = room_assignments[targetRoom][UnityEngine.Random.Range(0, room_assignments[targetRoom].Count)];

            CarveHallway(source, target);

            // Re-run DFS so room_assignments reflects the carved path
            dfs = RunDFS();
            room_assignments = dfs.Groups;
            num_rooms = dfs.RoomCount;
        }
    }
    private int GetLargestRoom() {
        int maxSize = -1;
        int roomId = -1;
        foreach(var room in room_assignments) {
            if (room.Key == -1) {
                continue;
            }
            if (room.Value.Count > maxSize) {
                maxSize = room.Value.Count;
                roomId = room.Key;
            }
        }
        return roomId;
    }

    //"blast open walls" by carving an L-shaped path
    private void CarveHallway(int[] source, int[] target)
    {
        //where are we starting
        int x = source[0];
        int y = source[1];
        //where are we ending
        int tx = target[0];
        int ty = target[1];

        int stepX = (tx > x) ? 1 : -1;
        //horizontal cut
        while (x != tx)
        {   
            //mark the current square as floor
            if (floor_map[x, y] == -1) {
                floor_map[x, y] = 0; 
            }
            x += stepX;
        }

        //vertical cut
        int stepY = (ty > y) ? 1 : -1;
        while (y != ty)
        {  
            //mark the current square as floor
            if (floor_map[x, y] == -1)
                floor_map[x, y] = 0;
            y += stepY;
        }

        //mark the start as floor
        if (floor_map[x, y] == -1) {
            floor_map[x, y] = 0;
        }
    }

    private bool OutOfBounds(int nx, int ny) {
        return nx < 0 || nx >= width || ny < 0 || ny >= length;
    }

    private bool OutOfBoundsWall(int nx, int ny) {
        return nx <= 0 || nx >= width || ny <= 0 || ny >= length;
    }

    //for demo, "maze" is just colored planes on the floor
    public void DrawMaze()
    {
        placed_object_positions = new();
        for (int row = 0; row < width; row++)
        {
            for (int col = 0; col < length; col++)
            {   
                int[] coords = new int[2];
                coords[0] = row;
                coords[1] = col;
                //add the floor maze tiles and player
                if (row == goalX && col == 0)
                {
                    GameObject curGround = Instantiate(grass_prefab,  new Vector3 (row, tileSize / 2f, col), Quaternion.LookRotation(Vector3.forward));
                    curGround.name = "[" + row + ", " + col + "]";
                    curGround.transform.SetParent(this.gameObject.transform); 
                    curGround.layer = 3;
                    curGround.transform.localScale = new Vector3 (tileSize, 0.25f, tileSize);
                    Vector3 pos = new Vector3(row + tileSize, 1f, col + tileSize); //tile center
                    player = Instantiate(player_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                    placed_object_positions.Add(coords);
                    Debug.Log(player == null);
                } else if (row == goalX && col == length - 1)
                {
                    GameObject curGround = Instantiate(grass_prefab,  new Vector3 (row, tileSize / 2f, col), Quaternion.LookRotation(Vector3.forward));
                    curGround.name = "[" + row + ", " + col + "]";
                    curGround.transform.SetParent(this.gameObject.transform); 
                    curGround.layer = 3;
                    
                    curGround.transform.localScale = new Vector3 (tileSize, 1f, tileSize);
                    Vector3 pos = new Vector3(row, 0.5f, col - tileSize * 0.5f); //tile center
                    GameObject gate = Instantiate(gate_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                    winBox = gate.transform.Find("win box").gameObject;
                    placed_object_positions.Add(coords);
                    continue;
                }
                
                if (floor_map[row, col] == -1) {
                    GameObject curCorn;
                    if ((row == 0 && col % 10 == 0) || (row % 10 == 0 && col == 0))
                    {
                        curCorn = Instantiate(corn_with_lantern_prefab, new Vector3 (row, tileSize / 2f, col), Quaternion.LookRotation(Vector3.forward));
                    } else
                    {
                        curCorn = Instantiate(corn_prefab, new Vector3 (row, tileSize / 2f, col), Quaternion.LookRotation(Vector3.forward));
                    }
                    curCorn.name = "[" + row + ", " + col + "]";
                    curCorn.transform.SetParent(this.gameObject.transform); 
                    curCorn.layer = 3;
                } else
                {
                    //add the ground
                    GameObject curGround = Instantiate(grass_prefab,  new Vector3 (row, tileSize / 2f, col), Quaternion.LookRotation(Vector3.forward));
                    curGround.name = "[" + row + ", " + col + "]";
                    curGround.transform.SetParent(this.gameObject.transform); 
                    curGround.layer = 3;

                    //add interactable objects
                    if (!CanPlaceHere(row, col))
                    {
                        continue;
                    }
                    if (keyNotPlaced && UnityEngine.Random.Range(0, 1f) < key_chance) {
                        Vector3 pos = new Vector3(row + tileSize, 1f, col + tileSize); //tile center
                        key = Instantiate(key_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                        placed_object_positions.Add(coords);
                        keyNotPlaced = false;
                    } else
                    {
                        key_chance *= 2f;
                    }
                    if (numPumpkins < maxPumpkins && UnityEngine.Random.Range(0, 1f) < pumpkin_chance)
                    {
                        Vector3 pos = new Vector3(row + tileSize * 0.5f, 0.75f, col + tileSize * 0.5f); //tile center
                        Instantiate(pumpkin_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                        placed_object_positions.Add(coords);
                        numPumpkins++;
                    } 
                    else if (numScarecrows < maxScarecrows && UnityEngine.Random.Range(0, 1f) < scarecrow_chance)
                    {
                        Vector3 pos = new Vector3(row + tileSize * 0.5f, 0.75f, col + tileSize * 0.5f); //tile center
                        Instantiate(scarecrow_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                        placed_object_positions.Add(coords);
                        numScarecrows++;
                    } else if (!lifeCandyPlaced && UnityEngine.Random.Range(0, 1f) < candy_chance) {
                        Vector3 pos = new Vector3(row + tileSize, 1.25f, col + tileSize); //tile center
                        Instantiate(extra_life_candy_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                        placed_object_positions.Add(coords);
                        lifeCandyPlaced = true;
                    } else if (!timerCandyPlaced && UnityEngine.Random.Range(0, 1f) < candy_chance) {
                        Vector3 pos = new Vector3(row + tileSize, 1.25f, col + tileSize); //tile center
                        Instantiate(timer_candy_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                        placed_object_positions.Add(coords);
                        timerCandyPlaced = true;
                    } else if (!bulletCandyPlaced && UnityEngine.Random.Range(0, 1f) < candy_chance) {
                        Vector3 pos = new Vector3(row + tileSize, 1.25f, col + tileSize); //tile center
                        Instantiate(bullet_reload_candy_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                        placed_object_positions.Add(coords);
                        bulletCandyPlaced = true;
                    } 
                }
            }
        }
    } 

    private bool CanPlaceHere(int r, int c)
    {
        return IsOpenSpace(r, c) && IsFarAway(r, c);
    }
    //are there walls within a 5x5 centered at x, y?
    private bool IsOpenSpace(int x, int y) {
        int num = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int nx = x + i;
                int ny = y + j;

                if (nx < 0 || nx >= width || ny < 0 || ny >= length)
                {
                    num++;
                    continue;
                }
                if (floor_map[nx, ny] == -1)
                {
                    num++;
                }
            }
        }
        return num <= 1;
    }

    private bool IsFarAway(int r, int c)
    {
        foreach (var existing in placed_object_positions)
        {
            float arg = Mathf.Pow(r - existing[0], 2) + Mathf.Pow(c - existing[1], 2);
            if (Mathf.Sqrt(arg) < minSpacingDistance)
            {
                return false;
            } 
        }
        return true;
    }
}