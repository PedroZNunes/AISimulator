using UnityEngine;
using Object = UnityEngine.Object;

public class TreeGenerator : MonoBehaviour
{

    public delegate void SetCameraHandler (int depth, int branch, float stepY);
    static public event SetCameraHandler SetCameraEvent;

    //prefabs
    [SerializeField] private GameObject minPrefab;
    [SerializeField] private GameObject maxPrefab;
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private GameObject linkPrefab;

    private const float hwRatio = 0.4f;

    private int branching; //how many branches leave each node
    static public int depth { get; private set; } // the amount of levels the tree has

    private float spacingY; //used for placement in the grid, spacing in Y

    static public TreeNode Root { get; private set; } //the root node

    static private TreeGenerator instance;

    //events
    private void OnEnable ()
    {
        UIGamesTheory.GenerateMapEvent += Generate;
    }
    private void OnDisable ()
    {
        UIGamesTheory.GenerateMapEvent -= Generate;
    }

    private void Awake ()
    {
        if (instance == null)
            instance = FindObjectOfType<TreeGenerator> ();
        if (instance != this)
            Destroy (this.gameObject);
    }

    /// <summary>
    /// generate the map
    /// </summary>
    /// <param name="b">branches per node</param>
    /// <param name="d">depth: number of floors or levels</param>
    private void Generate (int b, int d)
    {
        //setup
        Debug.Log ("Generating map.");
        branching = b;
        depth = d;

        NodeType nodeType = NodeType.Max;

        ResetTree ();

        Root = new TreeNode (b, 0, nodeType);


        //spawn the first node
        GameObject rootGO = (GameObject)Instantiate (maxPrefab, Vector2.zero, Quaternion.identity, this.transform);

        rootGO.name = Root.Type.ToString () + " " + Root.ID;
        rootGO.transform.localScale *= Mathf.Clamp (Mathf.Pow (branching, depth - 2) / 2, 1f, float.MaxValue);

        Root.GO = rootGO;

        //spawn the nodes linked to it
        SpawnBranchesOf (Root);

        if (SetCameraEvent != null) {
            SetCameraEvent (depth, branching, spacingY);
        }

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
            TreeNode toSpawn = parentNode.GetOther (i);
            //calculate position
            Vector2 step = new Vector2 ();
            step.x = Mathf.Pow (branching, depth - toSpawn.depth);
            step.y = spacingY = Mathf.Max (step.x * (branching - 1) * hwRatio, 1);

            Vector2 position = new Vector2 ();
            float initialPosX = parentNode.GO.transform.position.x - ((branching - 1) * step.x / 2); //left-most position
            position.x = initialPosX + (step.x * i);
            position.y = parentNode.GO.transform.position.y - step.y;
            //Debug.LogFormat ("initial posX: {0}. stepX: {1}. pos{2}", initialPosX, step.x, pos);

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
            go.transform.localScale *= Mathf.Clamp (step.x / (2 * branching), 1f, float.MaxValue);

            if (prefab == leafPrefab) {
                go.GetComponent<UITreeNode> ().AssignValue (toSpawn.Score);
                go.name = "Leaf " + toSpawn.ID;
            }
            else {
                go.name = toSpawn.Type.ToString () + " " + toSpawn.ID;
            }

            toSpawn.GO = go;


            //prepare the link
            position = (toSpawn.GO.transform.position + parentNode.GO.transform.position) / 2;
            //angle
            double branchAngle = Mathf.Atan2 (toSpawn.GO.transform.position.y - parentNode.GO.transform.position.y, toSpawn.GO.transform.position.x - parentNode.GO.transform.position.x) * Mathf.Rad2Deg + 90;
            //scale
            float branchScale = Vector2.Distance (parentNode.GO.transform.position, toSpawn.GO.transform.position);
            //float width = Mathf.Max (depth - toSpawn.depth, 1f) + ((branching - 1) / 2) * (depth - toSpawn.depth);

            //spawn the link
            go = Instantiate (linkPrefab, position, Quaternion.identity, transform);
            go.transform.localScale = new Vector3 (1f, branchScale * 4, 1f);
            go.transform.eulerAngles = new Vector3 (0f, 0f, (float)branchAngle);

            parentNode.branches[i].GO = go;

            //then spawn the nodes linked to each linked node
            SpawnBranchesOf (toSpawn);
        }
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

}
