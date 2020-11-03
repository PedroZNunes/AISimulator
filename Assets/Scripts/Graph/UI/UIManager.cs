using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

using Random = System.Random;

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

    static public int size { get; private set; }

    [SerializeField] private InputField nodeCountInput;
    [SerializeField] private InputField maxLinksInput;

    static private int nodeCount;
    static private int maxLinks;

    //search inputs
    [SerializeField] private Dropdown algorithmsDropdownInput;
    [SerializeField] private InputField beamsInput;
    [SerializeField] private Slider speedInput;

    static private string algorithm;
    static private int beams;
    static private int speed;

    static private UIManager instance;

    //start and goal setting
    [SerializeField] private UINodeSetup nodeSetup;

    [SerializeField] private Button generateButton;
    [SerializeField] private Button searchButton;
    [SerializeField] private Button quitButton;


    [SerializeField] private TextAsset infoTextFile;
    [SerializeField] private Text infoText;

    [SerializeField] private Text AddInfoLabel;
    [SerializeField] private Text AddInfoValue;


    private void OnEnable () {
        GraphSearchAlgorithm.UpdateUIEvent += UpdateOutputValues;
        GraphSearchAlgorithm.IncrementEnqueueEvent += IncrementEnqueue;
        GraphSearchAlgorithm.AlteredSearchStateEvent += UpdateButtons;

        GraphSearcher.ResettingPathsEvent += ResetOutput;
    }

    private void OnDisable () {
        GraphSearchAlgorithm.UpdateUIEvent -= UpdateOutputValues;
        GraphSearchAlgorithm.IncrementEnqueueEvent -= IncrementEnqueue;
        GraphSearchAlgorithm.AlteredSearchStateEvent -= UpdateButtons;

        GraphSearcher.ResettingPathsEvent -= ResetOutput;
    }

    private void Awake () {
        instance = FindObjectOfType<UIManager> ();
        if (instance != this)
            Destroy(this.gameObject);
        else if (instance == null)
            Debug.LogError("No UIManager found in the scene.");

        InitializeUI ();
        Initialize ();
        ResetOutput ();       
    }

    private void Start () {
        OnPressGenerateMap ();
    }

    //UI Preparation Setup
    private void Initialize () {
        //map generation
        OnChangeSize();
        OnChangeNodeCount();

        OnChangeLinks();

        //search
        OnChangeAlgorithm();
        OnChangeBeams();
        OnChangeSpeed();
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

        //search panel
        List<string> algorithms = new List<string> {
            "BFS",
            "DFS",
            "Hill Climbing",
            "Beam",
            "Branch and Bound",
            "A*"
        };

        algorithmsDropdownInput.ClearOptions ();
        algorithmsDropdownInput.AddOptions (algorithms);
        algorithmsDropdownInput.value = 5;

        beamsInput.text = "2";
        beamsInput.transform.parent.gameObject.SetActive( false );

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
        string sizeString = (sizeDropdownInput.options[sizeDropdownInput.value].text);
        sizeString = sizeString.Substring( 0, sizeString.IndexOf( ' ' ) );
        size = Int32.Parse( sizeString );

        CapMaxNodeCount ();
    }


    public void OnChangeAlgorithm() {
        algorithm = algorithmsDropdownInput.options[algorithmsDropdownInput.value].text.Trim();
        if (algorithm == "Beam") {
            beamsInput.transform.parent.gameObject.SetActive( true );
        }
        else {
            beamsInput.transform.parent.gameObject.SetActive( false );
        }

        //load info
        string infoFile = infoTextFile.text;

        //Separate text starting from the first time the algorithms name appears followed by an arrow
        int startIndex = infoFile.IndexOf( algorithm + "->" );
        string textFromName = infoFile.Substring( startIndex );

        //then get it from after the arrow and before the first $, which indicates the end of a description.
        startIndex = textFromName.IndexOf( '>' ) + 1;

        int length = (textFromName.IndexOf( '$' ) > 0) ? textFromName.IndexOf( '$' ) - startIndex - 1 : textFromName.Length - startIndex - 1;
        string info = textFromName.Substring( startIndex, length );
        info = "\n" + info.Trim() + "\n";

        infoText.text = info;

        if (algorithm == "A*") {
            AddInfoLabel.text = "Est. Shortest Distance:";
            AddInfoLabel.transform.parent.gameObject.SetActive( true );
        }
        else {
            AddInfoLabel.text = "";
            AddInfoLabel.transform.parent.gameObject.SetActive( false );
        }

    }

    public void OnChangeSpeed() {
        speed = Mathf.RoundToInt( speedInput.value );
    }

    public void OnChangeBeams() {
        beams = Int32.Parse( beamsInput.text );
    }

    public void OnChangeNodeCount() {
        CapMaxNodeCount();
        nodeCount = Int32.Parse( nodeCountInput.text );
    }

    public void OnChangeLinks() {
        maxLinks = Mathf.Clamp( Int32.Parse( maxLinksInput.text ), 3, 20);
        maxLinksInput.text = maxLinks.ToString();
    }

    public void CapMaxNodeCount () {
        if (nodeCountInput.text == "")
            nodeCountInput.text = "0";
        else {
            int count = Int32.Parse (nodeCountInput.text);
            int maxNodeCount = ((size * size) / 5);
            nodeCountInput.text = Mathf.Clamp( count, 3, maxNodeCount ).ToString();
        }
    }

    //Updating right panel text, triggered by event
    private void UpdateOutputValues( int queueSize, float length, float addInfoValue = 0f) {
        //update queue info
        if (queueSize > (Int32.Parse (maxQueueSizeValue.text))) {
            maxQueueSizeValue.text = queueSize.ToString();
        }

        queueSizeValue.text = queueSize.ToString();

        //assign path length
        pathLengthValue.text = String.Format ("{0:0.00}", length);

        //increment nodes expanded
        nodesExpandedValue.text = (Int32.Parse (nodesExpandedValue.text) + 1).ToString ();

        //additional information
        if (AddInfoValue.isActiveAndEnabled)
            AddInfoValue.text = String.Format( "{0:0.00}", addInfoValue );
    }

    public void OnPressGenerateMap() {
        if (GraphSearchAlgorithm.IsSearching)
            return;

        nodeSetup.ResetStartAndGoalNodes();
        int grain = new Random().Next( 1, 99 );
        if (GenerateMapEvent != null)
            GenerateMapEvent(size, nodeCount, maxLinks, grain);
    }

        //Called by the Search button
    public void OnPressSearch() { 
        //if not searching yet, search
        if (!GraphSearchAlgorithm.IsSearching) {

            int fps = Mathf.RoundToInt (Mathf.Pow( 2, speed ));
            Node start, goal;
            nodeSetup.AssignStartAndGoal(out start, out goal);

            if (BeginSearchEvent != null)
                BeginSearchEvent (algorithm, start, goal, fps, beams);
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
        if (GraphSearchAlgorithm.IsSearching) {
            generateButton.interactable = false;
            searchButton.GetComponentInChildren<Text>().text = "Cancel";
            quitButton.interactable = false;
            algorithmsDropdownInput.interactable = false;
            maxLinksInput.interactable = false;
            nodeCountInput.interactable = false;
            sizeDropdownInput.interactable = false;
            speedInput.interactable = false;
            beamsInput.interactable = false;
        }
        else {
            generateButton.interactable = true;
            searchButton.GetComponentInChildren<Text>().text = "Search";
            quitButton.interactable = true;
            algorithmsDropdownInput.interactable = true;
            maxLinksInput.interactable = true;
            nodeCountInput.interactable = true;
            sizeDropdownInput.interactable = true;
            speedInput.interactable = true;
            beamsInput.interactable = true;
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
