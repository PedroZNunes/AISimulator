using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Drawing;
using UnityEngine.Experimental.Rendering;

public class UIManager : MonoBehaviour {

    public delegate void GenerateMapHandler (int size, int nodeCount, int maxLinks, int grain);
    static public event GenerateMapHandler GenerateMapEvent;


    //Search related events
    public delegate void SearchHandler (string algorythm, Node start, Node goal, int framesPerSecond, int beamPaths);
    static public event SearchHandler BeginSearchEvent;
    static public event Action CancelSearchEvent;
    static public event Action ResetSearchEvent;

    //output
    [SerializeField] private Text enqueuingsValue;
    [SerializeField] private Text maxQueueSizeValue;
    [SerializeField] private Text queueSizeValue;
    [SerializeField] private Text pathLengthValue;
    [SerializeField] private Text nodesExpandedValue;

    //map generation inputs
    [SerializeField] private Dropdown sizeDropdownInput;
    static private Dropdown sSizeDropdownInput;

    static public int sSize { get; private set; }

    [SerializeField] private InputField nodeCountInput;
    static private InputField sNodeCountInput;

    [SerializeField] private InputField maxLinksInput;
    static private InputField sMaxLinksInput;
    [SerializeField] private InputField grainInput;
    static private InputField sGrainInput;

    //search inputs
    [SerializeField] private Dropdown algorythmDropdownInput;
    static private Dropdown sAlgorythmDropdownInput;
    [SerializeField] private InputField beamPathsInput;
    static private InputField sBeamPathsInput;
    [SerializeField] private Slider speedInput;
    static private Slider sSpeedInput;

    static private UIManager instance;

    //start and goal setting
    [SerializeField] private UINodeSetup nodeSetup;

    [SerializeField] private Button generateButton;
    [SerializeField] private Button searchButton;
    [SerializeField] private Button quitButton;


    [SerializeField] private TextAsset infoTextFile;
    [SerializeField] private Text infoText;


    private void OnEnable () {
        PathfindingAlgorythm.UpdateUIEvent += UpdateOutputValues;
        PathfindingAlgorythm.IncrementEnqueueEvent += IncrementEnqueue;
        PathfindingAlgorythm.AlteredSearchStateEvent += UpdateButtons;

        Pathfinder.ResettingPathsEvent += ResetOutput;
    }

    private void OnDisable () {
        PathfindingAlgorythm.UpdateUIEvent -= UpdateOutputValues;
        PathfindingAlgorythm.IncrementEnqueueEvent -= IncrementEnqueue;
        PathfindingAlgorythm.AlteredSearchStateEvent -= UpdateButtons;

        Pathfinder.ResettingPathsEvent -= ResetOutput;
    }

    private void Awake () {
        instance = FindObjectOfType<UIManager> ();
        if (instance != this)
            Destroy(this.gameObject);
        else if (instance == null)
            Debug.LogError("No UIManager found in the scene.");

        Initialize ();
        InitializeUI ();
        ResetOutput ();       
    }

    private void Start () {
        OnPressGenerateMap ();
    }

    //UI Preparation Setup
    static private void Initialize () {
        //map generation
        sSizeDropdownInput = instance.sizeDropdownInput;
        sNodeCountInput = instance.nodeCountInput;
        sMaxLinksInput = instance.maxLinksInput;
        sGrainInput = instance.grainInput;

        //search
        sAlgorythmDropdownInput = instance.algorythmDropdownInput;
        sBeamPathsInput = instance.beamPathsInput;
        sSpeedInput = instance.speedInput;
    }

    private void InitializeUI () {
        //map generation panel
        List<string> sizes = new List<string> ();
        sizes.Add ("5 x 5");
        sizes.Add ("9 x 9");
        sizes.Add ("17 x 17");
        sizes.Add ("33 x 33");
        sizes.Add ("65 x 65");
        sizes.Add ("129 x 129");

        sizeDropdownInput.ClearOptions ();
        sizeDropdownInput.AddOptions (sizes);
        sizeDropdownInput.value = 3;

        nodeCountInput.text = "100";
        maxLinksInput.text = "10";
        grainInput.text = "5";

        //search panel
        List<string> algorythms = new List<string> ();
        algorythms.Add ("BFS");
        algorythms.Add ("DFS");
        algorythms.Add ("Hill Climbing");
        algorythms.Add ("Beam");
        algorythms.Add ("Branch and Bound");
        algorythms.Add ("A*");

        algorythmDropdownInput.ClearOptions ();
        algorythmDropdownInput.AddOptions (algorythms);
        algorythmDropdownInput.value = 5;

        beamPathsInput.text = "2";
        beamPathsInput.transform.parent.gameObject.SetActive( false );

        speedInput.value = 4;
    }
    
    private void ResetOutput () {
        enqueuingsValue.text = "0"; 
        maxQueueSizeValue.text = "0";
        queueSizeValue.text = "0";
        pathLengthValue.text = "0";
        nodesExpandedValue.text = "0";
    }

