using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIMessageSystem : MonoBehaviour
{

    [SerializeField] private Text messageText;

    [SerializeField] private Image messageBackground;

    private string startNodeSetupMessage = "Choose the node you want to start the search from.";
    private string goalNodeSetupMessage = "Choose the node you want to search towards.";
    private string resetNodesMessage = "Start and Goal nodes have been reset.";
    private string emptyMessage = "";

    private void OnEnable()
    {
        UINodeSetup.SettingUpNodesEvent += NodeSetup;
        UINodeSetup.NodeSelectedEvent += NodeSelected;
        UINodeSetup.ResetNodesEvent += ResetNodes;
    }

    private void OnDisable()
    {
        UINodeSetup.SettingUpNodesEvent -= NodeSetup;
        UINodeSetup.NodeSelectedEvent -= NodeSelected;
        UINodeSetup.ResetNodesEvent -= ResetNodes;
    }

    private void NodeSetup(bool isStart)
    {
        if (isStart) {
            ShowMessage(startNodeSetupMessage);
        }
        else {
            ShowMessage(goalNodeSetupMessage);
        }

    }

    private void NodeSelected()
    {
        CloseMessage();
    }

    private void ResetNodes()
    {
        StartCoroutine (ShowMessage (resetNodesMessage, 5f));
    }


    private void ShowMessage(string message)
    {
        messageText.text = message;
        messageBackground.enabled = true;
    }

    private IEnumerator ShowMessage(string message, float time)
    {
        messageText.text = message;
        messageBackground.enabled = true;

        yield return new WaitForSeconds(time);

        CloseMessage();
    }

    private IEnumerator ShowMessageWithTime (float time)
    {
        messageBackground.enabled = true;

        yield return new WaitForSeconds(time);

        messageText.text = emptyMessage;
        messageBackground.enabled = false;
    }

    private void CloseMessage()
    {
        messageText.text = emptyMessage;
        messageBackground.enabled = false;

    }
}