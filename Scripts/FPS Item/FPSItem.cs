using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSItem : MonoBehaviour
{
    public virtual void EnableItem() { }
    public virtual void DisableItem() { }

    public virtual void OnFirePress() { }
    public virtual void OnFireRelease() { }
    public virtual void OnFireHold() { }
    public virtual void OnAimPress() { }
    public virtual void OnAimRelease() { }
    public virtual void OnAimHold() { }
    public virtual void OnReloadPress() { }
    public virtual void OnReloadRelease() { }
    public virtual void OnReloadHold() { }
    public virtual void OnChangeFireModePress() { }
    public virtual void OnChangeFireModeRelease() { }
    public virtual void OnChangeFireModeHold() { }
}
