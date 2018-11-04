using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PickupEvent : MonoBehaviour
{
    public GameObject ParentBody;

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: Won't work if weapon is on the ground
        if (collision.gameObject.CompareTag("Weapon"))
        {
            ParentBody.SendMessage("OnWeaponPickup", collision.gameObject);
        }
    }
}