﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPointTrigger : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.blue;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(LogUtility.MakeLogStringFormat("CheckPointTrigger", "Enter CheckPoint"));
            CheckPointManager.Instance.Save(gameObject.transform, this);
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        var size = gameObject.GetComponent<BoxCollider2D>().size;
        var offset = gameObject.GetComponent<BoxCollider2D>().offset;
        var pos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z);
        Gizmos.DrawCube(pos, size);
    }
}
