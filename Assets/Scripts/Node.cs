using System.Collections.Generic;
using UnityEngine;

public class Node {
    static private int Count = 0;

    public int ID { get; private set; }

    public Vector2 pos;
    public List<Node> links { get; private set; }

    public GameObject nodeGO;

    public bool isSet;

    public Node () {
        this.pos = new Vector2 ();
        links = new List<Node> ();
        ID = ++Count;
    }

    public Node ( int x , int y ) {
        pos.x = x;
        pos.y = y;
        links = new List<Node> ();
        ID = ++Count;
    }

    public void AddLink ( Node node ) {
        links.Add (node);
    }

}