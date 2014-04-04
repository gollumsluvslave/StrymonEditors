using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace RITS.StrymonEditor.Models
{
    [Serializable]
    public class PotValueItem
    {
        [XmlAttribute]
        public int Value { get; set; }
        
        [XmlAttribute]
        public double Angle { get; set; }

        [XmlAttribute]
        public string ClockPosition { get; set; }

        [XmlIgnore]
        public int FineValue { get; set; }

    }

    [Serializable]
    public class PotValueMap : List<PotValueItem>
    {
        private List<PotValueItem> expanded;
        
        public int GetValueForAngle(double angle)
        {
            if (expanded == null) ExpandList();
            var map = expanded.FirstOrDefault(x => x.Angle > angle);
            if (map.Value == 0) return map.Value;
            var prev = expanded.FirstOrDefault(x => x.Value == map.Value - 1);
            return prev.Value;
            
        }
        public double GetAngleForValue(int value)
        {
            if (expanded == null) ExpandList();
            var map = expanded.FirstOrDefault(x => x.Value == value);
            if (map != null)
            {
                return map.Angle;
            }
            return 0;
        }
        public void Reset()
        {
            expanded = null;            

        }
        private void ExpandList()
        {
            expanded = new List<PotValueItem>();
            PotValueItem prev = null;
            foreach (var m in this.OrderBy(x => x.Value))
            {
                if (prev != null)
                {
                    Expand(prev, m);
                }
                prev = m;
            }
            expanded.Add(prev);
            
        }

        public List<PotValueItem> LookupMap { get { return expanded; } }
        public void ApplyFineValueIncrementMap(List<Increment> incrementMap, ParameterDef definition)
        {
            if (expanded == null) ExpandList();
            int i=0;
            int fineValueAccum=definition.FineRange.MinValue;
            var incItem = incrementMap[0];            
            foreach (var pvi in expanded)
            {
                if (pvi.Value == 0) { pvi.FineValue = definition.FineRange.MinValue; }
                else
                {

                    if (incItem.End == 0) incItem.End = definition.FineRange.MaxValue;
                    int actualIncValue = incItem.GetIncrementValue(pvi.Value);
                    if ((fineValueAccum + actualIncValue) <= incItem.End)
                    {
                        fineValueAccum += actualIncValue;
                        pvi.FineValue = fineValueAccum;
                    }
                    if (fineValueAccum == incItem.End)
                    {
                        // Need to 
                        incItem = incrementMap.FirstOrDefault(x => x.End > fineValueAccum);
                        if (incItem == null)
                        {
                            incItem = incrementMap.Last();
                        }
                    }
                    
                }
                if (fineValueAccum >=definition.FineRange.MaxValue)
                {
                    break;
                }
            }
        }
        private void Expand(PotValueItem from, PotValueItem to)
        {
            expanded.Add(new PotValueItem { Value = from.Value, FineValue = from.FineValue, Angle = from.Angle, ClockPosition = from.ClockPosition });

            int noOfValues = (to.Value - (from.Value+1));
            double angleDelta = to.Angle - from.Angle;
            double angleIncrement = angleDelta / (noOfValues+1);
            for (int i = 1; i <= noOfValues; i++)
            {
                expanded.Add(new PotValueItem { Value = from.Value + i, Angle = from.Angle+(angleIncrement * i) });
            }
        }
    }
}
