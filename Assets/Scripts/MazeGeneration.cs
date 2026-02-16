using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Object References")]
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject key_prefab;
    [SerializeField] private GameObject gate_prefab;

    #region book-keeping
    private int[,] floor_map;
    [SerializeField] public Dictionary<int, List<int[]>> room_assignments;
    [SerializeField] public int num_rooms;
    private bool keyNotPlaced = true;
    #endregion

    void Start()
    {
        BeginFloorPlanning();
    }
    // called by MeshGenerator to get the floor plan
    public void BeginFloorPlanning() {
        UnityEngine.Random.InitState(random_seed);
        floor_map = new int[width, length];
        PlaceFloors();
        SmoothFloors();
        ConnectRooms();
        DrawMaze();
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
    
    //connect walls between empty rooms
    private void BreakWalls() {
        //we connect all rooms to the largst room
        int largestRoom = GetLargestRoom();
        for (int i = 0; i <= room_assignments.Count; i++) {
            if (!room_assignments.ContainsKey(i) || i == largestRoom) {
                continue;
            }
            //select a random cell from the largest room b/c idk nearest neighbor algorithms
            int randomSourceIndex = UnityEngine.Random.Range(0, room_assignments[largestRoom].Count);
            int[] source = room_assignments[largestRoom][randomSourceIndex];
            //select a random cell from the room we're connecting to
            int randomTargetIndex = UnityEngine.Random.Range(0, room_assignments[i].Count);
            int[] target = room_assignments[i][randomTargetIndex];
            room_assignments[i].Remove(target); //remove this cell so we don't select it later
            //"blast open walls"
            CarveHallway(source, target);
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
    private void CarveHallway(int[] source, int[] target) {
        int endX = target[0];
        //if the target is further left, carve to the left
        if (target[0] - source[0] < 0) {
            for (int x = source[0]; x >= target[0]; x--) {
                room_assignments[floor_map[x, source[1]]].Remove(new int[] {x, source[1]});
                floor_map[x, source[1]] = floor_map[source[0], source[1]];
                room_assignments[floor_map[source[0], source[1]]].Add(new int[] {x, source[1]});
            }
        } 
        //otherwise, carve to the right
        else {
             for (int x = source[0]; x <= target[0]; x++) {
                room_assignments[floor_map[x, source[1]]].Remove(new int[] {x, source[1]});
                floor_map[x, source[1]] = floor_map[source[0], source[1]];
                room_assignments[floor_map[source[0], source[1]]].Add(new int[] {x, source[1]});
            }
        }
        //if the target is further down, carve down
        if (target[1] - source[1] < 0) {
            for (int y = source[1]; y >= target[1]; y--) {
                room_assignments[floor_map[endX, y]].Remove(new int[] {endX, y});
                floor_map[endX, y] = floor_map[source[0], source[1]];
                room_assignments[floor_map[source[0], source[1]]].Add(new int[] {endX, y});
            }
        } 
        //otherwise, carve upwards
        else {
            for (int y = source[1]; y <= target[1]; y++) {
                room_assignments[floor_map[endX, y]].Remove(new int[] {endX, y});
                floor_map[endX, y] = floor_map[source[0], source[1]];
                room_assignments[floor_map[source[0], source[1]]].Add(new int[] {endX, y});
            }
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
       for (int row = 0; row < width; row++)
       {
            for (int col = 0; col < length; col++)
            {   
                GameObject curTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                curTile.name = "[" + row + ", " + col + "]";

                curTile.transform.position = new Vector3 (row, tileSize / 2f, col);
                Renderer rend = curTile.GetComponent<Renderer>();
                if (floor_map[row, col] == -1) {
                    rend.material.color = Color.red;
                    curTile.transform.localScale = new Vector3 (tileSize, tileSize * 2f, tileSize);
                } else
                {
                    curTile.transform.localScale = new Vector3 (tileSize, 0.25f, tileSize);
                    rend.material.color = Color.blue;
                    //if there aren't any walls around, attempt to spawn a treasure chest
                    if (keyNotPlaced && IsOpenSpace(row, col) && UnityEngine.Random.Range(0, 1f) < key_chance) {
                        Vector3 pos = new Vector3(row + tileSize * 0.5f, 1f, col + tileSize * 0.5f); //tile center
                        Instantiate(key_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                        keyNotPlaced = false;
                    } else if (IsOpenSpace(row, col))
                    {
                        key_chance *= 3f;
                    }
                }

                if (row == goalX && col == 0)
                {
                    curTile.transform.localScale = new Vector3 (tileSize, 0.25f, tileSize);
                    rend.material.color = Color.green;
                } else if (row == goalX && col == length - 1)
                {
                    curTile.transform.localScale = new Vector3 (tileSize, 0.25f, tileSize);
                    rend.material.color = Color.green;
                    Vector3 pos = new Vector3(row, 0.25f, col - tileSize * 0.5f); //tile center
                    Instantiate(gate_prefab, pos, Quaternion.LookRotation(Vector3.forward));
                }
                curTile.transform.SetParent(this.gameObject.transform); 
            }
        } 
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
}