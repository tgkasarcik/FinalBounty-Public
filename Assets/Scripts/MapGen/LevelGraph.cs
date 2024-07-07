using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelGraph
{
    const string DEFAULT_SCENE = "MainMenu";
    public string sceneToUse {get; set;}
    //stores all edges of a node into a matrix (row is node, column is edges)
    public List<int>[] graph;

    //currently unused as all levels are the same
    private Scene[] levels;

    //TODO: make certain level data persist between loading in new scene through
    //the use of generated level data per level (in order to configure each level)
    //public var levelData;
    public int startingLevel {get; set;}

    public int numLevels {get; set;}
    public int numEdges {get; set;}  


    public LevelGraph()
    {
        this.sceneToUse = DEFAULT_SCENE;
        this.numLevels = 10;
        this.numEdges = 10;
        this.populateGraphVar();
    }

    public LevelGraph(string sceneToUse, int numLevels, int numEdges)
    {
        this.sceneToUse = sceneToUse;
        this.numLevels = numLevels;
        this.numEdges = numEdges;
        this.populateGraphVar();
    }

    private void populateGraphVar()
    {
        this.graph = new List<int>[this.numLevels];

        for(int i = 0; i < this.numLevels; i++)
        {
            this.graph[i] = new List<int>();
        }
    }

    //method to generate complete level graph
    public void generateLevelGraph(int seed)
    {
        //sets seed for random generated content
        System.Random rnd = new System.Random(seed);

        //checks if number of edges is greater than there could possibly be
        //for an undirected graph and culls it if it is
        int maxEdges = (int)((this.numLevels * (this.numLevels - 1)) / 2);
        if(this.numEdges > maxEdges)
        {
            this.numEdges = maxEdges;
        }

        //generates all edges for graph
        for(int i = 0; i < this.numEdges; i++)
        {
            //gets edges
            var from = (int)(rnd.NextDouble() * this.numLevels);
            var to = (int)(rnd.NextDouble() * this.numLevels);

            //makes sure edges arent the same node
            while(from == to)
            {
                to = (int)(rnd.NextDouble() * this.numLevels);
            }

            //adds both nodes to list of connections in the graph
            this.graph[from].Add(to);
            this.graph[to].Add(from);  
        }

        //makes starting level random
        //this.startingLevel = (int)(Random.Range(0, this.numLevels));

        //Tala changed this so that the game starts at the first level
        this.startingLevel = 0;
    }

    public void printConnections()
    {
        int v = 0;
        foreach(var node in this.graph)
        {
            Debug.Log("Connections for node " + v + "...");
            foreach(int connection in node)
            {
                Debug.Log(connection);
            }
            v++;
        }
    }
}
