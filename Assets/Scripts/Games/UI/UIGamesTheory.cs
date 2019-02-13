using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamesTheory : MonoBehaviour
{

    //events
    public delegate void GenerateMapHandler (int branching, int depth);
    static public event GenerateMapHandler GenerateMapEvent;

    static public event Action MapGeneratedEvent;

    public delegate void SearchHandler (string algorythm, int branching, int depth, int fps);
    static public event SearchHandler SearchEvent;


    //output
    [SerializeField] private Text analyzedValue;
    [SerializeField] private Text prunedValue;
    [SerializeField] private Text percentValue;

    //map generation inputs
    [SerializeField] private InputField branchingInput;

    [SerializeField] private InputField depthInput;

    //search inputs
    [SerializeField] private Dropdown algorythmDropdownInput;
    [SerializeField] private InputField fpsInput;

    //links and nodes prefabs
    [SerializeField] private Sprite inactiveBranch;
    [SerializeField] private Sprite inactiveNode;
    [SerializeField] private Sprite activeBranch;
    [SerializeField] private Sprite activeNode;
    [SerializeField] private Sprite exploredBranch;
    [SerializeField] private Sprite exploredNode;
    [SerializeField] private Sprite prunedNode;

    public int branching { get; private set; }
    public int depth { get; private set; }
    public string algorithm { get; private set; }
    public int fps { get; private set; } //does nothing so far

    private int maxLeafCount = 81;
    private int minDepth = 2;
    private int maxDepth = 5;

    static private UIGamesTheory instance;

    #region Initialization

    private void OnEnable ()
    {
        TreeSearcher.TreeUpdatedEvent += UpdatePaths;
        GamesAlgorithm.NodeAnalyzedEvent += IncrementAnalyzed;
        GamesAlgorithm.PrunedNodesEvent += CountPruned;
    }
    private void OnDisable ()
    {
        TreeSearcher.TreeUpdatedEvent -= UpdatePaths;
        GamesAlgorithm.NodeAnalyzedEvent -= IncrementAnalyzed;
        GamesAlgorithm.PrunedNodesEvent -= CountPruned;
    }

    private void Awake ()
    {
        if (instance == null)
            instance = FindObjectOfType<UIGamesTheory> ();
        if (instance != this)
            Destroy (this.gameObject);

        InitializeUI ();
        Initialize ();
        ResetOutput ();
    }

    private void Start ()
    {
        GenerateMap ();
    }

    private void InitializeUI ()
    {
        //map generation panel
        branchingInput.text = "2";
        depthInput.text = "4";

        //search panel
        List<string> algorythms = new List<string> ();
        algorythms.Add ("Minimax");
        algorythms.Add ("Alpha - Beta");

        algorythmDropdownInput.ClearOptions ();
        algorythmDropdownInput.AddOptions (algorythms);
        algorythmDropdownInput.value = 0;

        fpsInput.text = "3";
    }

    private void Initialize ()
    {
        //map generation
        SetBranchingAndDepth ();

        //search
        SetFPS ();
        SetAlgorithm ();
    }
    #endregion


    /// <summary>
    /// Updates the path sprites
    /// </summary>
    /// <param name="leafs">leaf array used as base for the tree's link states</param>
    private void UpdatePaths (TreeNode[] leafs)
    {
        SpriteRenderer sr = new SpriteRenderer ();
        TreeNode activeLeaf = null;
        //update everything except for the active leaf
        for (int i = 0; i < leafs.Length; i++) {
            TreeNode leaf = leafs[i];
            sr = leaf.GO.GetComponent<SpriteRenderer> ();
            switch (leaf.State) {
                case NodeState.Active:
                    sr.sprite = activeNode;
                    break;
                case NodeState.Inactive:
                    sr.sprite = inactiveNode;
                    break;
                case NodeState.Explored:
                    sr.sprite = exploredNode;
                    break;
                case NodeState.Pruned:
                    sr.sprite = prunedNode;
                    break;
                default:
                    sr.sprite = inactiveNode;
                    break;
            }

            if (leaf.State == NodeState.Active) {
                activeLeaf = leaf;
                continue;
            }

            TracePathToRoot (leaf);
        }

        TracePathToRoot (activeLeaf);
    }

    /// <summary>
    ///traces the path from the leaf back to the root
    /// </summary>
    /// <param name="leaf"></param>
    private void TracePathToRoot (TreeNode leaf)
    {
        Queue<TreeBranch> path = new Queue<TreeBranch> ();
        SpriteRenderer sr = new SpriteRenderer ();

        TreeNode parent = leaf;
        while (parent != TreeGenerator.Root) {
            path.Enqueue (parent.parentBranch);
            parent = parent.GetParent ();
        }

        while (path.Count > 0) {
            TreeBranch link = path.Dequeue ();
            sr = link.GO.GetComponent<SpriteRenderer> ();
            switch (leaf.State) {
                case NodeState.Active:
                    sr.sprite = activeBranch;
                    break;
                case NodeState.Explored:
                    sr.sprite = exploredBranch;
                    break;
                default:
                    break;
            }
        }
    }

    public void GenerateMap ()
    {
        ResetOutput ();

        if (GenerateMapEvent != null)
            GenerateMapEvent (branching, depth);

        if (MapGeneratedEvent != null)
            MapGeneratedEvent ();
    }

    #region OutputData


    private void IncrementAnalyzed (TreeNode node)
    {
        int analyzed = Int32.Parse (analyzedValue.text);

        analyzedValue.text = (++analyzed).ToString ();
    }

    /// <summary>
    /// Count how many nodes have been cut-off from the process
    /// </summary>
    private void CountPruned ()
    {
        int analyzed = Int32.Parse (analyzedValue.text);
        float total = Mathf.Pow (branching, depth);

        prunedValue.text = (total - analyzed).ToString ();
        UpdatePercent ();
    }

    private void UpdatePercent ()
    {
        int skipped = Int32.Parse (prunedValue.text);
        float total = Mathf.Pow (branching, depth);
        float percent = skipped / total * 100;

        percentValue.text = String.Format ("{0:0.00}%", percent);
    }

    private void ResetOutput ()
    {
        analyzedValue.text = "0";
        prunedValue.text = "0";
        percentValue.text = "0";
    }
    #endregion

    #region Input Handling

    public void SetFPS () { fps = Int32.Parse (instance.fpsInput.text); }

    public void SetBranchingAndDepth ()
    {
        depth = Mathf.Clamp (Int32.Parse (instance.depthInput.text), minDepth, maxDepth);
        instance.depthInput.text = depth.ToString ();

        branching = Int32.Parse (instance.branchingInput.text);
        int totalLeafCount = Mathf.RoundToInt (Mathf.Pow (depth, branching));
        while (totalLeafCount > maxLeafCount) {
            branching--;
            totalLeafCount = Mathf.RoundToInt (Mathf.Pow (depth, branching));
        }

        instance.branchingInput.text = branching.ToString ();
        Debug.LogFormat ("Branches: {0}, Depth: {1}", branching, depth);
    }

    public void SetAlgorithm () { algorithm = instance.algorythmDropdownInput.options[instance.algorythmDropdownInput.value].text; }


    public void OnPressSearch ()
    {
        OnPressReset ();

        if (SearchEvent != null)
            SearchEvent (algorithm, branching, depth, fps);
    }

    public void OnPressReset ()
    {
        ResetOutput ();
        TreeNode.ResetNodes ();
        //TreeBranch.LoadOriginalBranchList ();

        foreach (TreeBranch branch in TreeBranch.Branches) {
            branch.GO.GetComponent<SpriteRenderer> ().sprite = inactiveBranch;
        }

        foreach (TreeNode leaf in TreeNode.Leaves) {
            leaf.GO.GetComponent<SpriteRenderer> ().sprite = inactiveNode;
        }

        foreach (TreeNode node in TreeNode.Nodes) {
            if (TreeNode.Leaves.Contains (node))
                continue;
        }
    }

    public void OnPressQuit ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene (0);
    }
    #endregion

}
