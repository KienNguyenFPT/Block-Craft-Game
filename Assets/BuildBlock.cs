using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildBlock : MonoBehaviour
{
    GuyAction guyAction;
    public Transform shootingPoint;

    void Start()
    {
        guyAction = FindObjectOfType<GuyAction>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; */

            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit))
            {
                Vector3 buildPosition = hit.point + hit.normal * 0.5f;
                buildPosition = new Vector3(Mathf.RoundToInt(buildPosition.x), Mathf.RoundToInt(buildPosition.y), Mathf.RoundToInt(buildPosition.z));
                guyAction.BuildBlock(buildPosition);
            }
        }

        if (Input.GetMouseButtonDown(1) && !IsPointerOverUIObject())
        {
            /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; */

            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    guyAction.DestroyBlock(hit.transform.gameObject);
                }
            }
        }
    }
    bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
