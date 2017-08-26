using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Min, Max }

public class GamesNode {

    static public int Count { get; private set; }

    public int ID { get; private set; }

    [HideInInspector]
    public GameObject GO;

    public GamesNode parent;

    public NodeType nodeType { get; private set; }
    public GamesLink[] links { get; private set; }

    public int value;

    public int alpha;
    public int beta;

    public int depth { get; private set; }

    private float randomSeed;

    public GamesNode (int branching, int depth, int currentDepth, NodeType type, GamesNode parent) {
        ID = Count++;

        nodeType = type;

        value = (nodeType == NodeType.Max) ? int.MinValue : int.MaxValue;

        this.parent = parent;

        alpha = int.MinValue;
        beta = int.MaxValue;

        randomSeed = Random.value;

        this.depth = currentDepth;

        if (currentDepth < depth) {
            links = new GamesLink[branching];
            for (int i = 0 ; i < links.Length ; i++) {
                NodeType newType = ((currentDepth + 1) % 2 == 0) ? NodeType.Max : NodeType.Min;
                links[i] = new GamesLink (this, new GamesNode (branching, depth, currentDepth + 1, newType, this));
            }
        }
        else {
            links = new GamesLink[0];
            value = Random.Range (0, 20);
        }
    }




    public static bool operator == (GamesNode a, GamesNode b) {
        return (a.GetHashCode () == b.GetHashCode ());
    }

    public static bool operator != (GamesNode a, GamesNode b) {
        return (a.GetHashCode () != b.GetHashCode ());
    }

    public bool Equals (GamesNode other) {
        return Equals (other, this);
    }

    public override bool Equals (object obj) {
        if (obj == null || GetType () != obj.GetType ()) {
            return false;
        }

        GamesNode other = (GamesNode) obj;

        return (other.GetHashCode () == this.GetHashCode ());
    }

    public override int GetHashCode () {
        return randomSeed.GetHashCode ();
    }
}
