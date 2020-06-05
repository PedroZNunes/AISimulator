using UnityEngine;
using Object = UnityEngine.Object;

public class TreeGenerator : MonoBehaviour
{

    static private TreeGenerator instance;


    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private GameObject linkPrefab;
    [SerializeField] private GameObject maxPrefab;
    [SerializeField] private GameObject minPrefab;


    public delegate void TreeGenerated (int depth, int branch);

    static public event TreeGenerated treeGenerated;

    static public int depth { get; private set; } // the amount of levels the tree has
    private int branching; //how many branches leave each node

    static public TreeNode root { get; private set; } //the root node


    #region Initialization
    private void OnEnable ()
    {
        TreeUIManagement.generateClicked += Generate;
    }
    private void OnDisable ()
    {
        TreeUIManagement.generateClicked -= Generate;
    }

    private void Awake ()
    {
        if (instance == null)
            instance = FindObjectOfType<TreeGenerator> ();
        if (instance != this)
            Destroy (this.gameObject);
    }
    #endregion

    /// <summary>
    /// this function generates the map.
    /// It spawns the first node, the top-most one, and then it calls a recursive method for spawning the branches.
    /// </summary>
    /// <param name="b">branches per node, or children per parent</param>
    /// <param name="d">depth: number of levels or generations</param>
    private void Generate (int b, int d)
    {
        //setup
        Debug.Log ("Generating map.");
        branching = b;
        depth = d;

        NodeType nodeType = NodeType.Max;

        ResetTree ();

        //spawn the first node
        root = new TreeNode (b, 0, nodeType);

        GameObject rootGO = (GameObject)Instantiate (maxPrefab, Vector2.zero, Quaternion.identity, this.transform);
        SetNodeScale (ref rootGO, root.depth);


        rootGO.name = root.Type.ToString () + " " + root.ID;

        root.GO = rootGO;

        //spawn the nodes linked to it
        SpawnBranchesOf (root);

        OnTreeGenerated ();
    }
    

    /// <summary>
    /// recursive spawning 
    /// </summary>
    /// <param name="parentNode"></param>
    private void SpawnBranchesOf (TreeNode parentNode)
    {
        if (parentNode.depth == depth)
            return;

        //spawn node with position relative to parent.
        for (int i = 0; i < branching; i++) {
            TreeNode childNode = parentNode.GetOtherNodeFromBranchByIndex (i);

            SpawnNode (parentNode, childNode, i);
            SpawnBranch (parentNode, childNode, i);

            //then proceed to spawning the children
            SpawnBranchesOf (childNode);
        }
    }

    private void SpawnBranch (TreeNode parentNode, TreeNode childNode, int i)
    {
        //prepare the link
        Vector2 position = (childNode.GO.transform.position + parentNode.GO.transform.position) / 2;
        //angle
        double branchAngle = Mathf.Atan2 (childNode.GO.transform.position.y - parentNode.GO.transform.position.y, childNode.GO.transform.position.x - parentNode.GO.transform.position.x) * Mathf.Rad2Deg + 90;
        //scale
        float branchScale = Vector2.Distance (parentNode.GO.transform.position, childNode.GO.transform.position);

        //spawn the link
        GameObject go = Instantiate (linkPrefab, position, Quaternion.identity, transform);
        go.transform.localScale = new Vector3 (0.5f, branchScale * 4, 1f);
        go.transform.eulerAngles = new Vector3 (0f, 0f, (float)branchAngle);

        parentNode.branches[i].GO = go;
    }

    /// <summary>
    /// deals with transform operations and prefab stuff. it spawns a node
    /// </summary>
    /// <param name="parentNode">node's parent for positioning reference</param>
    /// <param name="toSpawn">node to be spawned</param>
    /// <param name="i">current branch index</param>
    private void SpawnNode (TreeNode parentNode, TreeNode toSpawn, int i)
    {
        //calculate position
        Vector2 step = new Vector2 ();
        step.x = Mathf.Pow (branching, depth - toSpawn.depth);

        //distance between rows
        step.y = 1 + ((branching * (depth - toSpawn.depth)) * (1 / Camera.main.aspect));


        Vector2 position = new Vector2 ();
        float initialPosX = parentNode.GO.transform.position.x - ((branching - 1) * step.x / 2); //left-most position
        position.x = initialPosX + (step.x * i);
        position.y = parentNode.GO.transform.position.y - step.y;

        //assign prefab according to node type (leaf, min or max)
        Object prefab;
        if (depth == toSpawn.depth)
            prefab = leafPrefab;
        else if (toSpawn.Type == NodeType.Max)
            prefab = maxPrefab;
        else
            prefab = minPrefab;

        //instantiate the node
        GameObject go = (GameObject)Instantiate (prefab, position, Quaternion.identity, this.transform);
        SetNodeScale(ref go, toSpawn.depth);

        if (prefab == leafPrefab) {
            go.GetComponent<TreeUINode> ().AssignScore (toSpawn.Score);
        }
        go.name = toSpawn.Type.ToString () + " " + toSpawn.ID;

        toSpawn.GO = go;
    }

    private void SetNodeScale (ref GameObject nodeGO, int currentDepth) {
        //if the grid gets too big, scale nodes a bit so they are visible
        int totalLeafs = Mathf.RoundToInt(Mathf.Pow(branching, depth));

        nodeGO.transform.localScale *= Mathf.Max( 1 + ((branching * (depth - currentDepth - 1)) / (2 * Camera.main.aspect)), 1 );
    }

    /// <summary>
    /// reset the tree for generating another one
    /// </summary>
    private void ResetTree ()
    {
        for (int i = 0; i < TreeNode.Nodes.Count; i++) {
            Destroy (TreeNode.Nodes[i].GO);
        }
        TreeNode.Reset ();

        for (int i = 0; i < TreeBranch.Branches.Count; i++) {
            Destroy (TreeBranch.Branches[i].GO);
        }
        TreeBranch.Reset ();
    }

    private void OnTreeGenerated ()
    {
        if (treeGenerated != null) {
            treeGenerated (depth, branching);
        }
    }
}
