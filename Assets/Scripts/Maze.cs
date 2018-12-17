
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    //the width of the maze (it will be a square)
    public static readonly int width = 24; //should be an even number due to the fact that every tile visited by the algorithm must be surrounded by walls/passages

    public Wall wallPrefab;

    public Passage passagePrefab;

    public StartRoom startRoomPrefab;

    public EndRoom endRoomPrefab;

    public Cat catPrefab;

    private StartRoom startRoom;

    private EndRoom endRoom;

    private Cat cat;

    private bool[,] grid; //the cells of the maze

    private Tile[,] maze;

    private struct pair
    {
        public int a;
        public int b;

        public pair(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }

    public void Generate()
    {

        grid = new bool[width+1, width+1];
        maze = new Tile[width+1, width+1];

        List<pair> frontiers = new List<pair>(); //cells to visit

        //starting tile
        int pos = width / 2 + 1;
        grid[pos, pos] = true;
        //frontiers.Add(new pair(1, width - 3));
        //frontiers.Add(new pair(3, width - 1));

        frontiers.Add(new pair(pos - 2, pos));
        frontiers.Add(new pair(pos + 2, pos));
        frontiers.Add(new pair(pos, pos - 2));
        frontiers.Add(new pair(pos, pos + 2));

        while (frontiers.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, frontiers.Count); //get random cell from list
            pair coords = frontiers[index];

            List<pair> neighbors = getNeighbors(coords.a, coords.b); //get random neighbor
            int index2 = UnityEngine.Random.Range(0, neighbors.Count);
            //Debug.Log(index2);
            pair neighbor = neighbors[index2];

            int dx = (neighbor.a - coords.a) / 2;
            int dy = (neighbor.b - coords.b) / 2;

            //Debug.Log("dx: " + dx);
            //Debug.Log("dy: " + dy);

            grid[coords.a, coords.b] = true;
            grid[coords.a + dx, coords.b + dy] = true; //passage between selected frontier cell and selected neighbor

            if (valid(coords.a + 2, coords.b)) //add chosen cell's frontier cells
            {
                if (!grid[coords.a + 2, coords.b])
                {
                    //Debug.Log(grid[coords.a + 2, coords.b]);

                    pair p1 = new pair(coords.a + 2, coords.b);
                    if (!frontiers.Contains(p1))
                    {
                        frontiers.Add(new pair(coords.a + 2, coords.b));
                    }
                }
            }
            if (valid(coords.a, coords.b + 2))
            {
                if (!grid[coords.a, coords.b + 2])
                {
                    // Debug.Log(grid[coords.a, coords.b + 2]);

                    pair p2 = new pair(coords.a, coords.b + 2);
                    if (!frontiers.Contains(p2))
                    {
                        frontiers.Add(new pair(coords.a, coords.b + 2));
                    }
                }
            }
            if (valid(coords.a - 2, coords.b))
            {
                if (!grid[coords.a - 2, coords.b])
                {
                    //  Debug.Log(grid[coords.a - 2, coords.b]);

                    pair p3 = new pair(coords.a - 2, coords.b);
                    if (!frontiers.Contains(p3))
                    {
                        frontiers.Add(new pair(coords.a - 2, coords.b));
                    }
                }
            }
            if (valid(coords.a, coords.b - 2))
            {
                if (!grid[coords.a, coords.b - 2])
                {
                    // Debug.Log(grid[coords.a, coords.b - 2]);

                    pair p4 = new pair(coords.a, coords.b - 2);
                    if (!frontiers.Contains(p4))
                    {
                        frontiers.Add(new pair(coords.a, coords.b - 2));
                    }
                }
            }

            frontiers.Remove(coords); //remove chosen cell from list of frontier cells
        }

        //positions of start and end rooms (along top and bottom rows)
        int start = 2 * UnityEngine.Random.Range(1, width / 2) - 1;
        int end = 2 * UnityEngine.Random.Range(1, width / 2) - 1;

        grid[0, start] = true; //passage from start room to maze
        grid[width, end] = true; //passage from maze to end room (use width not width-1 because grid is actually width+1 wide)

        startRoom = Instantiate(startRoomPrefab) as StartRoom;
        startRoom.name = "Start Room";
        startRoom.transform.parent = transform;
        startRoom.transform.localPosition = new Vector3(0f, 0f, 0f);

        endRoom = Instantiate(endRoomPrefab) as EndRoom;
        endRoom.name = "End Room";
        endRoom.transform.parent = transform;
        endRoom.transform.localPosition = new Vector3(StartRoom.WIDTH / 2 + width + 2f + EndRoom.WIDTH / 2, 0, end + 1f - EndRoom.WIDTH / 2 - start + 1f);

        int catX = 2 * UnityEngine.Random.Range(1, width / 2 + 1) - 3;
        int catZ = 2 * UnityEngine.Random.Range(1, width / 2 + 1) - 3;

        cat = Instantiate(catPrefab) as Cat;
        cat.name = "Cat";
        cat.transform.parent = transform;
        cat.transform.localPosition = new Vector3(catX + 1f + StartRoom.WIDTH/2, 0f, catZ + 1f - start + 1f);

        for (int z = 0; z < width+1; ++z)
        {
            for(int x = 0; x < width+1; ++x)
            {
                if (grid[x, z]) //if a passage
                {
                    Passage pass = Instantiate(passagePrefab) as Passage;
                    maze[x, z] = pass;
                    pass.name = "Tile " + x + ", " + z;
                    pass.transform.parent = transform;
                    pass.transform.localPosition = new Vector3(x + 1f + StartRoom.WIDTH/2, 0f, z - start); //want tile at (1,1) to be at position x = 0 z = 0 so player starts in top left
                }
                else //else a wall
                {
                    Wall wall = Instantiate(wallPrefab) as Wall;
                    maze[x, z] = wall;
                    wall.name = "Tile " + x + ", " + z;
                    wall.transform.parent = transform;
                    wall.transform.localPosition = new Vector3(x + 1f + StartRoom.WIDTH/2, 0f, z - start); //want tile at (1,1) to be at position x = 0 z = 0 so player starts in top left
                }
            }
        }

    }

    private bool valid(int x, int z) //returns true if the given coordinates represent a valid position within the grid
    {
        return ((x >= 0) && (x < width)) && ((z >= 0) && (z < width));
    }

    private List<pair> getNeighbors(int x, int z)
    {
        List<pair> neighbors = new List<pair>();

        if(valid(x + 2, z))
        {
            if(grid[x + 2, z])
            {
                neighbors.Add(new pair(x + 2, z));
            }
        }
        if (valid(x, z + 2))
        {
            if (grid[x, z + 2])
            {
                neighbors.Add(new pair(x, z + 2));
            }
        }
        if (valid(x - 2, z))
        {
            if (grid[x - 2, z])
            {
                neighbors.Add(new pair(x - 2, z));
            }
        }
        if (valid(x, z - 2))
        {
            if (grid[x, z - 2])
            {
                neighbors.Add(new pair(x, z - 2));
            }
        }

        return neighbors;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
