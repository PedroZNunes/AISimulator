using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSearcher : MonoBehaviour {

    private GamesAlgorythm algorythm;

    [SerializeField]
    private Sprite inactiveLink;

    [SerializeField]
    private Sprite inactiveNode;

    [SerializeField]
    private Sprite activeLink;

    [SerializeField]
    private Sprite activeNode;

    [SerializeField]
    private Sprite exploredLink;

    [SerializeField]
    private Sprite exploredNode;

    [SerializeField]
    private Sprite prunedNode;


    private static Stack<Link> activeLinks = new Stack<Link> ();
    private static Stack<Link> exploredLinks = new Stack<Link> ();
    private static Stack<Node> activeNodes = new Stack<Node> ();
    private static Stack<Node> exploredNodes = new Stack<Node> ();

    static private TreeSearcher instance;

    private void Awake () {
        instance = FindObjectOfType<TreeSearcher> ();
    }

    private void OnEnable () {
        UIGames.SearchEvent += StartSearching;
    }
    private void OnDisable () {
        UIGames.SearchEvent -= StartSearching;
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
                if (!GamesAlgorythm.IsSearching) {
                    //HardResetPathVisualization ();
                    //this.algorythm.ResetUI ();
                    Debug.LogFormat ("Reading Tree using {0}.", algorythm);
                    StartCoroutine (this.algorythm.Search (TreeGenerator.Root, branching, depth, framesPerSecond));
                }
                else {
                    Debug.LogWarning ("Another search in progress.");
                }
            }
        }
        else
            Debug.LogWarning ("Algorythm not set. Search canceled.");
    }

    public void SetAlgorythm (GamesAlgorythm algorythm) {
        this.algorythm = algorythm;
    }

    static public void VisualizePath () {
        

    }
}
