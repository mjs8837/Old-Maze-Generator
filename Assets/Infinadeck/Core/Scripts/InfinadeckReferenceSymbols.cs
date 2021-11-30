using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InfinadeckReferenceSymbols : MonoBehaviour
{
    public GameObject reference;
    public GameObject holder;
    public float factor;
    public float min;
    public float max;
    public float minDeg;
    public float maxDeg;
    public bool showDemo;
    public GameObject symbol_deckOn;
    public GameObject symbol_deckArmed;
    public GameObject symbol_deckCalibrating;
    public GameObject symbol_APILock;
    public GameObject symbol_backup;
    public GameObject symbol_demo;
    public Text symbol_demoText;
    private Vector3 pseudoReference;
    public Vector3 worldScale = Vector3.one;

    private void Start()
    {
        
        if (FindObjectOfType<InfinadeckSpawner>())
        {
            worldScale = FindObjectOfType<InfinadeckSpawner>().worldScale;
            max *= worldScale.x;
            min *= worldScale.x;
            if (!reference) { reference = FindObjectOfType<InfinadeckSpawner>().headset; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (true) ////////////////////////////////////////////////////////////////////////////// CHECK IF NOT IN EMPTY STATE
        {
            holder.SetActive(true);
            pseudoReference = new Vector3(reference.transform.position.x,this.transform.position.y, reference.transform.position.z);
            factor = Vector3.Distance(transform.position, pseudoReference);
            if (factor > max) { factor = max; }
            else if (factor < min) { factor = min; }
            holder.transform.localEulerAngles = new Vector3(0,0,minDeg + (maxDeg-minDeg)*(factor-max)/(min-max));
            //if (false) { EnableSymbol(null); } ////////////////////////////////////////////////////////////////////////////// CHECK IF INIT STATE
            if (Infinadeck.Infinadeck.GetCalibrating()) { EnableSymbol(symbol_deckCalibrating); }
            else if (Infinadeck.Infinadeck.GetTreadmillRunState()) { EnableSymbol(symbol_deckOn); }
            else if (Infinadeck.Infinadeck.GetAPILock()) { EnableSymbol(symbol_APILock); }
            else { EnableSymbol(symbol_deckArmed); }
        }
        /*else
        {
            holder.SetActive(false);
        }*/
    }

    void EnableSymbol(GameObject correct)
    {
        symbol_deckOn.SetActive(false);
        symbol_deckArmed.SetActive(false);
        symbol_deckCalibrating.SetActive(false);
        symbol_APILock.SetActive(false);
        symbol_backup.SetActive(false);
        correct.SetActive(true);
        if (Infinadeck.Infinadeck.GetDemoMode() && showDemo)
        {
            symbol_demo.SetActive(true);
            if (showDemo)
            {
                symbol_demoText.text = Mathf.Floor(
                (float)Infinadeck.Infinadeck.GetDemoTimeRemaining() / 60).ToString()
                + ":" + Mathf.RoundToInt(
                    (float)Infinadeck.Infinadeck.GetDemoTimeRemaining() % 60).ToString("00");
            }
            else { symbol_demoText.text = null; }
        }
        else
        {
            symbol_demo.SetActive(false);
        }
    }
}
