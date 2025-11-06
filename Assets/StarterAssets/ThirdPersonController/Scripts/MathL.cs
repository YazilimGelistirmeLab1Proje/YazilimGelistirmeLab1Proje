using UnityEngine;

public class MathL : MonoBehaviour
{
    public static float PercentageCalculation(float currentNumber, float maxNumber)
    {
        float valueToReturn = 0;
        valueToReturn = ((currentNumber * 100) / maxNumber);
        return valueToReturn;
    }
}
