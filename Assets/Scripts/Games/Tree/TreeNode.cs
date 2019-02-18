using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Min node tests a new value against it's own and keeps the lowest.
/// The Max node does the same, but it keeps the higher one.
/// </summary>
public enum NodeType { Min, Max, Leaf }

/// <summary>
/// Inactive have not been explored yet,
/// Explored are nodes that have been looked at, but are not the "Active",
/// Pruned are nodes that have been taken out of consideration completely by
/// one of his parents
/// </summary>
public enum NodeState { Active, Inactive, Explored, Pruned }

/// <summary>
/// A "Game Node" represents each min and max on the tree.
/// The tree starts with the top node linked to a "branching" number of nodes.
/// These in turn are linked to other links and so on. Each level is represented by "depth"
/// </summary>
public class TreeNode
{
    #region Lists

    /// <summary>
    /// counter updated at every node spawn
    /// </summary>
    static public int Count { get; private set; }

    /// <summary>
    /// list of all nodes, including leaves
    /// </summary>
    static private List<TreeNode> nodes = new List<TreeNode> ();
    static public List<TreeNode> Nodes { get { return nodes; } }

    /// <summary>
    /// A leaf is a node in the last level of depth in the tree, or at the bottom of it
    /// leafs are also present in the nodes list. 
    /// </summary>
    static private List<TreeNode> leaves = new List<TreeNode> ();
    static public List<TreeNode> Leaves { get { return leaves; } }
    #endregion

    #region VariablesThatWontChangeAfterInitialization

    /// <summary>
    /// a list of this nodes' branches
    /// </summary>
    public TreeBranch[] branches { get; private set; }

    /// <summary>
    /// GO spawned for this node
    /// </summary>
    [HideInInspector] public GameObject GO;

    /// <summary>
    /// branch coming from the parent
    /// </summary>
    public TreeBranch parentBranch { get; private set; }

    /// <summary>
    /// min or max node?
    /// </summary>
    public NodeType Type { get; private set; }
    /// <summary>
    /// unique identification number
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    ///in which floor is this node located
    /// </summary>
    public int depth { get; private set; }
    /// <summary>
    /// random number for equality evaluation
    /// </summary>
    private float randomSeed;
    #endregion

    #region VariablesThatMightBeChangedBySearchAlgorithms

    //values that change.
    /// <summary>
    /// The highest value that a maximizer got this far
    /// </summary>
    public int bestScoreMax;
    /// <summary>
    /// The lowest value found that a minimizer got this far
    /// </summary>
    public int bestScoreMin;

    /// <summary>
    /// active, inactive, explored...? the UI uses this
    /// </summary>
    public NodeState State { get; private set; }

    /// <summary>
    /// the ID from the leaf where the value came from. Useful for tracing the path
    /// </summary>
    public int? leafID;
    /// <summary>
    /// The current node Score
    /// </summary>
    public int Score { get; private set; }

    #endregion


    /// <summary>
    /// recursive constructor that ends up creating all tree by itself
    /// </summary>
    /// <param name="branching">the number of links each node has down the tree</param>
    /// <param name="currentDepth">how far down the tree is this node</param>
    /// <param name="type">Max or Min</param>
    public TreeNode (int branching, int currentDepth, NodeType type)
    {
        //node gets new ID, seed, depth and type
        randomSeed = Random.value;
        ID = Count++;
        Type = type;
        this.depth = currentDepth;

        SetInitialValues ();


        if (currentDepth < TreeGenerator.depth) {
            //sets up the branches linking to child nodes
            branches = new TreeBranch[branching];
            for (int i = 0; i < branches.Length; i++) {
                NodeType newType = ((currentDepth + 1) % 2 == 0) ? NodeType.Max : NodeType.Min;
                branches[i] = new TreeBranch (this, new TreeNode (branching, currentDepth + 1, newType));
                branches[i].b.parentBranch = branches[i];
            }
            
            leafID = null;
        }
        else {
            //if its the last depth level, it becomes a leaf
            Type = NodeType.Leaf;
            branches = new TreeBranch[0];
            Score = Random.Range (0, 20);
            leafID = ID;
            leaves.Add (this);
        }

        nodes.Add (this);
    }

