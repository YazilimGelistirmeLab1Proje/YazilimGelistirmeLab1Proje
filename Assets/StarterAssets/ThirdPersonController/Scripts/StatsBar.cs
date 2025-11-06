using UnityEngine;
using UnityEngine.UI;
public class StatsBar : MonoBehaviour
{
    [SerializeField] private Image bar;

    public void UpdateBar(float current, float max)
    {
        bar.fillAmount = MathL.PercentageCalculation(current, max)/100f;
    }
}
