using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIGames : MonoBehaviour {

    //events
    public delegate void GenerateMapHandler (int branching, int depth, int grain);
    static public event GenerateMapHandler GenerateMapEvent;

    public delegate void SearchHandler (string algorythm, int fps);
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
    static private InputField sBranchingInput;

    [SerializeField]
    private InputField depthInput;
    static private InputField sDepthInput;
    [SerializeField]
    private InputField grainInput;
    static private InputField sGrainInput;

    //search inputs
    [SerializeField]
    private Dropdown algorythmDropdownInput;
    static private Dropdown sAlgorythmDropdownInput;
    [SerializeField]
    private InputField fpsInput;
    static private InputField sFpsInput;

    static private UIGames instance;

    //events
    private void OnEnable () {
        GamesAlgorythm.NodeAnalyzedEvent += IncrementAnalyzed;
        GamesAlgorythm.NodeSkippedEvent += IncrementSkipped;
    }
    private void OnDisable () {
        GamesAlgorythm.NodeAnalyzedEvent -= IncrementAnalyzed;
        GamesAlgorythm.NodeSkippedEvent -= IncrementSkipped;
    }

    private void Awake () {
        if (instance == null)
            instance = this;
        else
            Destroy (this.gameObject);

        Initialize ();
        InitializeUI ();
        ResetOutput ();
    }

    static private void Initialize () {
        //map generation
        sBranchingInput = instance.branchingInput;
        sDepthInput = instance.depthInput;
        sGrainInput = instance.grainInput;

        //search
        sAlgorythmDropdownInput = instance.algorythmDropdownInput;
        sFpsInput = instance.fpsInput;
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
    }

    private void IncrementAnalyzed () {
        int analyzed = Int32.Parse (analyzedValue.text);

        analyzedValue.text = (++analyzed).ToString ();
        UpdatePercent ();
    }

    private void IncrementSkipped () {
        int skipped = Int32.Parse (skippedValue.text);

        skippedValue.text = (++skipped).ToString ();
        UpdatePercent ();
    }

    private void UpdatePercent () {
        int skipped = Int32.Parse (skippedValue.text);
        int analyzed = Int32.Parse (analyzedValue.text);
        float total = skipped + analyzed;
        float percent = skipped / total;

        percentValue.text = String.Format ("{0:0.00}%", percent);
    }

    public void GenerateMap () {
        if (GenerateMapEvent != null)
            GenerateMapEvent (Int32.Parse (sBranchingInput.text), Int32.Parse (sDepthInput.text), Int32.Parse (sGrainInput.text));
    }

    public void Search () {
        if (!GamesAlgorythm.IsSearching) {
            string algorythmString = (sAlgorythmDropdownInput.options[sAlgorythmDropdownInput.value].text);

            int fps = Int32.Parse (sFpsInput.text);

            if (SearchEvent != null)
                SearchEvent (algorythmString, fps);
        }
    }
}