    #region GetsSetsAndReset

    /// <summary>
    /// Sets up variables that will be changed during search
    /// </summary>
    private void SetInitialValues ()
    {
        bestScoreMax = int.MinValue;
        bestScoreMin = int.MaxValue;
        State = NodeState.Inactive;

        //if not a leaf, change the score and th leafID
        if (depth < TreeGenerator.depth) {
            leafID = null;
            Score = (Type == NodeType.Max) ? bestScoreMax : bestScoreMin;
        }
    }

    /// <summary>
    /// Sets initial values. This is useful for resetting the same tree and re-searching it
    /// </summary>
    static public void ResetNodes ()
    {
        foreach (TreeNode node in nodes) {
            node.SetInitialValues ();
            if (TreeNode.Leaves.Contains (node)) {
                continue;
            }

            node.GO.GetComponent<UITreeNode> ().AssignScore (null);
        }
    }

    /// <summary>
    /// This gets triggered when a node (max or min) get a new best value from a child. it stores its ID and Score.
    /// </summary>
    /// <param name="node"></param>
    public void SetScore (TreeNode node)
    {
        Score = node.Score;
        leafID = node.leafID;

        if (Type == NodeType.Max) {
            bestScoreMax = node.Score;
        }
        else {
            bestScoreMin = node.Score;
        }

        GO.GetComponent<UITreeNode> ().AssignScore (Score);
    }


    /// <summary>
    /// Sets the Node State (Explored, Active, Pruned or Inactive)
    /// </summary>
    /// <param name="nodeState"></param>
    public void SetState (NodeState nodeState)
    {
        this.State = nodeState;
    }

    /// <summary>
    /// Resets all static values
    /// </summary>
    static public void Reset ()
    {
        nodes = new List<TreeNode> ();
        leaves = new List<TreeNode> ();
        Count = 0;
    }

    /// <summary>
    /// Searches for a node that matches the requested ID
    /// </summary>
    /// <param name="id">node identification number ID</param>
    /// <returns>the node</returns>
    static public TreeNode GetByID (int? id)
    {
        if (id == null) {
            return null;
        }

        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].ID == id)
                return nodes[i];
        }
        return null;
    }

    /// <summary>
    /// gets the other node connected to a branch
    /// </summary>
    /// <param name="i"> branch index </param>
    /// <returns> node linked to this one </returns>
    public TreeNode GetOtherNodeFromBranchByIndex (int i)
    {
        if (i >= branches.Length || i < 0)
            return null;

        if (this == branches[i].a)
            return branches[i].b;
        else if (this == branches[i].b)
            return branches[i].a;
        else
            return null;
    }

    /// <summary>
    /// returns the node's parent node object.
    /// </summary>
    /// <returns> the parent node </returns>
    public TreeNode GetParent ()
    {
        if (parentBranch == null)
            return null;

        if (this == parentBranch.a)
            return parentBranch.b;
        else if (this == parentBranch.b)
            return parentBranch.a;
        else
            return null;
    }
    #endregion

    #region OverridesAndOverloadOfUnityStardardCode
    /// <summary>
    /// So I can compare game nodes through == and != operators
    /// </summary>
    public static bool operator == (TreeNode a, TreeNode b)
    {
        return (a.GetHashCode () == b.GetHashCode ());
    }

    public static bool operator != (TreeNode a, TreeNode b)
    {
        return (a.GetHashCode () != b.GetHashCode ());
    }

    /// <summary>
    /// Overriding the equality checking standard code.
    /// This way they all have a unique random seed, so they are only equal to themselves.
    /// </summary>
    public bool Equals (TreeNode other)
    {
        return Equals (other, this);
    }

    public override bool Equals (object obj)
    {
        //if the type does not match, or no object
        if (obj == null || GetType () != obj.GetType ()) {
            return false;
        }

        TreeNode other = (TreeNode)obj;

        return (other.GetHashCode () == this.GetHashCode ());
    }

    public override int GetHashCode ()
    {
        return randomSeed.GetHashCode ();
    }
    #endregion
}
