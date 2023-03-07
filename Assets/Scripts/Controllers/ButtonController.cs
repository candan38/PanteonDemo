using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private GameObject barracks;
    [SerializeField] private GameObject powerPlant;
    private GameObject freeBuilding;
    
    
    public void clickBarracks()
    {
        //Barracks oluþturup inþaa edilene kadar freeBuilding game objesinin içerisinde tutuyoruz

        freeBuilding = GameObject.Find("FreeBuilding").gameObject;
        if (freeBuilding.transform.childCount>0)
        {
            return;
        }
        var barracksCLone = Instantiate(barracks,new Vector2(1,1) ,Quaternion.identity);
        barracksCLone.transform.SetParent(freeBuilding.transform);
    }
    public void clickPowerPlant()
    {
        //Power Plant oluþturup inþaa edilene kadar freeBuilding game objesinin içerisinde tutuyoruz

        freeBuilding = GameObject.Find("FreeBuilding").gameObject;

        if (freeBuilding.transform.childCount > 0)
        {
            return;
        }
        var powerPlantClone = Instantiate(powerPlant, new Vector2(1, 1), Quaternion.identity);
        powerPlantClone.transform.SetParent(freeBuilding.transform);
    }
}
