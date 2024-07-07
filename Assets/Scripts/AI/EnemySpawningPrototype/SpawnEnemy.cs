using PlanetGen;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SpawnEnemy
{


    private readonly GameObject EnemyObject;
    private GenPlanet MyPlanet;

    public SpawnEnemy(Cell location, GameObject prefab, GenPlanet myPlanet)
    {

        //Debug.Log("TRYING TO SPAWN ENEMYT");

        this.MyPlanet = myPlanet;

        prefab.SetActive(false);

        this.EnemyObject = Transform.Instantiate(prefab, location.getLocation(), Quaternion.identity);

        //Debug.Log("Is myplanet working in the spawner??? " + MyPlanet + " [END]");

        Debug.Log("Did we get the inerface? " + this.EnemyObject.GetComponent<IEnemy>() + " END");

        this.EnemyObject.GetComponent<IEnemy>().SetStartCell(location);
        this.EnemyObject.GetComponent<IEnemy>().SetStartPlanet(this.MyPlanet);

        //foreach (MonoBehaviour component in this.EnemyObject.GetComponents<MonoBehaviour>())
        //{
        //    Debug.Log("Compnent is: " + component.name);

        //    if (component is IEnemy enemy)
        //    {

        //        Debug.Log("No way this actually works");
        //        enemy.SetStartCell(location);
        //        enemy.SetStartPlanet(this.MyPlanet);

        //    }
        //}


        //EnemyMovement2 moveScript = this.EnemyObject.GetComponent<EnemyMovement2>();

        //moveScript.ChangeGoal(location.getLocation());

        //Debug.Log("Successfully changed goal");


        //List<Cell> navPath = GetPath();

        //Debug.Log("Inside spawn enemy, raw navpath is: " + navPath + " END");


        this.EnemyObject.SetActive(true);




    }


    public GameObject GetInstantiatedPrefab()
    {
        return this.EnemyObject;
    }




    private List<Cell> GetPath()
    {

        System.Random rand = new System.Random();
        Searcher searcher = new Searcher();
        Cell[] cells = this.MyPlanet.Grid.GetCells();
        int randNum = rand.Next(cells.Length - 1);
        Cell endCell = cells[randNum];
        //Debug.Log("Randnum is: " + randNum);
        Cell startCell = cells[0];



        return searcher.RefreshPath(startCell, endCell);
    }





}
