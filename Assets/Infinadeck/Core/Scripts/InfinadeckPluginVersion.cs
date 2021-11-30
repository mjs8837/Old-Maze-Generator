using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class InfinadeckPluginVersion : MonoBehaviour
{
    public Text version;
    private InfinadeckSpawner spawner;

    // Update is called once per frame
    void Update()
    {
        if (!spawner) { spawner = FindObjectOfType<InfinadeckSpawner>(); }
        else { version.text = spawner.pluginVersion; }
    }
}
