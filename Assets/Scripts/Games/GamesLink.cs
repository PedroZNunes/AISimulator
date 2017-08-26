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


    //this is weird sayint node.links[i].getother (node)
    /// <summary>
    /// returns the other node in the link
    /// </summary>
    /// <param name="node"> the node you know is present on the link </param>
    /// <returns> the other node in the link </returns>
    public GamesNode GetOther (GamesNode node) {
        if (node == a)
            return b;
        else if (node == b)
            return a;
        else
            return null;
    }

}
