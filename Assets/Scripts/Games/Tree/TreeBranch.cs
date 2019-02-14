using System.Collections.Generic;
using UnityEngine;

public class TreeBranch
{

    public TreeNode a { get; private set; }
    public TreeNode b { get; private set; }

    public NodeState State { get; private set; }
    public GameObject GO;

    static private List<TreeBranch> branches = new List<TreeBranch> ();
    static public List<TreeBranch> Branches { get { return branches; } }

    public TreeBranch (TreeNode a, TreeNode b)
    {
        this.a = a;
        this.b = b;

        State = NodeState.Inactive;

        branches.Add (this);
    }

    /// <summary>
    /// Sets the Branch State (Explored, Active, Pruned or Inactive)
    /// </summary>
    /// <param name="nodeState"></param>
    public void SetState (NodeState branchState)
    {
        this.State = branchState;
    }

    static public void ResetBranches ()
    {
        foreach(TreeBranch branch in Branches) {
            branch.SetState (NodeState.Inactive);
        }
    }

    static public void Reset ()
    {
        branches = new List<TreeBranch> ();
    }

    public bool Contains (TreeNode node)
    {
        return (a == node || b == node);
    }

}
