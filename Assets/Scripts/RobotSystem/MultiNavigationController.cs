using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[System.Serializable]
public class NavigationRobot {
    public string rosNamespace;
    public Color color;
    public GameObject turtlebot3Obj;
    [NonSerialized] public string mapFrameName;
    [NonSerialized] public string odomFrameName;
    [NonSerialized] public Pose mapFramePose = new Pose();
    [NonSerialized] public Pose odomFramePose = new Pose();
}

public class MultiNavigationController : MonoBehaviour
{
   // [SerializeField] TMP_Dropdown rosNamespaceDropdown;
    [SerializeField] List<NavigationRobot> navigationRobots = new List<NavigationRobot>();
    public List<NavigationRobot> NavigationRobots {
        get { return navigationRobots; }
    }
    NavigationRobot selectedRobot;
    public NavigationRobot SelectedRobot {
        get { return selectedRobot; }
    }

    void Start()
    {
       // rosNamespaceDropdown.ClearOptions();
        var dropdownOptions = new List<string>();
        for (int i = 0; i < navigationRobots.Count; i++)
        {
            var navigationRobot = navigationRobots[i];
            dropdownOptions.Add(navigationRobot.rosNamespace);
            var renderers = navigationRobot.turtlebot3Obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.color = navigationRobot.color;
            }
        }
        // rosNamespaceDropdown.AddOptions(dropdownOptions);
        // rosNamespaceDropdown.onValueChanged.AddListener(delegate {
        //     selectedRobot = navigationRobots[rosNamespaceDropdown.value];
        // });

        selectedRobot = navigationRobots[0];
    }
}
