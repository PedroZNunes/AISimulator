using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchManager : MonoBehaviour {

    private SearchAlgorythm searchAlgorythm;

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
    private int framesPerSecond = 30;

    [SerializeField]
    private int beamMaxPaths = 3;

    private static Stack<Link> linksChanged = new Stack<Link> ();
    private static Stack<Link> allLinksChanged = new Stack<Link> ();
    private static Stack<Node> nodesChanged = new Stack<Node> ();
    private static Stack<Node> allNodesChanged = new Stack<Node> ();

    static private SearchManager instance;

    private void Awake () {
        instance = FindObjectOfType<SearchManager> ();
    }

    private void Update () {
        GetInput ();
    }

    private void GetInput () {
        //setting up algorythm
        if (Input.GetKeyDown (KeyCode.Alpha1)) {
            Debug.Log ("Algorythm set to BrittishMuseum");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha2)) {
            SetAlgorythm (new BFS ());
            Debug.Log ("Algorythm set to BFS");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha3)) {
            SetAlgorythm (new DFS ());
            Debug.Log ("Algorythm set to DFS");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha4)) {
            SetAlgorythm (new HillClimbing ());
            Debug.Log ("Algorythm set to HillClimbing");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha5)) {
            SetAlgorythm (new Beam (beamMaxPaths));
            Debug.Log ("Algorythm set to Beam");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha6)) {
            SetAlgorythm (new BranchAndBound ());
            Debug.Log ("Algorythm set to Branch and Bound");
        }
        else if (Input.GetKeyDown (KeyCode.Alpha7)) {
            SetAlgorythm (new AStar ());
            Debug.Log ("Algorythm set to A*");
        }
        

    }

    public void StartPathing ( int size , Node start , Node goal , bool trackVisitedNodes ) {
        if (searchAlgorythm != null) {
            HardResetPathVisualization ();
            searchAlgorythm.ResetUI ();
            StartCoroutine (searchAlgorythm.Search (MapGenerator.nodes, size, start, goal, framesPerSecond, trackVisitedNodes));
        }
        else
            Debug.LogError ("Algorythm not set. Search canceled.");
    }

    public void SetAlgorythm ( SearchAlgorythm algorythm ) {
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
                nodesChanged.Push (current);
                allNodesChanged.Push (current);
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
                    linksChanged.Push (link);
                    allLinksChanged.Push (link);
                }
            }

        }

        renderer = current.GO.GetComponent<SpriteRenderer> ();
        if (renderer != null) {
            renderer.sprite = activeNode;
            nodesChanged.Push (current);
        }

    }

    static private void SoftResetPathVisualization () {
        Sprite exploredLink = instance.exploredLink;
        Sprite exploredNode = instance.exploredNode;

        while (linksChanged.Count > 0) {
            Link link = linksChanged.Pop ();

            SpriteRenderer renderer = link.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = exploredLink;
            }
        }

        while (nodesChanged.Count > 0) {
            Node node = nodesChanged.Pop ();

            SpriteRenderer renderer = node.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = exploredNode;
            }
        }

    }

    static private void HardResetPathVisualization () {
        SoftResetPathVisualization ();

        Sprite inactiveLink = instance.inactiveLink;
        Sprite inactiveNode = instance.inactiveNode;

        while (allLinksChanged.Count > 0) {
            Link link = allLinksChanged.Pop ();

            SpriteRenderer renderer = link.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = inactiveLink;
            }
        }

        while (allNodesChanged.Count > 0) {
            Node node = allNodesChanged.Pop ();

            SpriteRenderer renderer = node.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = inactiveNode;
            }
        }

    }


}
