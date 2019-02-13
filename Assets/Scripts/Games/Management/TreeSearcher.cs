using UnityEngine;

public class TreeSearcher : MonoBehaviour
{

    public delegate void TreeUpdatedHandler (TreeNode[] leafs);
    static public event TreeUpdatedHandler TreeUpdatedEvent;

    private GamesAlgorithm algorithm;

    static private TreeSearcher instance;

    private void OnEnable ()
    {
        UIGamesTheory.SearchEvent += StartSearching;
        GamesAlgorithm.NodeAnalyzedEvent += SetLeafStates;
    }
    private void OnDisable ()
    {
        UIGamesTheory.SearchEvent -= StartSearching;
        GamesAlgorithm.NodeAnalyzedEvent -= SetLeafStates;
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
