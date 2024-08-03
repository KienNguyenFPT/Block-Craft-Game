using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlowingSphere : MonoBehaviour
{
    public float hoverHeight = 0.3f;
    public float hoverSpeed = 0.5f;
    public float rotationSpeed = 50.0f;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        float newY = originalPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GuyInventory inventory = other.GetComponent<GuyInventory>();
            if (inventory != null)
            {
                if (CompareTag("Weapon"))
                {
                    inventory.AddWeapon(gameObject.name);
                    Destroy(gameObject);
                }
                if (CompareTag("Block"))
                {
                    inventory.AddItem(gameObject.name, ItemType.Block);
                    Destroy(gameObject);
                }
            }
        }
    }
}
