using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infinadeck;

/**
 * ------------------------------------------------------------
 * Script to modify the appearance and position of the Infinadeck Reference Objects.
 * http://tinyurl.com/InfinadeckSDK
 * Created by Griffin Brunner & George Burger @ Infinadeck, 2019
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckReferenceObjects : MonoBehaviour
{
	public GameObject referenceRing;
	public GameObject referenceEdge;
	public GameObject referenceCenter;
    public GameObject referenceSymbols;
    public GameObject heading;
    public InfinaDATA preferences;
    public Material syncMaterial;
    public InfinadeckStatusQuerier iQuerier;
    private Renderer rendRing;
    private Renderer rendEdge;
    public GameObject deckModel;
    public Vector3 worldScale = Vector3.one;

    /**
     * Runs once on the object's first frame.
     */
    void Start() {
        preferences = this.gameObject.AddComponent<InfinaDATA>();
        preferences.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Core/";
        preferences.fileName = "inf_preferences.ini";
        preferences.all = new Dictionary<string, InfinaDATA.DataEntry>
        {
            // Reference Object Settings:
            { "deckVisibility", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "false" } },
            { "deckHeadingVisibility", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "false" } },
            { "colorblindMode", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "false" } },
            { "ringVisibility", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "true" } },
            { "edgeVisibility", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "true" } },
            { "centerVisibility", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "true" } },
            { "centerModel", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "0" } },
            { "forceCenter", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "false" } },
            { "centerX", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "0.0000" } },
            { "centerY", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "0.0000" } },
            { "centerZ", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "0.0000" } },
            { "enableFixedRingHeights", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "false" } },
            { "baseFixedRingHeight", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "31.5" } },
            { "modulusOfFixedRingHeight", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "3" } },
            { "indexOfFixedRingHeight", new InfinaDATA.DataEntry { EntryName = "ReferenceObjects", EntryValue = "0" } }
        };
        preferences.InitMe();
        if (!iQuerier)
        {
            if (!CheckForStatusInScene())
            {
                iQuerier = this.gameObject.AddComponent<InfinadeckStatusQuerier>();
            }
        }
        this.transform.localScale = worldScale;
        rendRing = referenceRing.GetComponent<Renderer>();
        rendEdge = referenceEdge.GetComponent<Renderer>();
        StartCoroutine(UpdateReferenceObjectPosition());
        StartCoroutine(UpdateObjectVisibility());
    }

    InfinadeckStatusQuerier CheckForStatusInScene()
    {
        iQuerier = FindObjectOfType<InfinadeckStatusQuerier>();
        return iQuerier;
    }

    void Update()
    {
        if (Infinadeck.Infinadeck.GetAPILock()) { SyncColor(Color.black);} ////////////////////////////////////////////////////////////////////////////// CHECK IF INIT STATE
            else if (Infinadeck.Infinadeck.GetCalibrating()) { SyncColor(Color.yellow); }
            else if (Infinadeck.Infinadeck.GetTreadmillRunState()) { SyncColor(Color.green); }
            else if (Infinadeck.Infinadeck.GetAPILock()) { SyncColor(Color.blue); }
            else { SyncColor(Color.red); }
    }

    void SyncColor(Color inColor)
    {
        syncMaterial.SetColor("_EmissionColor", inColor);
    }

    IEnumerator UpdateObjectVisibility()
    {
        while (true)
        {
            deckModel.SetActive(preferences.ReadBool("deckVisibility"));
            heading.SetActive(preferences.ReadBool("deckHeadingVisibility"));
            if (!Infinadeck.Infinadeck.CheckConnection()) ////////////////////////////////////////////////////////////////////////////// CHECK IF EMPTY STATE
            {
                rendRing.enabled = false;
                rendEdge.enabled = false;
                referenceCenter.SetActive(false);
                referenceSymbols.SetActive(false);
            }
            else if (Infinadeck.Infinadeck.GetDemoMode())
            {
                rendRing.enabled = true;
                rendEdge.enabled = true;
                referenceCenter.SetActive(true);
                referenceSymbols.SetActive(true);
            }
            else
            {
                rendRing.enabled = preferences.ReadBool("ringVisibility");
                rendEdge.enabled = preferences.ReadBool("edgeVisibility");
                referenceCenter.SetActive(preferences.ReadBool("centerVisibility"));
                referenceSymbols.SetActive(preferences.ReadBool("colorblindMode"));
            }
            int currentCenter = preferences.ReadInt("centerModel");
            foreach (Transform child in referenceCenter.transform)
            {
                if (child.GetSiblingIndex() == currentCenter) { child.gameObject.SetActive(true); }
                else { child.gameObject.SetActive(false); }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    /**
     * Infinite co-routine that cycles every 1 seconds;
     * Updates the reference object positions from the connected deck
     * and save the values as needed.
     */
    IEnumerator UpdateReferenceObjectPosition() {
        while (true) {
            Vector3 ringposition = Vector3.zero;
            if (preferences.ReadBool("forceCenter"))
            {
                this.transform.localPosition = new Vector3(preferences.ReadFloat("centerX"), 0, preferences.ReadFloat("centerZ")); // 0 out vertical axis
                referenceRing.transform.localPosition = new Vector3(0, preferences.ReadFloat("centerY"), 0); // use ONLY vertical axis
            }
            else
            {
                Ring infRing = Infinadeck.Infinadeck.GetRingValues();
                ringposition = new Vector3((float)infRing.x, (float)infRing.z, (float)infRing.y);
                this.transform.localPosition = new Vector3(ringposition.x, 0, ringposition.z); // 0 out vertical axis
                referenceRing.transform.localPosition = new Vector3(0, ringposition.y, 0); // use ONLY vertical axis
                PushCenterPreferencesToINI();
            }

            if (preferences.ReadBool("enableFixedRingHeights"))
            {
                float height = preferences.ReadFloat("baseFixedRingHeight") + preferences.ReadFloat("modulusOfFixedRingHeight") * preferences.ReadInt("indexOfFixedRingHeight");
                Debug.Log(height);
                referenceRing.transform.localPosition = new Vector3(0, height * .0254f, 0);
            }

            yield return new WaitForSeconds(5.0f);
        }
        
	}

    /**
     * Change the visibility of the Deck Ring, and save the values.
     */
    public void ToggleDeckRing()
    {
        rendRing.enabled = !rendRing.enabled;
        preferences.Write("ringVisibility", rendRing.enabled.ToString());
    }

    /**
     * Change the visibility of the Deck Edge, and save the values.
     */
    public void ToggleDeckEdge()
    {
        rendEdge.enabled = !rendEdge.enabled;
        preferences.Write("edgeVisibility", rendEdge.enabled.ToString());
    }

    /**
     * Change the visibility of the Deck Center, and save the values.
     */
    public void ToggleDeckCenter()
    {
        referenceCenter.SetActive(!referenceCenter.activeSelf);
        preferences.Write("centerVisibility", referenceCenter.activeSelf.ToString());

    }

    /**
     * Change the active Deck Center model.
     */
    public void CycleDeckCenter()
    {
        int currentChild = 0;
        foreach (Transform child in referenceCenter.transform)
        {
            if (child.gameObject.activeSelf) { currentChild = child.GetSiblingIndex(); }
        }
        int nextChild = currentChild + 1;
        if (nextChild >= referenceCenter.transform.childCount) { nextChild = 0; }
        referenceCenter.transform.GetChild(currentChild).gameObject.SetActive(false);
        referenceCenter.transform.GetChild(nextChild).gameObject.SetActive(true);
        preferences.Write("centerModel", nextChild.ToString());
    }

    /**
     * Updates the deck center preference values from the in game values.
     */
    public void PushCenterPreferencesToINI()
    {
        if ((preferences.ReadFloat("centerX") != this.transform.localPosition.x) ||
            (preferences.ReadFloat("centerY") != referenceRing.transform.localPosition.y) ||
            (preferences.ReadFloat("centerZ") != this.transform.localPosition.z))
        {
            preferences.Write("centerX", this.transform.localPosition.x.ToString());
            preferences.Write("centerY", referenceRing.transform.localPosition.y.ToString());
            preferences.Write("centerZ", this.transform.localPosition.z.ToString());
        }
    }
}