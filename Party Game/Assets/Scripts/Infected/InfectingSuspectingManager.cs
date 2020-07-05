using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;

public class InfectingSuspectingManager : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Animator eyeAnimator;

    [Space(10)]
    [SerializeField] private GameLoopManager gameLoopManager;

    // Start is called before the first frame update
    private void Start()
    {
        //Setup event listeners
        gameLoopManager.AddEventListener((JSONNode GD) => new KeyValuePair<JSONNode, string>(GD["stepSpecification"], "Conclusion"), OnConclusion);
    }

    void OnEnable()
    {
        eyeAnimator.SetTrigger("Close");
    }

    private void OnConclusion()
    {
        eyeAnimator.SetTrigger("Open");
    }

    private void OpenAnimationFinished()
    {
        ServerInfo.NextStep("Infecting", "Conclusion");
    }
}
