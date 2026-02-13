using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] List<Portal> allPortals;
    [HideInInspector]public List<Portal> activePortals = new();
    public List<Portal> SetupPortalsForWave(int activesQuantity)
    {

        int portalsToActivate = Mathf.Clamp((activesQuantity - 1) / 3 + 1, 1, allPortals.Count);

        //// Deactivate all portals
        //foreach (var portal in allPortals)
        //    portal.Deactivate();

        this.activePortals.Clear();

        var shuffled = allPortals.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < portalsToActivate; i++)
        {
            //activate befor adding
            //shuffled[i].Activate();
            //Portal portal = shuffled[i].GetComponent<Portal>();
            activePortals.Add(shuffled[i]);
        }
        Debug.Log(activePortals.Count);
        return activePortals;
    }

    //public List<Portal> GetActivePortals()
    //{
    //    return activePortals;
    //}
}
