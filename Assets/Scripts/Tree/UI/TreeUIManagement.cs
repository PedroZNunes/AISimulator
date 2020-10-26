using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeUIManagement : MonoBehaviour
{
    #region Events
    /// <summary>
    /// Event that calls out for the object responsible for generating the map.
    /// </summary>
    /// <param name="branching">how wide</param>
    /// <param name="depth">how high</param>
    public delegate void GenerateClicked (int branching, int depth);
    static public event GenerateClicked generateClicked;

    /// <summary>
    /// Event that calls out when you want to just reset the path the search algorithm made, and not the whole thing
    /// </summary>
    static public event Action resetClicked;

    /// <summary>
    /// Event that calls out for the object responsible for starting the search
    /// </summary>
    public delegate void SearchClicked (string algorithm, int branching, int depth);
    static public event SearchClicked searchClicked;
    #endregion

    #region OutputValues
    [SerializeField] private Text analyzedValue;
    [SerializeField] private Text prunedValue;
    [SerializeField] private Text percentValue;
    #endregion

    #region RawInput
    //map generation inputs
    [SerializeField] private InputField branchingInput;
    [SerializeField] private InputField depthInput;

    //search inputs
    [SerializeField] private Dropdown algorythmDropdownInput;
    #endregion

    #region Locals
    private int branching;
    private int depth;
    private string algorithm;

    private int maxLeafCount = 150;
    private int minBranching = 2;
    private int maxBranching = 4;
    #endregion

    static private TreeUIManagement instance;

    #region Initialization

    private void OnEnable ()
    {
        TreeSearchAlgorithm.searchEnded += CountPrunedAndAnalyzed;
    }
    private void OnDisable ()
    {
        TreeSearchAlgorithm.searchEnded -= CountPrunedAndAnalyzed;
    }

    private void Awake ()
    {
        if (instance == null)
            instance = FindObjectOfType<TreeUIManagement> ();
        if (instance != this)
            Destroy (this.gameObject);

        InitializeUI ();
        Initialize ();
        ResetOutput ();
    }

    private void Start ()
    {
        //generate a map when opening scene with standard values
        OnClickGenerate ();
    }

    private void InitializeUI ()
    {
        //map generation panel
        branchingInput.text = "2";
        depthInput.text = "4";

        //search panel
        List<string> algorythms = new List<string> ();
        algorythms.Add ("Minimax");
        algorythms.Add ("Alpha - Beta");

        algorythmDropdownInput.ClearOptions ();
        algorythmDropdownInput.AddOptions (algorythms);
        algorythmDropdownInput.value = 0;
    }

    private void Initialize ()
    {
        //map generation
        SetBranchingAndDepth ();

        //search
        SetAlgorithm ();
    }
    #endregion

    #region Input Handling

    public void SetAlgorithm () { algorithm = instance.algorythmDropdownInput.options[instance.algorythmDropdownInput.value].text; }

    /// <summary>
    /// Makes sure there's not too many nodes in the screen.
    /// </summary>
    public void SetBranchingAndDepth ()
    {
        branching = Int32.Parse(instance.branchingInput.text);
        branching = Mathf.Clamp(branching, minBranching, maxBranching);
        instance.branchingInput.text = branching.ToString ();


        depth = Int32.Parse(instance.depthInput.text);
        
        int totalLeafCount = Mathf.RoundToInt (Mathf.Pow (branching, depth));

        while (totalLeafCount > maxLeafCount) {
            depth--;
            totalLeafCount = Mathf.RoundToInt (Mathf.Pow (branching, depth));
        }

        instance.depthInput.text = depth.ToString ();
        Debug.LogFormat ("Branches: {0}, Depth: {1}", branching, depth);
    }


    #endregion

    #region Button Pressing

    public void OnClickGenerate ()
    {
        ResetOutput ();

        OnGenerateClicked ();
    }

    public void OnClickSearch ()
    {
        OnClickReset ();

        OnSearchClicked ();
    }

    public void OnClickReset ()
    {
        ResetOutput ();
        TreeNode.ResetNodes ();
        TreeBranch.ResetBranches ();

        OnResetClicked ();
    }

    public void OnClickQuit ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene (0);
    }

    private void OnGenerateClicked ()
    {
        if (generateClicked != null)
            generateClicked (branching, depth);
    }
    private void OnSearchClicked ()
    {
        if (searchClicked != null)
            searchClicked (algorithm, branching, depth);
    }
    private static void OnResetClicked ()
    {
        if (resetClicked != null)
            resetClicked ();
    }
    #endregion

    #region OutputData

    /// <summary>
    /// Count how many nodes have been cut-off from the process
    /// </summary>
    private void CountPrunedAndAnalyzed ()
    {
        int analyzed = 0;
        foreach (TreeNode leaf in TreeNode.Leaves) {
            if (leaf.State == NodeState.Explored || leaf.State == NodeState.Active)
                analyzed++;
        }

        int pruned = TreeNode.Leaves.Count - analyzed;

        float percent = (float) pruned / TreeNode.Leaves.Count * 100f;

        prunedValue.text = pruned.ToString ();
        analyzedValue.text = analyzed.ToString ();
        percentValue.text = String.Format ("{0:0.00}%", percent);
        Debug.Log ("percent" + percent);
    }

    /// <summary>
    /// Resets output values like percent, pruned, analyzed, etc
    /// </summary>
    private void ResetOutput ()
    {
        analyzedValue.text = "0";
        prunedValue.text = "0";
        percentValue.text = "0";
    }
    #endregion

}
