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

        public abstract Reading GetReading();

    }

    public record Reading(string TagName, object TagValue, double TagScale);

    internal class Tag<T> : Tag where T : struct, IEquatable<T>
    {
        public string TagName { get; private set; }

        T value = default(T);

        PerformActionFunction<T> PerformAction;

        public T TagValue { get; private set; }
        public double TagScale { get; protected set; }

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

        public override Reading GetReading()
        {
            return new Reading(TagName, TagValue, TagScale) ;
        }
    }

}
