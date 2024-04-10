using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGatherable
{
    // Returns true if gatherable was gathered
    public bool Gather(int damage);
}
