using UnityEngine;
using UnityEngine.UI;
using System;

public class UINodeSetup : MonoBehaviour
{


    //node setup related events
    public delegate void SettingUpNodesHandler(bool isStart);
    static public event SettingUpNodesHandler SettingUpNodesEvent;
    static public event Action ResetNodesEvent;
    static public event Action NodeSelectedEvent;

    [HideInInspector] public bool settingStartNode = false;
    [HideInInspector] public bool settingGoalNode = false;

    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject goalPrefab;

    [SerializeField] private Button StartButton;
    [SerializeField] private Button GoalButton;
    [SerializeField] private Button ResetButton;

    private GameObject startNodeGO;
    private GameObject goalNodeGO;

    private Node startNode;
    private Node goalNode;

    private void OnEnable() {
        PathfindingAlgorythm.AlteredSearchStateEvent += UpdateButtons;
        UINode.NodeSelectedEvent += NodeSelected;
    }

    private void OnDisable() {
        PathfindingAlgorythm.AlteredSearchStateEvent -= UpdateButtons;
        UINode.NodeSelectedEvent -= NodeSelected;
    }

    public void AssignStartAndGoal(out Node start, out Node goal) {
        start = startNode ?? MapGenerator.RandomNode();
        goal = goalNode ?? MapGenerator.RandomNode();

        SetStartNode(start);
        SetGoalNode(goal);
    }

    private void UpdateButtons()
    {
        if (PathfindingAlgorythm.IsSearching) {
            StartButton.GetComponent<Button>().interactable = false;
            GoalButton.GetComponent<Button>().interactable = false;
            ResetButton.GetComponent<Button>().interactable = false;
        }
        else {
            StartButton.GetComponent<Button>().interactable = true;
            GoalButton.GetComponent<Button>().interactable = true;
            ResetButton.GetComponent<Button>().interactable = true;
        }
    }

    public void OnPressReset()
    {
        ResetStartAndGoalNodes();

        if (ResetNodesEvent != null) {
            ResetNodesEvent();
        }
    }

    public void ResetStartAndGoalNodes() {
        settingGoalNode = false;
        settingStartNode = false;

        Destroy(startNodeGO);
        Destroy(goalNodeGO);

        startNode = null;
        goalNode = null;

    }

    //called when a node is selected using Start and Goal buttons
    public void NodeSelected(Node node) {
        if (settingStartNode) {
            settingStartNode = false;

            startNode = node;
            SetStartNode(startNode);

            if (NodeSelectedEvent != null) {
                NodeSelectedEvent();
            }
        }
        else if (settingGoalNode) {
            settingGoalNode = false;

            goalNode = node;
            SetGoalNode(goalNode);

            if (NodeSelectedEvent != null) {
                NodeSelectedEvent();
            }
        }
    }

    public void StartNodeSetup() {
        if (SettingUpNodesEvent != null)
            SettingUpNodesEvent(true);

        settingStartNode = true;
        settingGoalNode = true;
    }

    private void SetStartNode(Node node) {
        Destroy(startNodeGO);

        startNodeGO = Instantiate(startPrefab, node.GO.transform);
    }

    public void GoalNodeSetup() {
        if (SettingUpNodesEvent != null)
            SettingUpNodesEvent(false);

        settingStartNode = false;
        settingGoalNode = true;
    }

    private void SetGoalNode(Node node) {
        Destroy(goalNodeGO);

        goalNodeGO = Instantiate(goalPrefab, node.GO.transform);
    }

}