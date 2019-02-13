using UnityEngine;
using System.Collections.Generic;

public class TreeSearcher : MonoBehaviour
{

    public delegate void TreeUpdatedHandler (TreeNode[] leafs);
    static public event TreeUpdatedHandler TreeUpdatedEvent;

    private GamesAlgorithm algorithm;

    //links and nodes prefabs
    [SerializeField] private Sprite inactiveBranch;
    [SerializeField] private Sprite inactiveNode;
    [SerializeField] private Sprite activeBranch;
    [SerializeField] private Sprite activeNode;
    [SerializeField] private Sprite exploredBranch;
    [SerializeField] private Sprite exploredNode;
    [SerializeField] private Sprite prunedNode;

    static private TreeSearcher instance;

    private void OnEnable ()
    {
        UIGamesTheory.ResetSearchEvent += ResetSprites;
        UIGamesTheory.SearchEvent += StartSearching;
        GamesAlgorithm.NodeAnalyzedEvent += SetLeafStates;
        TreeUpdatedEvent += UpdatePaths;
    }

    private void OnDisable ()
    {
        UIGamesTheory.ResetSearchEvent -= ResetSprites;
        UIGamesTheory.SearchEvent -= StartSearching;
        GamesAlgorithm.NodeAnalyzedEvent -= SetLeafStates;
        TreeUpdatedEvent -= UpdatePaths;
    }

    private void Awake ()
    {
        if (instance == null)
            instance = FindObjectOfType<TreeSearcher> ();
        if (instance != this)
            Destroy (this.gameObject);
    }

    public void StartSearching (string algorithm, int branching, int depth, int framesPerSecond)
    {
        if (algorithm != null) {
            switch (algorithm) {
                case ("Minimax"):
                    this.algorithm = new Minimax ();
                    break;

                case ("Alpha - Beta"):
                    this.algorithm = new AlphaBeta ();
                    break;

                default:
                    this.algorithm = new AlphaBeta ();
                    break;
            }

            if (this.algorithm != null) {
                Debug.LogFormat ("Reading Tree using {0}.", algorithm);
                this.algorithm.Search (TreeGenerator.Root, branching, depth);
            }
        }
        else
            Debug.LogWarning ("Algorithm not set. Search canceled.");
    }


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

    private void ResetSprites ()
    {
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

    public void SetAlgorithm (GamesAlgorithm algorithm)
    {
        this.algorithm = algorithm;
    }

    private void SetLeafStates (TreeNode node)
    {
        TreeNode[] leafs = TreeNode.Leaves.ToArray ();
        for (int i = 0; i < leafs.Length; i++) {

            if (leafs[i].ID == node.ID) {
                leafs[i].SetState (NodeState.Active);
            }
            else if (leafs[i].State == NodeState.Explored) {
                continue;
            }
            else if (leafs[i].State == NodeState.Active) {
                leafs[i].SetState (NodeState.Explored);
            }
            else if (leafs[i].ID < node.ID) {
                leafs[i].SetState (NodeState.Pruned);
            }
            else {
                leafs[i].SetState (NodeState.Inactive);
            }
        }

        if (TreeUpdatedEvent != null) {
            TreeUpdatedEvent (leafs);
        }

    }

}
