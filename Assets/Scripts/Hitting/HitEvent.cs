using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HitEvent : MonoBehaviour
{
    private HitMessage _message = null;


    // Constructor - use class HitMessage as parameter
    public void Initialise (HitMessage message)
    {
        _message = message;
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(collider.gameObject == gameObject.transform.root.gameObject)
                return;

            if (_message == null)
                return;

            var enemy = collider.gameObject.GetComponent<PlayerController>();
            enemy.OnHit(_message);

            // '_message' is explicitly set to null to avoid double hit registration
            _message = null;
        }
    }
}
