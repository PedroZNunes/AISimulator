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
        mapGenerator.Generate ();

        Debug.Log ("1 - BrittishMuseum");
        Debug.Log ("2 - BFS");
        Debug.Log ("3 - DFS");
        Debug.Log ("4 - Hill Climbing");
        Debug.Log ("5 - Beam");
        Debug.Log ("6 - A*");
        Debug.Log ("7 - Branch and Bound");

    }

    private void Update () {
        GetInput ();
    }

    private void GetInput () {
        //setting up algorythm
        if (Input.GetKeyDown (KeyCode.Alpha1)) {
            Debug.Log ("Algorythm set to BrittishMuseum");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha2)) {
            searchManager.SetAlgorythm (new BFS ());
            Debug.Log ("Algorythm set to BFS");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha3)) {
            Debug.Log ("Algorythm set to DFS");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha4)) {
            Debug.Log ("Algorythm set to HillClimbing");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha5)) {
            Debug.Log ("Algorythm set to Beam");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha6)) {
            Debug.Log ("Algorythm set to A*");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha7)) {
            Debug.Log ("Algorythm set to Branch and Bound");
        }
        //search, set goal and start
        else if (Input.GetKeyDown (KeyCode.Space)) {
            Debug.Log ("SearchInput Detected.");
            searchManager.StartPathing (mapGenerator.nodes , mapGenerator.Size, mapGenerator.RandomNode (), mapGenerator.RandomNode(), true);
        }
        else if (Input.GetKeyDown (KeyCode.G)) {
            Debug.Log ("Setting Goal Node. Select the desired goal node.");
        }
        else if (Input.GetKeyDown (KeyCode.S)) {
            Debug.Log ("Setting Start Node. Select the desired start node.");
        }

    }

}
