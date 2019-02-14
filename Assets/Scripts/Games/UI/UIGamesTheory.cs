using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamesTheory : MonoBehaviour
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
    public delegate void SearchClicked (string algorithm, int branching, int depth, int fps);
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
    [SerializeField] private InputField fpsInput;
    #endregion

    #region Locals
    private int branching;
    private int depth;
    private string algorithm;
    private int fps;

    private int maxLeafCount = 81;
    private int minDepth = 2;
    private int maxDepth = 5;
    #endregion

    static private UIGamesTheory instance;

    #region Initialization

    private void OnEnable ()
    {
        GamesAlgorithm.leafActivated += IncrementAnalyzed;
        GamesAlgorithm.searchEnded += CountPrunedAndAnalyzed;
    }
    private void OnDisable ()
    {
        GamesAlgorithm.leafActivated -= IncrementAnalyzed;
        GamesAlgorithm.searchEnded -= CountPrunedAndAnalyzed;
    }

    private void Awake ()
    {
        if (instance == null)
            instance = FindObjectOfType<UIGamesTheory> ();
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

        fpsInput.text = "3";
    }

    private void Initialize ()
    {
        //map generation
        SetBranchingAndDepth ();

        //search
        SetFPS ();
        SetAlgorithm ();
    }
    #endregion

    #region Input Handling

    public void SetFPS () { fps = Int32.Parse (instance.fpsInput.text); }
    public void SetAlgorithm () { algorithm = instance.algorythmDropdownInput.options[instance.algorythmDropdownInput.value].text; }

    /// <summary>
    /// Makes sure there's not too many nodes in the screen.
    /// </summary>
    public void SetBranchingAndDepth ()
    {
        depth = Mathf.Clamp (Int32.Parse (instance.depthInput.text), minDepth, maxDepth);
        instance.depthInput.text = depth.ToString ();

        branching = Int32.Parse (instance.branchingInput.text);
        int totalLeafCount = Mathf.RoundToInt (Mathf.Pow (depth, branching));
        while (totalLeafCount > maxLeafCount) {
            branching--;
            totalLeafCount = Mathf.RoundToInt (Mathf.Pow (depth, branching));
        }

        instance.branchingInput.text = branching.ToString ();
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
            searchClicked (algorithm, branching, depth, fps);
    }
    private static void OnResetClicked ()
    {
        if (resetClicked != null)
            resetClicked ();
    }
    #endregion

    #region OutputData

    private void IncrementAnalyzed (TreeNode node)
    {
        int analyzed = Int32.Parse (analyzedValue.text);

        analyzedValue.text = (++analyzed).ToString ();
    }

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

        float percent = pruned / TreeNode.Leaves.Count * 100;

        prunedValue.text = pruned.ToString ();
        analyzedValue.text = analyzed.ToString ();
        percentValue.text = String.Format ("{0:0.00}%", percent);
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
