using UnityEngine;
using System.Collections.Generic;

public class TreeSearcher : MonoBehaviour
{

    private GamesAlgorithm algorithm;

    //links and nodes prefabs
    [SerializeField] private Sprite inactiveBranch;
    [SerializeField] private Sprite inactiveNode;
    [SerializeField] private Sprite activeBranch;
    [SerializeField] private Sprite activeNode;
    [SerializeField] private Sprite exploredBranch;
    [SerializeField] private Sprite exploredNode;
    [SerializeField] private Sprite prunedNode;
    [SerializeField] private Sprite prunedBranch;

    static private TreeSearcher instance;

    private void OnEnable ()
    {
        UIGamesTheory.resetClicked += ResetSprites;
        UIGamesTheory.searchClicked += StartSearching;
        GamesAlgorithm.leafActivated += UpdateLeaves;
    }

    private void OnDisable ()
    {
        UIGamesTheory.resetClicked -= ResetSprites;
        UIGamesTheory.searchClicked -= StartSearching;
        GamesAlgorithm.leafActivated -= UpdateLeaves;
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

                if (TreeGenerator.Root.leafID.HasValue) {
                    int leafID = TreeGenerator.Root.leafID.Value;
                    UpdateLeaves (TreeNode.GetByID (leafID));
                }
            }
        }
        else
            Debug.LogWarning ("Algorithm not set. Search canceled.");
    }

    /// <summary>
    /// updates the leaves states. this happens one each frame
    /// </summary>
    /// <param name="activeLeaf"></param>
    private void UpdateLeaves (TreeNode activeLeaf)
    {
        TreeNode[] leaves = TreeNode.Leaves.ToArray ();
        for (int i = 0; i < leaves.Length; i++) {
            if (leaves[i].ID == activeLeaf.ID && activeLeaf.State != NodeState.Pruned) {
                leaves[i].SetState (NodeState.Explored);
            }
            else if (TreeGenerator.Root.leafID.HasValue) {
                if (leaves[i].ID == TreeGenerator.Root.leafID)
                    leaves[i].SetState (NodeState.Active);
            }
            
            //else if (leaves[i].State == NodeState.Explored) {
            //    continue;
            //}
            //else if (leaves[i].State == NodeState.Active) {
            //    leaves[i].SetState (NodeState.Explored);
            //}
        }

        UpdatePaths(leaves);
    }


    /// <summary>
    /// Updates the path sprites
    /// </summary>
    /// <param name="leafs">leaf array used as base for the tree's link states</param>
    private void UpdatePaths (TreeNode[] leafs)
    {
        SpriteRenderer sr = new SpriteRenderer ();
        TreeNode activeLeaf = null;

        foreach (TreeNode leaf in leafs) {
            sr = leaf.GO.GetComponent<SpriteRenderer> ();
            switch (leaf.State) {
                case NodeState.Active:
                    sr.sprite = activeNode;
                    activeLeaf = leaf;
                    continue;
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
                    Debug.LogErrorFormat ("Stateless leaf, id{0}", leaf.ID);
                    break;
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

        TreeNode node = leaf;
        //queue up all nodes from the leaf to the root
        while (node != TreeGenerator.Root) {
            path.Enqueue (node.parentBranch);
            node = node.GetParent ();
        }
        //change all subsequent branches according to the leaf's state
        while (path.Count > 0) {
            TreeBranch branch = path.Dequeue ();
            sr = branch.GO.GetComponent<SpriteRenderer> ();
            switch (leaf.State) {
                case NodeState.Active:
                    sr.sprite = activeBranch;
                    break;
                case NodeState.Explored:
                    sr.sprite = exploredBranch;
                    break;
                case NodeState.Inactive:
                    sr.sprite = inactiveBranch;
                    break;
                case NodeState.Pruned:
                    sr.sprite = prunedBranch;
                    if (branch.a.State != NodeState.Pruned)
                        return;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// Resets sprites. Useful for reseting the path without resetting everything
    /// </summary>
    private void ResetSprites () 
    {
        foreach (TreeBranch branch in TreeBranch.Branches) {
            branch.GO.GetComponent<SpriteRenderer> ().sprite = inactiveBranch;
        }

        foreach (TreeNode leaf in TreeNode.Leaves) {
            leaf.GO.GetComponent<SpriteRenderer> ().sprite = inactiveNode;
        }
    }

    public void SetAlgorithm (GamesAlgorithm algorithm)
    {
        this.algorithm = algorithm;
    }

}
