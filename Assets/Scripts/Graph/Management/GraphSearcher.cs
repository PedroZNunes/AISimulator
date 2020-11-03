using System.Collections.Generic;
using UnityEngine;
using System;

public class GraphSearcher : MonoBehaviour {


    public static event Action ResettingPathsEvent;

    private GraphSearchAlgorithm searchAlgorithm;

    [SerializeField]
    private Sprite inactiveLinkSprite;

    [SerializeField]
    private Sprite inactiveNodeSprite;

    [SerializeField]
    private Sprite activeLinkSprite;

    [SerializeField]
    private Sprite activeNodeSprite;

    [SerializeField]
    private Sprite exploredLinkSprite;

    [SerializeField]
    private Sprite exploredNodeSprite;

    [SerializeField]
    private Sprite queuedLinkSprite;

    [SerializeField]
    private Sprite queuedNodeSprite;


    private static Stack<Link> activeLinks = new Stack<Link> ();
    private static Stack<Link> exploredLinks = new Stack<Link> ();
    private static Stack<Node> activeNodes = new Stack<Node> ();
    private static Stack<Node> exploredNodes = new Stack<Node> ();

    private Coroutine searchCoroutine;

    static private int sortingOrderCount = 1;

    static private GraphSearcher instance;

    private void OnEnable () {
        UIManager.BeginSearchEvent += BeginSearch;
        UIManager.CancelSearchEvent += CancelSearch;
        UIManager.ResetSearchEvent += ResetAllPaths;
    }

    private void OnDisable () {
        UIManager.BeginSearchEvent -= BeginSearch;
        UIManager.CancelSearchEvent -= CancelSearch;
        UIManager.ResetSearchEvent -= ResetAllPaths;
    }

    private void Awake () {
        if (instance == null) 
            instance = FindObjectOfType<GraphSearcher> ();
        if (instance != this)
            Destroy (instance.gameObject);
    }

    ~GraphSearcher() { //finalizador
        activeLinks = new Stack<Link>();
        exploredLinks = new Stack<Link>();
        activeNodes = new Stack<Node>();
        exploredNodes = new Stack<Node>();
    }

    public void BeginSearch (string algorythm, Node start, Node goal, int framesPerSecond, int beamPaths) {
        if (algorythm != null)
        {
            switch (algorythm)
            {
                case ("BFS"):
                    searchAlgorithm = new BFS();
                    break;

                case ("DFS"):
                    searchAlgorithm = new DFS();
                    break;

                case ("Hill Climbing"):
                    searchAlgorithm = new HillClimbing();
                    break;

                case ("Beam"):
                    searchAlgorithm = new Beam(beamPaths);
                    break;

                case ("Branch and Bound"):
                    searchAlgorithm = new BranchAndBound();
                    break;

                case ("A*"):
                    searchAlgorithm = new AStar();
                    break;

                default:
                    searchAlgorithm = null;
                    break;
            }

            if (searchAlgorithm != null)
            {
                ResetAllPaths();

                Debug.LogFormat("Building a path using {0}.", algorythm);
                searchCoroutine = StartCoroutine(searchAlgorithm.Search(GraphGenerator.Nodes, GraphGenerator.Size, start, goal, framesPerSecond));
            }
            else
                Debug.LogWarning("Invalid algorythm. Search canceled.");
        }
        else
            Debug.LogWarning("Algorythm not set. Search canceled.");
    }

    public void CancelSearch()
    {
        Debug.LogFormat("Process aborted by user.");

        searchAlgorithm.CancelSearch();
        StopCoroutine(searchCoroutine);
    }

    public void SetAlgorythm ( GraphSearchAlgorithm algorythm ) {
        searchAlgorithm = algorythm;
    }

    static public void VisualizePath ( Dictionary<Node , Node> cameFrom , Node current , Node start ) {
        ResetActivePath ();

        Node previous = null;

        SpriteRenderer renderer;

        while (current != start) {

            renderer = current.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = instance.activeNodeSprite;
                activeNodes.Push (current);
                if (!exploredNodes.Contains (current))
                    exploredNodes.Push (current);
            }
            else {
                Debug.LogError("Sprite renderer not found.");
            }

            previous = current;
            current = cameFrom[current];

            Link link = null;
            for (int i = 0 ; i < GraphGenerator.AllLinks.Count ; i++) {
                Link currentLink = GraphGenerator.AllLinks[i];
                if (currentLink.HasNodes (current , previous)) {
                    link = currentLink;
                    break;
                }
            }

            if (link != null) {
                //change sprite on the link's Game Object
                renderer = link.GO.GetComponent<SpriteRenderer> ();
                renderer.sortingOrder = sortingOrderCount;
                sortingOrderCount++;
                if (renderer != null) {
                    renderer.sprite = instance.activeLinkSprite;
                    activeLinks.Push (link);
                    if (!exploredLinks.Contains (link))
                        exploredLinks.Push (link);
                }
            }

        }
        //add start to explored nodes so it can be properly cleaned up afterwards (UI)
        exploredNodes.Push(start);

        renderer = current.GO.GetComponent<SpriteRenderer> ();
        if (renderer != null) {
            renderer.sprite = instance.activeNodeSprite;
            activeNodes.Push (current);
        }

    }

    //Reseta o caminho ativo e o transforma em explorado (visual)
    static private void ResetActivePath () {
        sortingOrderCount = 1;
        while (activeLinks.Count > 0) {
            Link link = activeLinks.Pop ();

            SpriteRenderer renderer = link.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = instance.exploredLinkSprite;
            }
        }

        while (activeNodes.Count > 0) {
            Node node = activeNodes.Pop ();

            SpriteRenderer renderer = node.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = instance.exploredNodeSprite;
            }
        }

    }
    
    static public void ResetAllPaths () {
        ResetActivePath ();

        while (exploredLinks.Count > 0) {
            Link link = exploredLinks.Pop ();

            SpriteRenderer renderer = link.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = instance.inactiveLinkSprite;
            }
        }

        while (exploredNodes.Count > 0) {
            Node node = exploredNodes.Pop ();

            SpriteRenderer renderer = node.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = instance.inactiveNodeSprite;
            }
        }

        if (ResettingPathsEvent != null)
            ResettingPathsEvent();

    }

}
