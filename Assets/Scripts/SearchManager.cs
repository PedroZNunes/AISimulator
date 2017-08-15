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
    private int framesPerSecond = 30;

    private static Stack<Link> linksChanged = new Stack<Link> ();
    private static Stack<Node> nodesChanged = new Stack<Node> ();

    static private SearchManager instance;

    private void Awake () {
        instance = FindObjectOfType<SearchManager> ();
    }

    public void StartPathing ( int size , Node start , Node goal , bool trackVisitedNodes ) {
        if (searchAlgorythm != null)
            StartCoroutine (searchAlgorythm.Search (MapGenerator.nodes , size , start , goal , framesPerSecond , trackVisitedNodes));
        else
            Debug.LogError ("Algorythm not set. Search canceled.");
    }

    public void SetAlgorythm ( SearchAlgorythm algorythm ) {
        searchAlgorythm = algorythm;
    }

    static public void VisualizePath ( Dictionary<Node , Node> cameFrom , Node current , Node start ) {

        ResetPathVisualization ();

        Sprite activeLink = instance.activeLink;
        Sprite activeNode = instance.activeNode;
        
        Node previous = null;

        SpriteRenderer renderer;

        while (current != start) {

            renderer = current.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = activeNode;
                nodesChanged.Push (current);
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
                }
            }

        }

        renderer = current.GO.GetComponent<SpriteRenderer> ();
        if (renderer != null) {
            renderer.sprite = activeNode;
            nodesChanged.Push (current);
        }

    }

    static private void ResetPathVisualization () {
        Sprite inactiveLink = instance.inactiveLink;
        Sprite inactiveNode = instance.inactiveNode;

        while (linksChanged.Count > 0) {
            Link link = linksChanged.Pop ();

            SpriteRenderer renderer = link.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = inactiveLink;
            }
        }

        while (nodesChanged.Count > 0) {
            Node node = nodesChanged.Pop ();

            SpriteRenderer renderer = node.GO.GetComponent<SpriteRenderer> ();
            if (renderer != null) {
                renderer.sprite = inactiveNode;
            }
        }

    }


}
