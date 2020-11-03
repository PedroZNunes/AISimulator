using System;

/// <summary>
/// node-distance pair
/// </summary>
public class NodeDistance : IComparable {
    public float distance;
    public Node node;

    public NodeDistance ( Node node , float distance ) {
        this.distance = distance;
        this.node = node;
    }

    public int CompareTo ( object obj ) {
        if (obj == null) return 1;

        NodeDistance other = obj as NodeDistance;
        if (other != null)
            return this.distance.CompareTo (other.distance);
        else
            throw new ArgumentException ("Object is not valid");
    }
}

