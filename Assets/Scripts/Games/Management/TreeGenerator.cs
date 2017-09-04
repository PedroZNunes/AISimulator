using UnityEngine;
using Object = UnityEngine.Object;

public class TreeGenerator : MonoBehaviour {

    public delegate void SetCameraHandler (int depth, int branch, float stepY);
    static public event SetCameraHandler SetCameraEvent;

    //prefabs
    [SerializeField] private GameObject minPrefab;
    [SerializeField] private GameObject maxPrefab;
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private GameObject linkPrefab;

    private const float hwRatio = 0.5f;
    //private const float hwRatio = 0.736196319018404f;

    private int branching; //how many branches leave each node
    static public int treeDepth { get; private set; } // the amount of levels the tree has
    
    private float spacingY; //used for placement in the grid, spacing in Y

    static public GamesNode Root { get; private set; } //the root node

    static private TreeGenerator instance;

    //events
    private void OnEnable () {
        UIGames.GenerateMapEvent += Generate;
    }
    private void OnDisable () {
        UIGames.GenerateMapEvent -= Generate;
    }

    private void Awake () {
        if (instance == null) 
            instance = FindObjectOfType<TreeGenerator> ();
        if (instance != this)
            Destroy (this.gameObject);
    }

    //generate the map
    private void Generate (int branching, int depth) {
        Debug.Log ("Generating map.");
        this.branching = branching;
        treeDepth = depth;

        NodeType nodeType = NodeType.Max;

        ResetTree ();

        Root = new GamesNode (branching, 0, nodeType);

        VisualizeTree ();
    }

    //tree to screen
    private void VisualizeTree () {
        //spawn the first node
        GameObject go = (GameObject) Instantiate (maxPrefab, Vector2.zero, Quaternion.identity, this.transform);
        go.name = Root.nodeType.ToString () + " " + Root.ID;
        //go.transform.localScale *= Mathf.Clamp ((treeDepth - Root.depth) * (branching - 1), 1f, float.MaxValue);
        go.transform.localScale *= Mathf.Clamp (Mathf.Pow (branching, treeDepth-2) / 2, 1f, float.MaxValue);
        Root.GO = go;
        //spawn the nodes linked to it
        SpawnLinksOf (Root);

        if (SetCameraEvent != null) {
            SetCameraEvent (treeDepth, branching, spacingY);
        }
    }

    //recursive spawning 
    private void SpawnLinksOf (GamesNode parentNode) {
        if (parentNode.depth == treeDepth)
            return;

        //spawn node with position relative to parent.
        for (int i = 0 ; i < branching ; i++) {
            GamesNode toSpawn = parentNode.GetOther (i);
            //calculate position
            Vector2 step = new Vector2 ();
            step.x = Mathf.Pow (branching, treeDepth - toSpawn.depth);
            step.y = spacingY = Mathf.Max (step.x * (branching - 1) * hwRatio, 1);

            Vector2 pos = new Vector2 ();
            float initialPosX = parentNode.GO.transform.position.x - ((branching - 1) * step.x / 2); //left-most position
            pos.x = initialPosX + (step.x * i);
            pos.y = parentNode.GO.transform.position.y - step.y;
            //Debug.LogFormat ("initialposX: {0}. stepX: {1}. pos{2}", initialPosX, step.x, pos);

            //assign prefab according to node type (leaf, min or max)
            Object prefab;
            if (treeDepth == toSpawn.depth)
                prefab = leafPrefab;
            else if (toSpawn.nodeType == NodeType.Max)
                prefab = maxPrefab;
            else 
                prefab = minPrefab;

            //instantiate the node
            GameObject go = (GameObject) Instantiate (prefab, pos, Quaternion.identity, this.transform);
            go.name = toSpawn.nodeType.ToString () + " " + toSpawn.ID;
            go.transform.localScale *= Mathf.Clamp (step.x / (2 * branching), 1f, float.MaxValue);
            //go.transform.localScale *= Mathf.Clamp((treeDepth - toSpawn.depth + 1) * branching / 2, 1f, float.MaxValue);
            toSpawn.GO = go;

            if (prefab == leafPrefab) {
                go.GetComponent<UITreeNode> ().AssignValue (toSpawn.value);
            }

            //prepare the link
            //posição
            pos = (toSpawn.GO.transform.position + parentNode.GO.transform.position) / 2;
            //angle
            double angle = Mathf.Atan2 (toSpawn.GO.transform.position.y - parentNode.GO.transform.position.y, toSpawn.GO.transform.position.x - parentNode.GO.transform.position.x) * Mathf.Rad2Deg + 90;
            //scale
            float scale = Vector2.Distance (parentNode.GO.transform.position, toSpawn.GO.transform.position);
            float width = Mathf.Max (treeDepth - toSpawn.depth, 1f) + ((branching - 1) / 2) * (treeDepth - toSpawn.depth);

            //spawn the link
            go = Instantiate (linkPrefab, pos, Quaternion.identity, transform);
            go.transform.localScale = new Vector3 (width, scale * 4, 1f);
            go.transform.eulerAngles = new Vector3 (0f, 0f, (float) angle);

            parentNode.links[i].GO = go;

            //then spawn the nodes linked to each linked node
            SpawnLinksOf (toSpawn);
        }
    }

    //reset the tree for generating another one
    private void ResetTree () {
        for (int i = 0 ; i < GamesNode.Nodes.Count ; i++) {
            Destroy (GamesNode.Nodes[i].GO);
        }
        GamesNode.Reset ();

        for (int i = 0 ; i < GamesLink.Links.Count ; i++) {
            Destroy (GamesLink.Links[i].GO);
        }
        GamesLink.Reset ();
    }

}
