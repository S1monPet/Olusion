using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGatherable
{
    // Returns true if the gatherable was harvested and no longer exists.
    public bool Gather(int damage);
}
