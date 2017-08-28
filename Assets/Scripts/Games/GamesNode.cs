using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Min, Max }
public enum NodeState { Active, Inactive, Explored, Pruned }

public class GamesNode {

    static public int Count { get; private set; }

    public int ID { get; private set; }

    [HideInInspector]
    public GameObject GO;

    public GamesLink parentLink = null;

    public NodeState nodeState { get; private set; }
    public NodeType nodeType { get; private set; }
    public GamesLink[] links { get; private set; }

    public int? leafID;
    public int value;

    public int alpha;
    public int beta;

    public int depth { get; private set; }

    private float randomSeed;

    static private List<GamesNode> nodes = new List<GamesNode> ();
    static public List<GamesNode> Nodes { get { return nodes; } }

    static private List<GamesNode> leafs = new List<GamesNode> ();
    static public List<GamesNode> Leafs { get { return leafs; } }


    public GamesNode (int branching, int currentDepth, NodeType type) {
        ID = Count++;

        nodeType = type;
        nodeState = NodeState.Inactive;

        value = (nodeType == NodeType.Max) ? int.MinValue : int.MaxValue;

        alpha = int.MinValue;
        beta = int.MaxValue;

        randomSeed = Random.value;

        this.depth = currentDepth;

        if (currentDepth < TreeGenerator.treeDepth) {
            links = new GamesLink[branching];
            for (int i = 0 ; i < links.Length ; i++) {
                NodeType newType = ((currentDepth + 1) % 2 == 0) ? NodeType.Max : NodeType.Min;
                links[i] = new GamesLink (this, new GamesNode (branching, currentDepth + 1, newType));
                links[i].b.parentLink = links[i];
            }
            leafID = null;
        }
        else {
            links = new GamesLink[0];
            value = Random.Range (0, 20);
            leafID = ID;
            leafs.Add (this);
        }

        nodes.Add (this);
    }

    public void SetState (NodeState nodeState) {
        this.nodeState = nodeState;
    }

    static public void Reset () {
        nodes = new List<GamesNode> ();
        leafs = new List<GamesNode> ();
        Count = 0;
    }

    static public GamesNode GetByID (int id) {
        for (int i = 0 ; i < nodes.Count ; i++) {
            if (nodes[i].ID == id)
                return nodes[i];
        }
        return null;
    }

    /// <summary>
    /// gets the other node connected to a link
    /// </summary>
    /// <param name="i">link index</param>
    /// <returns> node linked to this one </returns>
    public GamesNode GetOther (int i) {
        if (i >= links.Length || i < 0)
            return null;

        if (this == links[i].a)
            return links[i].b;
        else if (this == links[i].b)
            return links[i].a;
        else
            return null;
    }

    public GamesNode GetParent () {
        if (parentLink == null)
            return null;

        if (this == parentLink.a)
            return parentLink.b;
        else if (this == parentLink.b)
            return parentLink.a;
        else
            return null;
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
