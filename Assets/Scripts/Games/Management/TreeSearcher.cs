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
    }

    private void OnDisable ()
    {
        UIGamesTheory.resetClicked -= ResetSprites;
        UIGamesTheory.searchClicked -= StartSearching;
    }

    private void Awake ()
    {
        if (instance == null)
            instance = FindObjectOfType<TreeSearcher> ();
        if (instance != this)
            Destroy (this.gameObject);
    }

    public void StartSearching (string algorithm, int branching, int depth)
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
                this.algorithm.Search (TreeGenerator.root, branching, depth);

                if (TreeGenerator.root.leafID.HasValue) {
                    TreeNode.GetByID (TreeGenerator.root.leafID.Value).SetState (NodeState.Active);
                    UpdateSprites ();
                }
            }
        }
        else
            Debug.LogWarning ("Algorithm not set. Search canceled.");
    }


    /// <summary>
    /// Updates the path sprites
    /// </summary>
    /// <param name="leafs">leaf array used as base for the tree's link states</param>
    private void UpdateSprites ()
    {
        SpriteRenderer sr;
        
        foreach (TreeBranch branch in TreeBranch.Branches) {
            sr = branch.GO.GetComponent<SpriteRenderer> ();

            switch (branch.State) {
                case NodeState.Active:
                    sr.sprite = activeBranch;
                    continue;
                case NodeState.Inactive:
                    sr.sprite = inactiveBranch;
                    break;
                case NodeState.Explored:
                    sr.sprite = exploredBranch;
                    break;
                case NodeState.Pruned:
                    sr.sprite = prunedBranch;
                    break;
                default:
                    Debug.LogErrorFormat ("Stateless branch: {0}", branch.GO.name);
                    break;
            }
        }

        foreach (TreeNode leaf in TreeNode.Leaves) {
            sr = leaf.GO.GetComponent<SpriteRenderer> ();
            switch (leaf.State) {
                case NodeState.Active:
                    sr.sprite = activeNode;
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

        }
        if (TreeGenerator.root.leafID.HasValue) {
            TreeNode activeLeaf = TreeNode.GetByID (TreeGenerator.root.leafID);
            TracePathToRoot (activeLeaf);
        }
    }

    /// <summary>
    /// traces the path from the leaf back to the root
    /// </summary>
    /// <param name="leaf"></param>
    private void TracePathToRoot (TreeNode leaf)
    {
        Queue<TreeBranch> path = new Queue<TreeBranch> ();
        SpriteRenderer sr;

        TreeNode node = leaf;
        //queue up all nodes from the leaf to the root
        while (node != TreeGenerator.root) {
            path.Enqueue (node.parentBranch);
            node = node.GetParent ();
        }
        //change all subsequent branches according to the leaf's state
        while (path.Count > 0) {
            TreeBranch branch = path.Dequeue ();
            sr = branch.GO.GetComponent<SpriteRenderer> ();
            sr.sprite = activeBranch;
        }
    }
    /// <summary>
    /// Resets sprites. Useful for reseting the path without resetting everything
    /// </summary>
    private void ResetSprites () 
    {
        foreach (TreeBranch branch in TreeBranch.Branches) {
            branch.GO.GetComponent<SpriteRenderer> ().sprite = prunedBranch;
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
