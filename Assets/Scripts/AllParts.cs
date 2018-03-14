using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;

public class AllParts : MonoBehaviour, ISpeechHandler
{
    public static int PartToPlace = 1;  // beim in der Reihenfolge erstem Teil beginnen

    private List<TinPart> allParts;  // Liste für alle Teile

    // Use this for initialization
    void Start()
    {
        // alle Blechteile in die Liste
        allParts = new List<TinPart>();
        GetComponentsInChildren(allParts);
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        switch (eventData.RecognizedText.ToLower())
        {
            case "reset":
            case "back":
            case "return":
                ResetParts();
                break;
            case "exit":
            case "quit":
            case "close":
                Application.Quit();
                break;
        }
    }

    public void ResetParts()
    {
        // alle Teile zurückbewegen die verschoben waren
        allParts.FindAll(x => x.IsResetable).ForEach(x => x.MoveBack());
        PartToPlace = 1;  // wieder von vorn beginnen
    }
}