using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GeneralUI : MonoBehaviour
{
    [SerializeField] Toggle[] modeToggles;
    [SerializeField] Button buttonFindPath;
    [SerializeField] Button buttonClearPath;

    [SerializeField] NavigationClient client;
    [SerializeField] GraphConstructor constructor;

    void Awake()
    {
        buttonFindPath.onClick.AddListener(() =>
        {
            client.FindAndShowPath();
        });
        buttonClearPath.onClick.AddListener(() =>
        {
            client.ClearPath();
        });
        for (int i = 0; i < modeToggles.Length; i++)
        {
            int index = i;
            modeToggles[i].onValueChanged.AddListener((isOn) =>
            {
                constructor.editMode = (GraphConstructor.GraphEditMode)index;
            });
        }
    }
}
