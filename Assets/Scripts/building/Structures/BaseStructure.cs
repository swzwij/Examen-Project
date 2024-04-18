using FishNet.Object;
using System.Collections;
using UnityEngine;

public class BaseStructure : NetworkBehaviour
{
    public void Initialze()
    {
        StartCoroutine(WaitBeforeActivate());
        this.gameObject.SetActive(true);
    }

    private IEnumerator WaitBeforeActivate()
    {
        yield return new WaitForSeconds(2);
        ActivateStructure();
    }

    [ObserversRpc]
    private void ActivateStructure() => this.gameObject.SetActive(true);
}