using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {

    public delegate void GenerateMapHandler (int size, int nodeCount, int maxLinks, int grain);
    static public event GenerateMapHandler GenerateMapEvent;

    public delegate void SearchHandler (string algorythm, Node start, Node goal, int framesPerSecond, int beamPaths);
    static public event SearchHandler SearchEvent;

    static public event Action EnableNodesEvent;
    static public event Action DisableNodesEvent;


    //output
    [SerializeField]
    private Text enqueuingsValue;
    [SerializeField]
    private Text maxQueueSizeValue;
    [SerializeField]
    private Text queueSizeValue;
    [SerializeField]
    private Text pathLengthValue;
    [SerializeField]
    private Text nodesExpandedValue;

    //map generation inputs
    [SerializeField]
    private Dropdown sizeDropdownInput;
    static private Dropdown sSizeDropdownInput;
    [SerializeField]
    private InputField nodeCountInput;
    static private InputField sNodeCountInput;

    [SerializeField]
    private InputField maxLinksInput;
    static private InputField sMaxLinksInput;
    [SerializeField]
    private InputField grainInput;
    static private InputField sGrainInput;

    //search inputs
    [SerializeField]
    private Dropdown algorythmDropdownInput;
    static private Dropdown sAlgorythmDropdownInput;
    [SerializeField]
    private InputField beamPathsInput;
    static private InputField sBeamPathsInput;
    [SerializeField]
    private InputField fpsInput;
    static private InputField sFpsInput;

    static private UIManager instance;

    public bool settingStartNode = false;
    public bool settingGoalNode = false;

    private Node startNode;
    private Node goalNode;

    //events
    private void OnEnable () {
        SearchAlgorythm.UpdateUIEvent += UpdateUI;
        SearchAlgorythm.IncrementEnqueueEvent += IncrementEnqueue;
        SearchAlgorythm.ResetUIEvent += ResetUI;

        UINode.NodeSelectedEvent += NodeSelected;
    }
    private void OnDisable () {
        SearchAlgorythm.UpdateUIEvent -= UpdateUI;
        SearchAlgorythm.IncrementEnqueueEvent -= IncrementEnqueue;
        SearchAlgorythm.ResetUIEvent -= ResetUI;

        UINode.NodeSelectedEvent -= NodeSelected;
    }

    private void Awake () {
        if (instance == null)
            instance = this;

        Initialize ();

        InitializeUI ();
        ResetUI ();       
    }

    private void Start () {
        GenerateMap ();
    }

    static private void Initialize () {
        //map generation
        sSizeDropdownInput = instance.sizeDropdownInput;
        sNodeCountInput = instance.nodeCountInput;
        sMaxLinksInput = instance.maxLinksInput;
        sGrainInput = instance.grainInput;

        //search
        sAlgorythmDropdownInput = instance.algorythmDropdownInput;
        sBeamPathsInput = instance.beamPathsInput;
        sFpsInput = instance.fpsInput;
    }

    private void UpdateUI (int queueSize, float length) {
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

    private void IncrementEnqueue () {
        enqueuingsValue.text = (Int32.Parse (enqueuingsValue.text) + 1).ToString();
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
        sizes.Add ("257 x 257");

        sizeDropdownInput.ClearOptions ();
        sizeDropdownInput.AddOptions (sizes);
        sizeDropdownInput.value = 4;

        nodeCountInput.text = "250";
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
        algorythmDropdownInput.value = 0;

        beamPathsInput.text = "2";
        fpsInput.text = "3";
    }

    private void ResetUI () {
        enqueuingsValue.text = "0"; 
        maxQueueSizeValue.text = "0";
        queueSizeValue.text = "0";
        pathLengthValue.text = "0";
        nodesExpandedValue.text = "0";
    }

    public void GenerateMap () {
        string sizeString = (sSizeDropdownInput.options[sSizeDropdownInput.value].text);
        sizeString = sizeString.Substring (0, sizeString.IndexOf (' '));
        int size = Int32.Parse (sizeString);

        if (GenerateMapEvent != null)
            GenerateMapEvent (size, Int32.Parse (sNodeCountInput.text), Int32.Parse (sMaxLinksInput.text), Int32.Parse (sGrainInput.text));
    }

    public void Search () {
        string algorythmString = (sAlgorythmDropdownInput.options[sAlgorythmDropdownInput.value].text);

        int beams = Int32.Parse (sBeamPathsInput.text);
        int fps = Int32.Parse (sFpsInput.text);

        Node start = startNode ?? MapGenerator.RandomNode ();
        Node goal = goalNode ?? MapGenerator.RandomNode ();

        if (SearchEvent != null)
            SearchEvent (algorythmString, start, goal, fps, beams);
    }

    public void SetStart () {
        if (EnableNodesEvent != null)
            EnableNodesEvent ();

        settingStartNode = true;
    }

    public void SetGoal () {
        if (EnableNodesEvent != null)
            EnableNodesEvent ();

        settingGoalNode = true;
    }

    public void NodeSelected (Node node) {
        if (settingStartNode) {
            settingStartNode = false;

            startNode = node;
            if (DisableNodesEvent != null) {
                DisableNodesEvent ();
            }
        }
        else if (settingGoalNode) {
            settingGoalNode = false;

            goalNode = node;
            if (DisableNodesEvent != null) {
                DisableNodesEvent ();
            }
        }
    }
}
