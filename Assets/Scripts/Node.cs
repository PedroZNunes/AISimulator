using System.Collections.Generic;
using System;
using UnityEngine;

public class Node : IComparable {
    static private int Count = 0;

    public int ID { get; private set; }

    public Vector2 pos;
    public List<Node> links { get; private set; }

    public GameObject GO;

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

    public int CompareTo ( object obj ) {
        if (obj == null) return 1;

        Node other = obj as Node;
        if (other != null)
            return this.ID.CompareTo (other.ID);
        else
            throw new ArgumentException ("Object is not valid");
    }
}