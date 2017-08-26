using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeDist : IComparable {
    public float distance;
    public Node node;

    public NodeDist ( Node node , float distance ) {
        this.distance = distance;
        this.node = node;
    }

    public int CompareTo ( object obj ) {
        if (obj == null) return 1;

        NodeDist other = obj as NodeDist;
        if (other != null)
            return this.distance.CompareTo (other.distance);
        else
            throw new ArgumentException ("Object is not valid");
    }
}

