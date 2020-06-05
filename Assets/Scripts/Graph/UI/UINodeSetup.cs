using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


public class UINodeSetup : MonoBehaviour
{


    //node setup related events
    public delegate void SettingUpNodesHandler(bool isStart);
    static public event SettingUpNodesHandler SettingUpNodesEvent;
    static public event Action NodeSelectedEvent;

    static public event Action NodeHoveredEvent;
    static public event Action NodeHoveredExitEvent;
    

    [HideInInspector] public bool settingStartNode = false;
    [HideInInspector] public bool settingGoalNode = false;

    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject goalPrefab;

    [SerializeField] private Button StartButton;
    [SerializeField] private Button GoalButton;
    [SerializeField] private Button ResetButton;

    private GameObject tempStartNodeGO;
    private GameObject tempGoalNodeGO;

    private GameObject startNodeGO;
    private GameObject goalNodeGO;

    private Node startNode;
    private Node goalNode;

    private void OnEnable() {
        GraphSearchAlgorithm.AlteredSearchStateEvent += UpdateButtons;
        UINode.NodeSelectedEvent += NodeSelected;
        UINode.NodeHoveredEvent += NodeHovered;
        UINode.NodeHoveredExitEvent += NodeHoveredExit;
    }

    private void OnDisable() {
        GraphSearchAlgorithm.AlteredSearchStateEvent -= UpdateButtons;
        UINode.NodeSelectedEvent -= NodeSelected;
        UINode.NodeHoveredEvent -= NodeHovered;
        UINode.NodeHoveredExitEvent -= NodeHoveredExit;
    }

    public void AssignStartAndGoal(out Node start, out Node goal) {
        start = startNode ?? GraphGenerator.RandomNode();
        goal = goalNode ?? GraphGenerator.RandomNode();

        SetStartNode(start);
        SetGoalNode(goal);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {

        Vector2 mousePos = Input.mousePosition;
        print(mousePos.ToString());

        }
    }

    private void UpdateButtons()
    {
        if (GraphSearchAlgorithm.IsSearching) {
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


    public void ResetStartAndGoalNodes() {
        settingGoalNode = false;
        settingStartNode = false;

        Destroy(startNodeGO);
        Destroy(goalNodeGO);

        startNode = null;
        goalNode = null;

    }

    public void NodeHovered(Node node) {
        if (settingStartNode) {
            tempStartNodeGO = null;
            tempStartNodeGO = Instantiate(startPrefab, node.GO.transform);
            SpriteRenderer nodeSprite = tempStartNodeGO.GetComponent<SpriteRenderer>();
            if (nodeSprite != null) {
                Color halfTransparency = Color.white;
                halfTransparency.a = 0.3f;
                nodeSprite.color = halfTransparency;
            }

        } else if (settingGoalNode) {
            tempGoalNodeGO = null;
            tempGoalNodeGO = Instantiate(goalPrefab, node.GO.transform);
            SpriteRenderer nodeSprite = tempGoalNodeGO.GetComponent<SpriteRenderer>();
            if (nodeSprite != null) {
                Color halfTransparency = Color.white;
                halfTransparency.a = 0.3f;
                nodeSprite.color = halfTransparency;
            }

        }
    }

    public void NodeHoveredExit(Node node) {
        Destroy(tempStartNodeGO);
        Destroy(tempGoalNodeGO);
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

        //make simbol stick to near nodes half transparent while you choose
        //where the mouse is
        Vector2 mousePos = Input.mousePosition;
        print(mousePos.ToString());

        if (GraphGenerator.Nodes != null) {
            string nodesPositions = "";
            for (int i = 0; i < GraphGenerator.Nodes.Count; i++) {
                nodesPositions += GraphGenerator.Nodes[i].pos.ToString() + ", ";
            }
            print (nodesPositions);
        }
        //at X range from the mouse
        //find nearest spot
        //move the placeholder to that point

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