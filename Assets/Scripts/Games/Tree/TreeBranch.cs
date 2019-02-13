using System.Collections.Generic;
using UnityEngine;

public class TreeBranch
{

    public TreeNode a { get; private set; }
    public TreeNode b { get; private set; }

    public GameObject GO;

    static private List<TreeBranch> branchesUntouched;
    static private List<TreeBranch> branches = new List<TreeBranch> ();
    static public List<TreeBranch> Branches { get { return branches; } }

    public TreeBranch (TreeNode a, TreeNode b)
    {
        this.a = a;
        this.b = b;

        branches.Add (this);
    }

    static public void LoadOriginalBranchList ()
    {
        branches = branchesUntouched;
    }

    static public void SaveOriginalBranchList ()
    {
        branchesUntouched = branches;
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
