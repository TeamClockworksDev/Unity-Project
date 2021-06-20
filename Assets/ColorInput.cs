using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        StartCoroutine(WaitForBody(Physics.RaycastAll(transform.position, input).OrderBy(hit => hit.distance).ToArray()));

    }
    IEnumerator WaitForBody(RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            yield return new WaitForSeconds(.4f);
            hit.collider.gameObject.AddComponent<Rigidbody>();
        }
    }
}