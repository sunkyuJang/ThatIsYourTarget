using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPriolity
{
    public int GetPriolityNum(PriolityData priolityData = null);
}

public abstract class PriolityData { }
public class ModelPriolity : PriolityData
{

}
