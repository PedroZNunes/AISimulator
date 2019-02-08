using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    private PathfindingAlgorythm searchAlgorythm;

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


    private static Stack<Link> activeLinks = new Stack<Link> ();
    private static Stack<Link> exploredLinks = new Stack<Link> ();
    private static Stack<Node> activeNodes = new Stack<Node> ();
    private static Stack<Node> exploredNodes = new Stack<Node> ();

    private Coroutine pathingCoroutine;

    static private Pathfinder instance;

    private void OnEnable () {
        UIManager.SearchEvent += StartPathing;
        UIManager.CancelSearchEvent += StopPathing;

    }
    private void OnDisable () {
        UIManager.SearchEvent -= StartPathing;
        UIManager.CancelSearchEvent -= StopPathing;

    }

    private void Awake () {
        if (instance == null) 
            instance = FindObjectOfType<Pathfinder> ();
        if (instance != this)
            Destroy (this.gameObject);
    }

    public void StartPathing (string algorythm, Node start, Node goal, int framesPerSecond, int beamPaths) {
        if (algorythm != null)
        {
            switch (algorythm)
            {
                case ("BFS"):
                    searchAlgorythm = new BFS();
                    break;

                case ("DFS"):
                    searchAlgorythm = new DFS();
                    break;

                case ("Hill Climbing"):
                    searchAlgorythm = new HillClimbing();
                    break;

                case ("Beam"):
                    searchAlgorythm = new Beam(beamPaths);
                    break;

                case ("Branch and Bound"):
                    searchAlgorythm = new BranchAndBound();
                    break;

                case ("A*"):
                    searchAlgorythm = new AStar();
                    break;

                default:
                    searchAlgorythm = null;
                    break;
            }

            if (searchAlgorythm != null)
            {
                UIResetAllPaths();
                searchAlgorythm.ResetUI();
                Debug.LogFormat("Building a path using {0}.", algorythm);
                pathingCoroutine = StartCoroutine(searchAlgorythm.Search(MapGenerator.Nodes, MapGenerator.Size, start, goal, framesPerSecond));
            }
            else
                Debug.LogWarning("Invalid algorythm. Search canceled.");
        }
        else
            Debug.LogWarning("Algorythm not set. Search canceled.");
    }

    public void StopPathing()
    {
        Debug.LogFormat("Process aborted by user.");

        searchAlgorythm.StopSearch();
        StopCoroutine(pathingCoroutine);
    }

    public void SetAlgorythm ( PathfindingAlgorythm algorythm ) {
        searchAlgorythm = algorythm;
    }

    static public void VisualizePath ( Dictionary<Node , Node> cameFrom , Node current , Node start ) {
        UIResetActivePath ();

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

            previous = current;
            current = cameFrom[current];

            Link link = null;
            for (int i = 0 ; i < MapGenerator.AllLinks.Count ; i++) {
                Link currentLink = MapGenerator.AllLinks[i];
                if (currentLink.HasNodes (current , previous)) {
                    link = currentLink;
                    break;
                }
            }

            if (link != null) {
                //change sprite on the link GO
                renderer = link.GO.GetComponent<SpriteRenderer> ();
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

    static private void UIResetActivePath () {

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

    static public void UIResetAllPaths () {
        UIResetActivePath ();

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

    }

}
