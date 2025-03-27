using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHeat : MonoBehaviour
{
    [SerializeField]
    Renderer[] heatLevel;
    [SerializeField]
    Material coolMaterial;
    [SerializeField]
    Material hotMaterial;
    [SerializeField]
    ShootingScript shootingScript;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        //another thing we can edit
        if (shootingScript.ShotsFired > 19) heatLevel[0].material = hotMaterial;
        else heatLevel[0].material = coolMaterial;
        if (shootingScript.ShotsFired > 39) heatLevel[1].material = hotMaterial;
        else heatLevel[1].material = coolMaterial;
        if (shootingScript.ShotsFired > 59) heatLevel[2].material = hotMaterial;
        else heatLevel[2].material = coolMaterial;
        if (shootingScript.ShotsFired > 79) heatLevel[3].material = hotMaterial;
        else heatLevel[3].material = coolMaterial;
        if (shootingScript.ShotsFired > 99) heatLevel[4].material = hotMaterial;
        else heatLevel[4].material = coolMaterial;
    }
}
