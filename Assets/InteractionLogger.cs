using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Networking; // Import this namespace to use UnityWebRequest
using System.Collections;

public class InteractionLogger : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private string endpointUrl = "https://mongo-endpoint-production.up.railway.app/api/interactions";

    void Start()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to the select entered and exited events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Log the grab interaction
        LogInteraction("grabbed");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Log the release interaction
        LogInteraction("released");
    }

    private void LogInteraction(string interactionType)
    {
        // Create the interaction data
        InteractionData data = new InteractionData
        {
            objectName = gameObject.name,
            interactionType = interactionType,
            timestamp = System.DateTime.UtcNow.ToString("o") // ISO 8601 format
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(data);

        // Send the interaction data to the server
        StartCoroutine(SendInteractionData(jsonData));
    }

    IEnumerator SendInteractionData(string jsonData)
    {
        // Create a UnityWebRequest for posting the JSON data
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(endpointUrl, jsonData))
        {
            // Set the content type to JSON
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending interaction data: " + webRequest.error);
            }
            else
            {
                Debug.Log("Interaction data sent successfully");
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the select entered and exited events
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    // Define a class to match the interaction data format
    [System.Serializable]
    private class InteractionData
    {
        public string objectName;
        public string interactionType;
        public string timestamp;
    }
}