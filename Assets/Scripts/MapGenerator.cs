using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private GameObject linkPrefab;

    [SerializeField]
    private int size = 0;
    public int Size { get { return size; } }

    [SerializeField]
    private float grain = 0;

    private List<int> sizeList;

    [SerializeField]
    private int maxNodes = 10;
    [SerializeField]
    private int maxLinksPerNode = 4;
    [SerializeField]
    private float maxCosAngle = 0.85f;

    private float maxDistance;

    private float[,] grid;
    private bool[,] nodesGrid;

    static public List<Node> nodes { get; private set; }
    static public List<Link> allLinks { get; private set; }


    private void OnValidate () {
        if (maxNodes > size * size / 2) {
            maxNodes = size * size / 2;
        }
    }

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

    public void Generate () {

        ResetBase ();
        Diamond ();
        SetNodes ();
        SetLinks ();
        CleanUp ();
        ToScreen ();
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
                sb.AppendFormat ("{0:0.0}  " , grid[cols , rows]);
            }
            sb.AppendLine ();
        }
        Debug.Log (sb);
    }

    private float DisplaceMiddle ( float num ) {
        float max = num / ( 2 * size ) * grain;
        return Random.Range (-0.5f , 0.5f) * max;
    }

    private void DivideGrid ( int x , int y , int currentSize , float corner1 , float corner2 , float corner3 , float corner4 ) {
        int newSize = currentSize / 2;
        //Debug.LogFormat ("dividing grid [{0}, {1}] of size {2}. halfSize {3}" , x , y, currentSize, newSize);

        if (currentSize > 2) {

            //Random the displacement while making sure the middle does not get displaced out of bounds
            float middle = Mathf.Clamp01 (( corner1 + corner2 + corner3 + corner4 ) / 4 + DisplaceMiddle (2 * newSize));

            float edge1 = ( corner1 + corner2 ) / 2; //Calculate the edges by averaging the two corners of each edge.
            float edge2 = ( corner2 + corner3 ) / 2;
            float edge3 = ( corner3 + corner4 ) / 2;
            float edge4 = ( corner4 + corner1 ) / 2;

            //middle
            grid[x + newSize , y + newSize] = middle;

            //north
            grid[x + newSize , y] = edge1;

            //east
            grid[x + currentSize - 1 , y + newSize] = edge2;

            //south
            grid[x + newSize , y + currentSize - 1] = edge3;

            //west
            grid[x , y + newSize] = edge4;


            //Do the operation over again for each of the four new grids.                 
            DivideGrid (x , y , newSize + 1 , corner1 , edge1 , middle , edge4);
            DivideGrid (x + newSize , y , newSize + 1 , edge1 , corner2 , edge2 , middle);
            DivideGrid (x + newSize , y + newSize , newSize + 1 , middle , edge2 , corner3 , edge3);
            DivideGrid (x , y + newSize , newSize + 1 , edge4 , middle , edge3 , corner4);
        }
    }

    private void SetNodes () {
        while (nodes.Count < maxNodes) {
            int col = Random.Range (0 , size);
            int row = Random.Range (0 , size);

            if (grid[col , row] != 0f && nodesGrid[col , row] != true) {
                //avoid placing nodes right next to each other
                if (!NodeHasNeighbours (col , row)) {
                    float randomValue = Random.Range (0f , 1f);
                    if (randomValue < grid[col , row]) {
                        nodes.Add (new Node (col , row));
                        nodesGrid[col , row] = true;
                    }
                }
            }
        }

        int[,] tempGrid = new int[size , size];
        for (int i = 0 ; i < nodes.Count ; i++) {
            Node node = nodes[i];
            tempGrid[(int) node.pos.x , (int) node.pos.y] = nodes[i].ID;
        }

        StringBuilder sb;
        sb = new StringBuilder ();
        for (int rows = 0 ; rows < size ; rows++) {
            for (int cols = 0 ; cols < size ; cols++) {
                sb.AppendFormat ("{0:00} " , tempGrid[cols , rows]);
            }
            sb.AppendLine ();
        }
        Debug.Log (sb);
    }

    private void SetLinks () {
        for (int i = 0 ; i < nodes.Count ; i++) {
            Node node = nodes[i];
            //generate a list of possible connections for each node, getting everything from twice the max distance down and getting all of it into a table ordered by the closest to the furthest
            List<NodeDist> possibleConnections = new List<NodeDist> ();
            for (int j = 0 ; j < nodes.Count ; j++) {
                if (i == j)
                    continue;
                Node other = nodes[j];

                float distance = Vector2.Distance (node.pos , other.pos);
                if (distance <= maxDistance) {
                    possibleConnections.Add (new NodeDist (other , distance));
                }
            }

            possibleConnections.Sort ();

            //clean up connections with very close angles, giving priority to the closer nodes
            if (possibleConnections.Count > 0) {
                for (int j = 0 ; j < possibleConnections.Count ; j++) {
                    if (j == 0)
                        continue;

                    //also just keep the first maxLinkCount nodes
                    if (j > maxLinksPerNode) {
                        possibleConnections.RemoveAt (j);
                        j--;
                    }

                    Node linkTarget = possibleConnections[j].node;
                    bool toBeDiscarded = false;
                    for (int k = 0 ; k < j ; k++) {
                        Node connection = possibleConnections[k].node;

                        Vector2 toTarget = linkTarget.pos - node.pos;
                        Vector2 toConnection = connection.pos - node.pos;
                        float cos = Vector2.Dot (toTarget , toConnection) / ( toTarget.magnitude * toConnection.magnitude );

                        if (cos > maxCosAngle) {
                            //Debug.LogFormat ("From node {0}, node {1} discarded as being too close to node {2}." , node.ID, linkTarget.ID , connection.ID);
                            toBeDiscarded = true;
                            break;
                        }
                    }

                    if (toBeDiscarded) {
                        possibleConnections.RemoveAt (j);
                        j--;
                    }
                }
            }

            ///printing the connections
            StringBuilder stringBuilder;
            stringBuilder = new StringBuilder ();
            stringBuilder.AppendFormat ("Possible Conenctions for node {0}: \n" , node.ID);
            for (int j = 0 ; j < possibleConnections.Count ; j++) {
                stringBuilder.AppendFormat ("{0} \t {1} units away \n" , possibleConnections[j].node.ID , possibleConnections[j].distance);
            }
            stringBuilder.AppendLine ();
            Debug.Log (stringBuilder);
            ///end of printing

            //spawning links by chance
            for (int j = 0 ; j < possibleConnections.Count ; j++) {
                Node connection = possibleConnections[j].node;

                //chance of setting the link by link count
                float linkCountProb = Mathf.Clamp01 (( maxLinksPerNode - node.links.Count ) / (float) maxLinksPerNode);

                float random = Random.value;
                if (random <= linkCountProb) {
                    if (connection.links.Count < maxLinksPerNode) {
                        if (!connection.links.Contains (node)) {
                            if (!connection.isSet || ( connection.isSet && node.links.Count == 0 && possibleConnections.Count < 3 ) || ( node.links.Count == 0 && j == possibleConnections.Count - 1 )) {
                                node.AddLink (connection);
                                connection.AddLink (node);
                            }
                        }
                    }
                }
                else {
                    Debug.LogFormat ("Link between {0} and {1} not instantiated by chance." , node.ID , connection.ID);
                }
            }
            node.isSet = true;
        }

        StringBuilder sb;
        sb = new StringBuilder ("List of all Links: \n");
        for (int i = 0 ; i < nodes.Count ; i++) {
            Node node = nodes[i];
            for (int j = 0 ; j < node.links.Count ; j++) {
                sb.AppendFormat ("{0} <-> {1} |\t" , node.ID , node.links[j].ID);
            }
            sb.AppendLine ();
        }
        Debug.Log (sb);
    }

    private void CleanUp () {
        for (int i = 0 ; i < nodes.Count ; i++) {
            if (nodes[i].links.Count == 0) {
                nodes.RemoveAt (i);
                i--;
            }
        }
    }

    private void ToScreen () {

        for (int i = 0 ; i < nodes.Count ; i++) {
            Node node = nodes[i];

            GameObject nodeGO = (GameObject) Instantiate (nodePrefab , node.pos , Quaternion.identity , transform);
            nodeGO.name = "node " + node.ID;

            nodes[i].GO = nodeGO;

            for (int j = 0 ; j < node.links.Count ; j++) {
                Node link = node.links[j];

                //if (!links.Contains (  new Link (node , node.links[j])) && !links.Contains (new Link (node.links[j] , node))) {
                bool contains = false;
                for (int k = 0 ; k < allLinks.Count ; k++) {
                    if (allLinks[k].HasNodes (node , link)) {
                        contains = true;
                        break;
                    }
                }

                if (!contains) { 
                    //prep the link
                    //angulo
                    double angle = Mathf.Atan2 (link.pos.y - node.pos.y , link.pos.x - node.pos.x) * Mathf.Rad2Deg + 90;
                    //posição
                    Vector2 pos = ( link.pos + node.pos ) / 2;
                    //scale
                    float scale = Vector2.Distance (node.pos , link.pos);

                    GameObject go = Instantiate (linkPrefab , pos , Quaternion.identity , transform);
                    go.transform.localScale = new Vector3 (1f , scale * 4 , 1f);
                    go.transform.eulerAngles = new Vector3 (0f , 0f , (float) angle);

                    allLinks.Add (new Link (node , link , go));
                } 
            }
        }

        SetCamera ();

    }

    public Node RandomNode () {
        Node node = nodes[Random.Range (0 , size - 1)];
        Debug.LogFormat ("Returning random node {0}", node.ID);
        return ( node );
    }

    public void SetCamera () {
        Camera camera = Camera.main;
        camera.orthographicSize = ( ( size - 1 ) / 2 ) + 1;
        camera.transform.position = new Vector3 (camera.orthographicSize - 1 , camera.orthographicSize - 1 , camera.transform.position.z);
    }

    private void ResetBase () {
        grid = new float[size , size];
        nodesGrid = new bool[size , size];

        nodes = new List<Node> ();
        allLinks = new List<Link> ();

        maxDistance = ( size * size ) / ( 1.5f * maxNodes );
        Debug.LogFormat ("Max link distance: {0}" , maxDistance);
    }

    private bool NodeHasNeighbours ( int x , int y ) {
        bool hasNeighbours = false;

        if (x + 1 < size)
            if (nodesGrid[x + 1 , y])
                hasNeighbours = true;

        if (y + 1 < size)
            if (nodesGrid[x , y + 1])
                hasNeighbours = true;

        if (x - 1 >= 0)
            if (nodesGrid[x - 1 , y])
                hasNeighbours = true;
        if (y - 1 >= 0)
            if (nodesGrid[x , y - 1])
                hasNeighbours = true;

        return hasNeighbours;
    }

    private class NodeDist : IComparable {
        public float distance;
        public Node node;

        public NodeDist ( Node node , float distance ) {
            this.distance = distance;
            this.node = node;
        }

        public int CompareTo ( object obj ) {
            if (obj == null) return 1;

            NodeDist other = obj as NodeDist;
            if (other != null)
                return this.distance.CompareTo (other.distance);
            else
                throw new ArgumentException ("Object is not valid");
        }
    }


}



