using UnityEngine;
using UnityEngine.UI;
using System;

public class UINodeSetup : MonoBehaviour
{


    //node setup related events
    static public event Action SettingUpNodesEvent;
    static public event Action DisableNodesEvent;

    [HideInInspector] public bool settingStartNode = false;
    [HideInInspector] public bool settingGoalNode = false;

    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject goalPrefab;

    private GameObject startNodeGO;
    private GameObject goalNodeGO;

    private Node startNode;
    private Node goalNode;

    private void OnEnable() {
        UINode.NodeSelectedEvent += NodeSelected;
    }

    private void OnDisable() {
        UINode.NodeSelectedEvent -= NodeSelected;
    }

    public void AssignStartAndGoal(out Node start, out Node goal) {
        start = startNode ?? MapGenerator.RandomNode();
        goal = goalNode ?? MapGenerator.RandomNode();

        SetStartNode(start);
        SetGoalNode(goal);
    }

    public void StartNodeSetup() {
        if (SettingUpNodesEvent != null)
            SettingUpNodesEvent();

        settingStartNode = true;
    }

    public void GoalNodeSetup() {
        if (SettingUpNodesEvent != null)
            SettingUpNodesEvent();

        settingGoalNode = true;
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

            if (DisableNodesEvent != null) {
                DisableNodesEvent();
            }
        }
        else if (settingGoalNode) {
            settingGoalNode = false;

            goalNode = node;
            SetGoalNode(goalNode);

            if (DisableNodesEvent != null) {
                DisableNodesEvent();
            }
        }
    }

    private void SetStartNode(Node node) {
        Destroy(startNodeGO);

        startNodeGO = Instantiate(startPrefab, node.GO.transform);
    }

    private void SetGoalNode(Node node) {
        Destroy(goalNodeGO);

        goalNodeGO = Instantiate(goalPrefab, node.GO.transform);
    }
}