    //Changing the size of the map
    public void OnChangeSize () {
        string sizeString = (sSizeDropdownInput.options[sSizeDropdownInput.value].text);
        sizeString = sizeString.Substring( 0, sizeString.IndexOf( ' ' ) );
        sSize = Int32.Parse( sizeString );

        CapMaxNodeCount ();
    }

    public void OnChangeAlgorithm() {

    }

    public void OnChangeSearchAlgorithm() {
        if (algorythmDropdownInput.options[algorythmDropdownInput.value].text.Trim() == "Beam") {
            beamPathsInput.transform.parent.gameObject.SetActive( true );
        }
        else {
            beamPathsInput.transform.parent.gameObject.SetActive( false );
        }

        //load info
        string infoString = (sAlgorythmDropdownInput.options[sAlgorythmDropdownInput.value].text);
        string infoFile = infoTextFile.text;
        //Separate text starting from the first time the algorithms name appears followed by an arrow
        int startIndex = infoFile.IndexOf( infoString + "->" );
        string textFromName = infoFile.Substring( startIndex );

        //then get it from after the arrow and before the first $, which indicates the end of a description.
        startIndex = textFromName.IndexOf( '>' ) + 1;

        int length = (textFromName.IndexOf( '$' ) > 0) ? textFromName.IndexOf( '$' ) - startIndex - 1 : textFromName.Length - startIndex - 1;
        string info = textFromName.Substring( startIndex, length );
        info = "\n" + info.Trim() + "\n";

        infoText.text = info;
    }

    public void CapMaxNodeCount () {
        if (nodeCountInput.text == "")
            nodeCountInput.text = "0";
        else {
            int count = Int32.Parse (nodeCountInput.text);
            int maxNodeCount = ((sSize * sSize) / 5);
            nodeCountInput.text = Mathf.Min (count, maxNodeCount).ToString ();
        }
    }

    //Updating right panel text, triggered by event
    private void UpdateOutputValues (int queueSize, float length) {
        //update queue info
        if (queueSize > (Int32.Parse (maxQueueSizeValue.text))) {
            maxQueueSizeValue.text = queueSize.ToString();
        }

        queueSizeValue.text = queueSize.ToString();

        //assign path length
        pathLengthValue.text = String.Format ("{0:0.00}", length);

        //increment nodes expanded
        nodesExpandedValue.text = (Int32.Parse (nodesExpandedValue.text) + 1).ToString ();
    }

    public void OnPressGenerateMap() {
        if (PathfindingAlgorythm.IsSearching)
            return;

        nodeSetup.ResetStartAndGoalNodes();

        int size = sSize;
        int nodeCount = Int32.Parse(sNodeCountInput.text);
        int maxLinks = Int32.Parse(sMaxLinksInput.text);
        int grain = Int32.Parse(sGrainInput.text);


        if (GenerateMapEvent != null)
            GenerateMapEvent(size, nodeCount, maxLinks, grain);
    }

        //Called by the Search button
    public void OnPressSearch() { 
        //if not searching yet, search
        if (!PathfindingAlgorythm.IsSearching) {
            string algorythmString = (sAlgorythmDropdownInput.options[sAlgorythmDropdownInput.value].text);

            int beams = Int32.Parse (sBeamPathsInput.text);
            float speedMult = sSpeedInput.value;

            int fps = Mathf.RoundToInt (Mathf.Pow( 2, speedMult ));
            Node start, goal;
            nodeSetup.AssignStartAndGoal(out start, out goal);

            if (BeginSearchEvent != null)
                BeginSearchEvent (algorythmString, start, goal, fps, beams);
        }
        //if already searching, cancel
        else {
            if (CancelSearchEvent != null)
                CancelSearchEvent();
            else
                Debug.LogWarning("CancelSearchEvent is empty");

        }
    }

    public void OnPressReset() {
        //reset start and goal using the uinodesteup
        UINodeSetup nodeSetup = GetComponentInChildren<UINodeSetup>();
        if (nodeSetup != null) {
            nodeSetup.ResetStartAndGoalNodes();
        }

        //reset all links on-screen to pruned
        if (ResetSearchEvent != null) {
            ResetSearchEvent();
        }
    }

    private void UpdateButtons() {
        if (PathfindingAlgorythm.IsSearching) {
            generateButton.interactable = false;
            searchButton.GetComponentInChildren<Text>().text = "Cancel";
            quitButton.interactable = false;
        }
        else {
            generateButton.interactable = true;
            searchButton.GetComponentInChildren<Text>().text = "Search";
            quitButton.interactable = true;
        }

    }

    private void IncrementEnqueue() {
        int enqueuings = Int32.Parse(enqueuingsValue.text);

        enqueuingsValue.text = (++enqueuings).ToString();
    }

    public void Quit() {
        SceneManager.LoadScene (0);
    }

}
