using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{

    public GenPlanet MyPlanet;
    public GameObject DebugEnemy;


    // Start is called before the first frame update
    void Start()
    {
        System.Random rand = new System.Random();
        Searcher searcher = new Searcher();
        Cell[] cells = MyPlanet.Grid.GetCells();
        int randNum = rand.Next(cells.Length - 1);
        Cell endCell = cells[randNum];
        //Debug.Log("Randnum is: " + randNum);
        Cell startCell = cells[0];



        //searcher.RefreshPath(startCell, endCell);

        SpawnEnemy test = new SpawnEnemy(startCell, DebugEnemy, MyPlanet);

    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
