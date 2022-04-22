using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService.Roboclaw
{
    delegate bool PerformActionFunction<T>(ref T value);
    delegate bool PerformDoubleActionFunction<T>(ref T value, ref T value2);

    internal abstract class Tag
    {
        public abstract bool DoReading();

        public abstract IEnumerable<Reading> Getreadings();

        public double TagScale { get; protected set; }
    }

    public record Reading(string TagName, object TagValue, double TagScale);

    internal class Tag<T> : Tag where T : struct, IEquatable<T>
    {
        public string TagName { get; private set; }

        T value = default(T);

        PerformActionFunction<T> PerformAction;

        public T TagValue { get; private set; }

        public Tag(string tagName, PerformActionFunction<T> func, double tagScale = 1) { 
            TagName = tagName; 
            PerformAction = func;
            TagScale = tagScale;
        }

        public override bool DoReading()
        {
            T temp=default(T);
            var result = PerformAction(ref temp);
            if (result) { 
                if (!temp.Equals(value))
                {
                    TagValue = temp;
                    value = temp;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public override IEnumerable<Reading> Getreadings()
        {
            return new Reading[] { new Reading(TagName, TagValue, TagScale) };
        }
    }

    internal class DoubleTag<T> : Tag where T : struct, IEquatable<T>
    {
        public string TagName1 { get; private set; }
        public string TagName2 { get; private set; }

        T value1 = default(T);
        T value2 = default(T);

        PerformDoubleActionFunction<T> PerformAction;

        public T TagValue1 { get; private set; }
        public T TagValue2 { get; private set; }

        public DoubleTag(string tagName1, string tagName2, PerformDoubleActionFunction<T> func, double tagScale = 1)
        {
            TagName1 = tagName1;
            TagName2 = tagName2;
            PerformAction = func;
            TagScale = tagScale;
        }

        public override bool DoReading()
        {
            T temp = default(T);
            T temp2 = default(T);
            var result = PerformAction(ref temp, ref temp2);
            if (result)
            {
                bool flag = false;
                if (!temp.Equals(value1))
                {
                    TagValue1 = temp;
                    value1 = temp;
                    flag = true ;
                }
                if (!temp2.Equals(value2))
                {
                    TagValue2 = temp2;
                    value2 = temp2;
                    flag = true;
                }
                return flag;
            }
            return false;
        }

        public override IEnumerable<Reading> Getreadings()
        {
            return new Reading[] {
                new Reading(TagName1, TagValue1, TagScale),
                new Reading(TagName2, TagValue2, TagScale)
            };
        }
    }
}
