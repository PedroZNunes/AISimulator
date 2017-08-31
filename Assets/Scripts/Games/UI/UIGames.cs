using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIGames : MonoBehaviour {

    //events
    public delegate void GenerateMapHandler (int branching, int depth);
    static public event GenerateMapHandler GenerateMapEvent;

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
    [SerializeField] private Sprite inactiveLink;
    [SerializeField] private Sprite inactiveNode;
    [SerializeField] private Sprite activeLink;
    [SerializeField] private Sprite activeNode;
    [SerializeField] private Sprite exploredLink;
    [SerializeField] private Sprite exploredNode;
    [SerializeField] private Sprite prunedNode;

    public int branching { get; private set; }
    public int depth { get; private set; }
    public string algorythm { get; private set; }
    public int fps { get; private set; } //does nothing so far

    static private UIGames instance;

    //events
    private void OnEnable () {
        TreeSearcher.TreeUpdatedEvent += UpdatePaths;
        GamesAlgorythm.NodeAnalyzedEvent += IncrementAnalyzed;
        GamesAlgorythm.PrunedNodesEvent += CountPruned;
    }
    private void OnDisable () {
        TreeSearcher.TreeUpdatedEvent -= UpdatePaths;
        GamesAlgorythm.NodeAnalyzedEvent -= IncrementAnalyzed;
        GamesAlgorythm.PrunedNodesEvent -= CountPruned;
    }

    private void Awake () {
        if (instance == null)
            instance = FindObjectOfType<UIGames> ();
        if (instance != this)
            Destroy (this.gameObject);

        InitializeUI ();
        Initialize ();
        ResetOutput ();
    }

    private void Initialize () {
        //map generation
        SetBranching ();
        SetDepth ();

        //search
        SetFPS ();
        SetAlgorythm ();
    }

    private void InitializeUI () {
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

        fpsInput.text = "2";
    }

    /// <summary>
    /// Updates the path sprites
    /// </summary>
    /// <param name="leafs">leaf array used as base for the tree's link states</param>
    private void UpdatePaths (GamesNode[] leafs) {
        SpriteRenderer sr = new SpriteRenderer ();
        GamesNode activeLeaf = null;
        //update everything except for the active leaf
        for (int i = 0 ; i < leafs.Length ; i++) {
            GamesNode leaf = leafs[i];
            sr = leaf.GO.GetComponent<SpriteRenderer> ();
            switch (leaf.nodeState) {
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

            if (leaf.nodeState == NodeState.Active) {
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
    private void TracePathToRoot(GamesNode leaf) {
        Queue<GamesLink> path = new Queue<GamesLink> ();
        SpriteRenderer sr = new SpriteRenderer ();

        GamesNode parent = leaf;
        while (parent != TreeGenerator.Root) {
            path.Enqueue (parent.parentLink);
            parent = parent.GetParent ();
        }

        while (path.Count > 0) {
            GamesLink link = path.Dequeue ();
            sr = link.GO.GetComponent<SpriteRenderer> ();
            switch (leaf.nodeState) {
                case NodeState.Active:
                    sr.sprite = activeLink;
                    break;
                case NodeState.Explored:
                    sr.sprite = exploredLink;
                    break;
                default:
                    break;
            }
        }
    }

    public void GenerateMap () {
        ResetOutput ();

        if (GenerateMapEvent != null)
            GenerateMapEvent (branching, depth);
    }

    public void Search () {
        ResetOutput ();

        if (SearchEvent != null)
            SearchEvent (algorythm, branching, depth, fps);
    }

    private void IncrementAnalyzed (GamesNode node) {
        int analyzed = Int32.Parse (analyzedValue.text);

        analyzedValue.text = (++analyzed).ToString ();
    }

    /// <summary>
    /// Count how many nodes have been cut-off from the process
    /// </summary>
    private void CountPruned () {
        int analyzed = Int32.Parse (analyzedValue.text);
        float total = Mathf.Pow (branching, depth);

        prunedValue.text = (total - analyzed).ToString ();
        UpdatePercent ();
    }

    private void UpdatePercent () {
        int skipped = Int32.Parse (prunedValue.text);
        float total = Mathf.Pow (branching, depth);
        float percent = skipped / total * 100;

        percentValue.text = String.Format ("{0:0.00}%", percent);
    }

    private void ResetOutput () {
        analyzedValue.text = "0";
        prunedValue.text = "0";
        percentValue.text = "0";
    }

    //A series of input-related functions
    public void SetFPS () { fps = Int32.Parse (instance.fpsInput.text); }
    public void SetBranching () { branching = Int32.Parse (instance.branchingInput.text); }
    public void SetDepth () { depth = Int32.Parse (instance.depthInput.text); }
    public void SetAlgorythm () { algorythm = instance.algorythmDropdownInput.options[instance.algorythmDropdownInput.value].text; }



}
