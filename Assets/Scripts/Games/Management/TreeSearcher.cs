using UnityEngine;

public class TreeSearcher : MonoBehaviour {

    public delegate void TreeUpdatedHandler (GamesNode[] leafs);
    static public event TreeUpdatedHandler TreeUpdatedEvent;

    private GamesAlgorythm algorythm;

    static private TreeSearcher instance;

    private void OnEnable () {
        UIGames.SearchEvent += StartSearching;
        GamesAlgorythm.NodeAnalyzedEvent += SetLeafStates;
    }
    private void OnDisable () {
        UIGames.SearchEvent -= StartSearching;
        GamesAlgorythm.NodeAnalyzedEvent -= SetLeafStates;
    }

    private void Awake () {
        if (instance == null)
            instance = FindObjectOfType<TreeSearcher> ();
        if (instance != this)
            Destroy (this.gameObject);
    }

    public void StartSearching (string algorythm, int branching, int depth, int framesPerSecond) {
        if (algorythm != null) {
            switch (algorythm) {
                case ("Minimax"):
                    this.algorythm = new Minimax ();
                    break;

                case ("Alpha - Beta"):
                    this.algorythm = new AlphaBeta ();
                    break;

                default:
                    this.algorythm = new AlphaBeta ();
                    break;
            }

            if (this.algorythm != null) {
                Debug.LogFormat ("Reading Tree using {0}.", algorythm);
                this.algorythm.Search (TreeGenerator.Root, branching, depth);
            }
        }
        else
            Debug.LogWarning ("Algorythm not set. Search canceled.");
    }

    public void SetAlgorythm (GamesAlgorythm algorythm) {
        this.algorythm = algorythm;
    }

    private void SetLeafStates (GamesNode node) {
        GamesNode[] leafs = GamesNode.Leafs.ToArray ();
        for (int i = 0 ; i < leafs.Length ; i++) {

            if (leafs[i].ID == node.ID) {
                leafs[i].SetState (NodeState.Active);
            }
            else if (leafs[i].nodeState == NodeState.Explored) {
                continue;
            }
            else if (leafs[i].nodeState == NodeState.Active) {
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
