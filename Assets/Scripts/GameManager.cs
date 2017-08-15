using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //calls map generator according to option selected in the menu
    //gets back the nodes map created by the mapgenerator
    //calls the search algorythm and sends the map over to it

    [SerializeField]
    private MapGenerator mapGenerator;

    [SerializeField]
    private SearchManager searchManager;

    private bool isWaitingMaxPaths = false;

    private void Start () {
        mapGenerator.Generate ();

        Debug.Log ("1 - BrittishMuseum");
        Debug.Log ("2 - BFS");
        Debug.Log ("3 - DFS");
        Debug.Log ("4 - Hill Climbing");
        Debug.Log ("5 - Beam");
        Debug.Log ("6 - Branch and Bound");
        Debug.Log ("7 - A*");

        Debug.Log ("S - Set Start Pos");
        Debug.Log ("G - Set Goal Pos");
        Debug.Log ("M - Generate new Map");
        Debug.Log ("Space - Search");

    }

    private void Update () {
        //search, set goal and start
        if (Input.GetKeyDown (KeyCode.Space)) {
            Debug.Log ("Showing the way.");
            searchManager.StartPathing (mapGenerator.Size, mapGenerator.RandomNode (), mapGenerator.RandomNode (), true);
        }
        else if (Input.GetKeyDown (KeyCode.G)) {
            Debug.Log ("Setting Goal Node. Select the desired goal node.");
        }
        else if (Input.GetKeyDown (KeyCode.S)) {
            Debug.Log ("Setting Start Node. Select the desired start node.");
        }
        else if (Input.GetKeyDown (KeyCode.M)) {
            Debug.Log ("Generating another map with the same parameters.");
            mapGenerator.Generate ();
        }
    }
}
