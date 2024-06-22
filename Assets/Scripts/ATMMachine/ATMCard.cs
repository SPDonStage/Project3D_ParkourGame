using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ATMCard : MonoBehaviour
{
    [SerializeField] private bool isDropped;
    [SerializeField] private GameObject auraHitbox;
    private Transform itemHolder;
    // Start is called before the first frame update
    void Start()
    {
        isDropped = false;
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 50 * Time.deltaTime);
        if (isDropped)
        {
            auraHitbox.gameObject.SetActive(true);
            if (transform.parent != null)
            {
                transform.parent = null;
                transform.localScale = Vector3.one;
            }
        }
        else
        {

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            auraHitbox.SetActive(false);
            itemHolder = other.transform.Find("Item Holder");
            transform.parent = itemHolder;
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(.3f, .3f, .3f);
            isDropped = false;
        }
    }
}
