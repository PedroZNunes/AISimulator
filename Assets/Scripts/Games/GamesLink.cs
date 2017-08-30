using System.Collections.Generic;
using UnityEngine;

public class GamesLink {

    public GamesNode a { get; private set; }
    public GamesNode b { get; private set; }

    public GameObject GO;

    static private List<GamesLink> links = new List<GamesLink> ();
    static public List<GamesLink> Links { get { return links; } }

    public GamesLink (GamesNode a, GamesNode b) {
        this.a = a;
        this.b = b;

        links.Add (this);
    }

    static public void Reset () {
        links = new List<GamesLink> ();
    }

    public bool Contains (GamesNode node) {
        return (a == node || b == node);
    }

}
