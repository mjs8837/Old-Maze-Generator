
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Infinadeck;

/**
 * ------------------------------------------------------------
 * Script to spawn all the backend components of the Infinadeck system.
 * http://tinyurl.com/InfinadeckSDK
 * Created by Griffin Brunner & George Burger @ Infinadeck, 2019
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckSpawner : MonoBehaviour
{
    [InfReadOnlyInEditor] public string pluginVersion = "1.8.1.0";
    [InfReadOnlyInEditor] public GameObject refObjects;
    [InfReadOnlyInEditor] public GameObject locomotion;
    private GameObject splashScreen;

	public GameObject cameraRig;
	public GameObject headset;
	public bool firstLevel = true;
	public bool movementLevel = true;
    public bool guaranteeDestroyOnLoad = false;
    public float speedGain = 1;
    public Vector3 worldScale = Vector3.one;
    public bool correctPosition = false;
    public bool correctRotation = false;
    public bool correctScale = false;

    /**
     * Runs upon the moment of creation of this object.
     */
    void Awake() {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        // Initialization and Error Checks
		if (!cameraRig)
        {
            Debug.LogWarning("INFINADECK WARNING: No CameraRig Reference Assigned, Assuming Parented to CameraRig");
            cameraRig = this.transform.parent.gameObject;
            if (!cameraRig)
            {
                Debug.LogWarning("INFINADECK WARNING: No CameraRig Reference Assigned and No Parent, Self is CameraRig");
                cameraRig = this.gameObject;
            }
        }
        else
        {
            this.transform.parent = cameraRig.transform;
        }
        if (correctPosition) { this.transform.localPosition = Vector3.zero; }
        if (correctRotation) { this.transform.localRotation = Quaternion.identity; }
        if (correctScale) { this.transform.localScale = Vector3.one; }

        if (!headset)
        {
            Debug.LogWarning("INFINADECK WARNING: No Headset Reference Assigned, Assuming Main Camera is Correct");
            headset = Camera.main.gameObject;
        }


        if (firstLevel) // Only spawn the following if actually needed this level
        {
            //Spawn Splashscreen
            /*splashScreen = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckSplashscreen") as GameObject, transform.position, Quaternion.identity);
            splashScreen.GetComponent<InfinadeckSplashscreen>().headset = headset;
            splashScreen.GetComponent<InfinadeckSplashscreen>().worldScale = worldScale;
            splashScreen.GetComponent<InfinadeckSplashscreen>().pluginVersion.text = pluginVersion;*/
        }

        // Spawn Reference Objects
        refObjects = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckReferenceObjects") as GameObject, transform.position, Quaternion.identity);
        refObjects.transform.parent = this.transform;
        refObjects.GetComponent<InfinadeckReferenceObjects>().worldScale = worldScale;

        if (movementLevel) // Only spawn the following if actually needed this level
        {
            // Spawn Locomotion
            locomotion = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckLocomotion") as GameObject, transform.position, Quaternion.identity);
            locomotion.transform.parent = this.transform;
            locomotion.GetComponent<InfinadeckLocomotion>().cameraRig = cameraRig;
            locomotion.GetComponent<InfinadeckLocomotion>().worldScale = worldScale;
            locomotion.GetComponent<InfinadeckLocomotion>().speedGain = speedGain;
        }
    }

    /**
     * Runs whenever the object is enabled.
     */
    void OnEnable()
    {
        SceneManager.sceneUnloaded += LevelChange;
    }

    /**
     * Runs whenever the object is disabled.
     */
    void OnDisable()
    {
        SceneManager.sceneUnloaded -= LevelChange;
    }

    /**
     * Runs when the level is changing or reloading.
     */
    private void LevelChange(Scene scene)
    {
        if (guaranteeDestroyOnLoad) { Destroy(this.gameObject); }
    }
}