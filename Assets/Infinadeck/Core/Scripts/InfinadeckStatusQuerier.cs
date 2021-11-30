using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinadeckStatusQuerier : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        /*if(false) //fail condition if no API seen or version mismatch
        {
            //deckStatus = IStat.empty;
        }

        else if (false) //fail condition if API seen but no deck
        {
            //deckStatus = IStat.empty;
        }

        if (Infinadeck.Infinadeck.GetAPILock())
        {
            //deckStatus = IStat.APILock;
        }
        else
        {
            if (Infinadeck.Infinadeck.GetTreadmillRunState())
            {
                deckStatus = IStat.deckOn;
            }
            else
            {
                deckStatus = IStat.deckArmed;
            }
            if (Infinadeck.Infinadeck.GetCalibrating())
            {
                deckStatus = IStat.deckCalibrating;
            }
        }
        demo = Infinadeck.Infinadeck.GetDemoMode();
        demoTime = (float)Infinadeck.Infinadeck.GetDemoTimeRemaining();
        
         
         public InfinadeckStatusQuerier iQuerier;
         if (!iQuerier)
        {
            if (!CheckForStatusInScene())
            {
                iQuerier = this.gameObject.AddComponent<InfinadeckStatusQuerier>();
            }
        }

        InfinadeckStatusQuerier CheckForStatusInScene()
    {
        iQuerier = FindObjectOfType<InfinadeckStatusQuerier>();
        return iQuerier;
    }
         */
    }
}
