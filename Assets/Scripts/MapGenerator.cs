using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    private int size = 0;

    [SerializeField]
    private float grain = 0;

    private List<int> sizeList;

    [SerializeField]
    private int maxNodes = 10;

    private float[,] grid;
    private bool[,] nodesGrid;

    private List<Node> nodesList;

    private void Awake () {
        sizeList = new List<int> ();
        sizeList.Add (5);
        sizeList.Add (9);
        sizeList.Add (17);
        sizeList.Add (33);
        sizeList.Add (65);
        sizeList.Add (129);
        sizeList.Add (257);
        sizeList.Add (513);
        sizeList.Add (1025);
        sizeList.Add (2049);

        bool validSize = false;
        for (int i = 0 ; i < sizeList.Count ; i++) {
            if (size == sizeList[i]) {
                validSize = true;
            }
        }
        if (!validSize) {
            Debug.LogError ("Invalid Size.");
        }
    }

    private void Start () {
        ResetBase ();
        TriggerDiamond ();
        SetNodes ();
    }

    public void TriggerDiamond () {
        Diamond ();
    }

    private void Diamond () {

        float corner1 = Random.value;
        float corner2 = Random.value;
        float corner3 = Random.value;
        float corner4 = Random.value;
        
        grid[0 , 0] = corner1;
        grid[size - 1 , 0] = corner2;
        grid[size - 1 , size - 1] = corner3;
        grid[0 , size - 1] = corner4;

        DivideGrid (0 , 0 , size , corner1 , corner2 , corner3 , corner4);

        StringBuilder sb;
        sb = new StringBuilder ();
        for (int rows = 0 ; rows < size ; rows++) {
            for (int cols = 0 ; cols < size ; cols++) {
                sb.AppendFormat ( "{0:0.0}  ", grid[cols , rows]);
            }
            sb.AppendLine ();
        }
        Debug.Log (sb);
        Debug.Log ("end of grid");
    }

    private float DisplaceMiddle ( float num ) {
        float max = num / (2 * size) * grain;
        return Random.Range (-0.5f , 0.5f) * max;
    }

    private void DivideGrid ( int x , int y , int currentSize , float corner1 , float corner2 , float corner3 , float corner4 ) {
        int newSize = currentSize / 2;
        //Debug.LogFormat ("dividing grid [{0}, {1}] of size {2}. halfSize {3}" , x , y, currentSize, newSize);

        if (currentSize > 2){

            //Random the displacement while making sure the middle does not get displaced out of bounds
            float middle = Mathf.Clamp01 (( corner1 + corner2 + corner3 + corner4 ) / 4 + DisplaceMiddle ( 2 * newSize));

            float edge1 = ( corner1 + corner2 ) / 2; //Calculate the edges by averaging the two corners of each edge.
            float edge2 = ( corner2 + corner3 ) / 2;
            float edge3 = ( corner3 + corner4 ) / 2;
            float edge4 = ( corner4 + corner1 ) / 2;

            //middle
            grid[x + newSize , y + newSize] = middle;

            //north
            grid[x + newSize , y] = edge1;

            //east
            grid[x + currentSize-1 , y + newSize] = edge2;

            //south
            grid[x + newSize, y + currentSize-1] = edge3;

            //west
            grid[x , y + newSize] = edge4;


            //Do the operation over again for each of the four new grids.                 
            DivideGrid (x , y ,                     newSize+1 , corner1 , edge1 , middle , edge4);
            DivideGrid (x + newSize , y ,           newSize+1 , edge1 , corner2 , edge2 , middle);
            DivideGrid (x + newSize , y + newSize , newSize+1 , middle , edge2 , corner3 , edge3);
            DivideGrid (x , y + newSize ,           newSize+1 , edge4 , middle , edge3 , corner4);
        }
    }

    private void SetNodes () {
        while (nodesList.Count < maxNodes) {
            int col = Random.Range (0 , size);
            int row = Random.Range (0 , size);

            if (grid[col , row] != 0f && nodesGrid[col, row] != true) {
                float randomValue = Random.Range (0f , 1f);
                if (randomValue < grid[col, row]) {
                    nodesList.Add (new Node (col , row));
                    nodesGrid[col , row] = true;
                }
            }
        }

        StringBuilder sb;
        sb = new StringBuilder ();
        for (int rows = 0 ; rows < size ; rows++) {
            for (int cols = 0 ; cols < size ; cols++) {
                sb.AppendFormat ("{0} " , (nodesGrid[cols , rows] == true) ? 1 : 0);
            }
            sb.AppendLine ();
        }
        Debug.Log (sb);
        Debug.Log ("end of grid");

    }

    private void ResetBase () {
        grid = new float[size , size];
        nodesGrid = new bool[size , size];

        nodesList = new List<Node> ();
    }

    private class Node {
        public Vector2 pos;
        public List<Node> links { get; private set; }

        public bool isSet { get; private set; }

        public Node () {
            this.pos = new Vector2 ();
        }

        public Node ( int x , int y ) {
            pos.x = x;
            pos.y = y;
        }

        public void AddLink (Node node) {
            links.Add (node);
        }

    }

}
