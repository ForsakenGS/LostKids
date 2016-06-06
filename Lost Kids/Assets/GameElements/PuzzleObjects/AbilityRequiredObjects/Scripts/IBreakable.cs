using UnityEngine;
using System.Collections;

public interface IBreakable  {


    void Break();

    void TakeHit();

    void TakeHit(float delay);
}
