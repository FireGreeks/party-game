using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngoreZTestCanvas : MonoBehaviour
{
    enum GameObjectNature
    {
        Image, Text
    }

    [SerializeField] private GameObjectNature nature = GameObjectNature.Image;

    // Start is called before the first frame update
    void Start()
    {
        if (nature == GameObjectNature.Image)
        {
            Material existingGlobalMat = gameObject.GetComponent<Image>().materialForRendering;
            Material updatedMaterial = new Material(existingGlobalMat);
            updatedMaterial.SetInt("unity_GUIZTestMode", 0);
            gameObject.GetComponent<Image>().material = updatedMaterial;
        }
        else if (nature == GameObjectNature.Text)
        {
            Material existingGlobalMat = gameObject.GetComponent<TextMeshProUGUI>().materialForRendering;
            Material updatedMaterial = new Material(existingGlobalMat);
            updatedMaterial.SetInt("unity_GUIZTestMode", 0);
            gameObject.GetComponent<TextMeshProUGUI>().material = updatedMaterial;
        }
    }

}
