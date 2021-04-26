using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATask : MonoBehaviour
{
    public abstract IEnumerator Interact();
    public abstract void CompleteTask();
    public abstract void FailTask();
    public abstract string GetNameOfTask();
    public abstract bool isCompleted();
}
