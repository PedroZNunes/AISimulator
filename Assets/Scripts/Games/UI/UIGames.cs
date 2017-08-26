using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIGames : MonoBehaviour {

    //events
    public delegate void GenerateMapHandler (int branching, int depth, int grain);
    static public event GenerateMapHandler GenerateMapEvent;

    public delegate void SearchHandler (string algorythm, int branching, int depth, int fps);
    static public event SearchHandler SearchEvent;

    //output
    [SerializeField]
    private Text analyzedValue;
    [SerializeField]
    private Text skippedValue;
    [SerializeField]
    private Text percentValue;

    //map generation inputs
    [SerializeField]
    private InputField branchingInput;

    [SerializeField]
    private InputField depthInput;
    [SerializeField]
    private InputField grainInput;

    //search inputs
    [SerializeField]
    private Dropdown algorythmDropdownInput;
    [SerializeField]
    private InputField fpsInput;

    public int branching { get; private set; }
    public int depth { get; private set; }
    public int grain { get; private set; }
    public string algorythm { get; private set; }
    public int fps { get; private set; }


    static private UIGames instance;

    //events
    private void OnEnable () {
        GamesAlgorythm.NodeAnalyzedEvent += IncrementAnalyzed;
        GamesAlgorythm.SkippedNodesEvent += CalculateSkipped;
    }
    private void OnDisable () {
        GamesAlgorythm.NodeAnalyzedEvent -= IncrementAnalyzed;
        GamesAlgorythm.SkippedNodesEvent -= CalculateSkipped;
    }

    private void Awake () {
        if (instance == null)
            instance = this;
        else
            Destroy (this.gameObject);

        InitializeUI ();
        Initialize ();
        ResetOutput ();
    }

    private void Initialize () {
        //map generation
        SetBranching ();
        SetDepth ();
        SetGrain ();

        //search
        SetFPS ();
        SetAlgorythm ();
    }

    private void InitializeUI () {
        //map generation panel
        branchingInput.text = "2";
        depthInput.text = "4";
        grainInput.text = "0";

        //search panel
        List<string> algorythms = new List<string> ();
        algorythms.Add ("Minimax");
        algorythms.Add ("Alpha - Beta");

        algorythmDropdownInput.ClearOptions ();
        algorythmDropdownInput.AddOptions (algorythms);
        algorythmDropdownInput.value = 0;

        fpsInput.text = "2";
    }

    private void ResetOutput () {
        analyzedValue.text = "0";
        skippedValue.text = "0";
        percentValue.text = "0";
    }

    public void SetGrain () { grain = Int32.Parse (instance.grainInput.text); }

    public void SetFPS () { fps = Int32.Parse (instance.fpsInput.text); }

    public void SetBranching () { branching = Int32.Parse (instance.branchingInput.text); }

    public void SetDepth () { depth = Int32.Parse (instance.depthInput.text); }

    public void SetAlgorythm () { algorythm = instance.algorythmDropdownInput.options[instance.algorythmDropdownInput.value].text; }

    private void IncrementAnalyzed () {
        int analyzed = Int32.Parse (analyzedValue.text);

        analyzedValue.text = (++analyzed).ToString ();
    }

    private void CalculateSkipped () {
        int analyzed = Int32.Parse (analyzedValue.text);
        float total = Mathf.Pow (branching, depth);

        skippedValue.text = (total - analyzed).ToString ();
        UpdatePercent ();
    }

    private void UpdatePercent () {
        int skipped = Int32.Parse (skippedValue.text);
        float total = Mathf.Pow (branching, depth);
        float percent = skipped / total * 100;

        percentValue.text = String.Format ("{0:0.00}%", percent);
    }

    public void GenerateMap () {
        ResetOutput ();

        if (GenerateMapEvent != null)
            GenerateMapEvent (branching, depth, grain);
    }

    public void Search () {
        ResetOutput ();

        if (!GamesAlgorythm.IsSearching) {

            if (SearchEvent != null)
                SearchEvent (algorythm, branching, depth, fps);
        }
    }

}
