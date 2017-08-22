using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchManager : MonoBehaviour {

    private PathfindingAlgorythm searchAlgorythm;

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


    private static Stack<Link> activeLinks = new Stack<Link> ();
    private static Stack<Link> exploredLinks = new Stack<Link> ();
    private static Stack<Node> activeNodes = new Stack<Node> ();
    private static Stack<Node> exploredNodes = new Stack<Node> ();

    static private SearchManager instance;

    private void Awake () {
        instance = FindObjectOfType<SearchManager> ();
    }

    private void OnEnable () {
        UIManager.SearchEvent += StartPathing;
    }

    private void OnDisable () {
        UIManager.SearchEvent -= StartPathing;
    }

    public void StartPathing (string algorythm, Node start, Node goal, int framesPerSecond, int beamPaths) {
        if (algorythm != null) {
            switch (algorythm) {
                case ("BFS"):
                    searchAlgorythm = new BFS ();
                    break;

                case ("DFS"):
                    searchAlgorythm = new DFS ();
                    break;

                case ("Hill Climbing"):
                    searchAlgorythm = new HillClimbing ();
                    break;

                case ("Beam"):
                    searchAlgorythm = new Beam (beamPaths);
                    break;

                case ("Branch and Bound"):
                    searchAlgorythm = new BranchAndBound ();
                    break;

                case ("A*"):
                    searchAlgorythm = new AStar ();
                    break;

                default:
                    break;
            }

            if (searchAlgorythm != null) {
                if (!PathfindingAlgorythm.IsSearching) {
                    HardResetPathVisualization ();
                    searchAlgorythm.ResetUI ();
                    Debug.LogFormat ("Building a path using {0}.", algorythm);
                    StartCoroutine (searchAlgorythm.Search (MapGenerator.Nodes, MapGenerator.Size, start, goal, framesPerSecond));
                }
                else {
                    Debug.LogWarning ("Another search in progress.");
                }
            }
        }
        else
            Debug.LogWarning ("Algorythm not set. Search canceled.");
    }

    public void SetAlgorythm ( PathfindingAlgorythm algorythm ) {
        searchAlgorythm = algorythm;
    }

    static public void VisualizePath ( Dictionary<Node , Node> cameFrom , Node current , Node start ) {
        SoftResetPathVisualization ();

        Sprite activeLink = instance.activeLink;
        Sprite activeNode = instance.activeNode;
        
        Node previous = null;

        SpriteRenderer renderer;

        while (current != start) {

            renderer = current.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = activeNode;
                activeNodes.Push (current);
                if (!exploredNodes.Contains (current))
                    exploredNodes.Push (current);
            }

            previous = current;
            current = cameFrom[current];

            Link link = null;
            for (int i = 0 ; i < MapGenerator.allLinks.Count ; i++) {
                Link currentLink = MapGenerator.allLinks[i];
                if (currentLink.HasNodes (current , previous)) {
                    link = currentLink;
                    break;
                }
            }

            if (link != null) {
                //change sprite on the link GO
                renderer = link.GO.GetComponent<SpriteRenderer> ();
                if (renderer != null) {
                    renderer.sprite = activeLink;
                    activeLinks.Push (link);
                    if (!exploredLinks.Contains (link))
                        exploredLinks.Push (link);
                }
            }

        }

        renderer = current.GO.GetComponent<SpriteRenderer> ();
        if (renderer != null) {
            renderer.sprite = activeNode;
            activeNodes.Push (current);
        }

    }

    static private void SoftResetPathVisualization () {
        Sprite exploredLink = instance.exploredLink;
        Sprite exploredNode = instance.exploredNode;

        while (activeLinks.Count > 0) {
            Link link = activeLinks.Pop ();

            SpriteRenderer renderer = link.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = exploredLink;
            }
        }

        while (activeNodes.Count > 0) {
            Node node = activeNodes.Pop ();

            SpriteRenderer renderer = node.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = exploredNode;
            }
        }

    }

    static public void HardResetPathVisualization () {
        SoftResetPathVisualization ();

        Sprite inactiveLink = instance.inactiveLink;
        Sprite inactiveNode = instance.inactiveNode;

        while (exploredLinks.Count > 0) {
            Link link = exploredLinks.Pop ();

            SpriteRenderer renderer = link.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = inactiveLink;
            }
        }

        while (exploredNodes.Count > 0) {
            Node node = exploredNodes.Pop ();

            SpriteRenderer renderer = node.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = inactiveNode;
            }
        }

    }

}
