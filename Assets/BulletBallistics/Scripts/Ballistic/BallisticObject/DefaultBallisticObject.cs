using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ballistics;

public class DefaultBallisticObject : BallisticObject {

    /// <summary>
    /// initializes bullet impact at a rayhit
    /// </summary>
    /// <param name="rayHit">Impact raycasthit</param>
    /// <param name="myBObj">material data of my materialID</param>
    public override void BulletImpact(RaycastHit rayHit, BallisticObjectData myBObj)
    {
        if (myBObj.impactObject != null)
        {
            //get instance of impactObject
            GameObject impactGO = PoolManager.Instance.GetNextGameObject(myBObj.impactObject.gameObject);

            Transform impact;
            if (impactGO == null)
            {
                impact = Instantiate(myBObj.impactObject.gameObject).transform;
                impact.SetParent(PoolManager.Instance.transform);
            }
            else
            {
                impact = impactGO.transform;
            }
            impact.gameObject.SetActive(true);

            impact.position = rayHit.point;

            ImpactObject myImpact = impact.GetComponent<ImpactObject>();
            if (myImpact != null)
            {
                myImpact.Hit(myBObj, rayHit);
            }
        }
    }
}
