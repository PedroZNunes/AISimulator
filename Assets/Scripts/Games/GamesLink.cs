using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesLink {

    public GamesNode a { get; private set; }
    public GamesNode b { get; private set; }

    public GameObject go;

    public GamesLink (GamesNode a, GamesNode b) {
        this.a = a;
        this.b = b;
    }

    public bool Contains (GamesNode node) {
        return (a == node || b == node);
    }

}
