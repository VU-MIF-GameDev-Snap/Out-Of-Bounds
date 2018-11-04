using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HitEvent : MonoBehaviour
{
    private HitMessage _message = null;

    
    // Constructor - use class HitMessage as parameter
    public void Initialise (object message)
    {
        var msg = message as HitMessage;
        _message = msg == null ? null : msg;
    }


    void OnTriggerEnter(Collider collider)
    {   
        if (collider.gameObject.CompareTag("Enemy"))
        {
            if (_message == null)
                return;

            var enemy = collider.gameObject.GetComponent<PlayerController>();
            enemy.SendMessage("ReceiveHit", _message);

            // '_message' is explicitly set to null to avoid double hit registration
            _message = null;
        }
    }
}