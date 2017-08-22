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

    private void Start () {
        
        Debug.Log ("S - Set Start Pos");
        Debug.Log ("G - Set Goal Pos");
        Debug.Log ("M - Generate new Map");
        Debug.Log ("Space - Search");

    }

    private void Update () {
        //search, set goal and start
        if (Input.GetKeyDown (KeyCode.G)) {
            Debug.Log ("Setting Goal Node. Select the desired goal node.");
        }
        else if (Input.GetKeyDown (KeyCode.S)) {
            Debug.Log ("Setting Start Node. Select the desired start node.");
        }
    }
}
