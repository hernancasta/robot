using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService.Roboclaw
{
    internal abstract class TagGroup
    {
        abstract public string TagGroupName { get; set; }
        public abstract bool DoReading();

        public abstract IEnumerable<Reading> GetReadings();
    }

    internal class TagGroup<T> : TagGroup where T : struct, IEquatable<T>
    {
        public string TagName1 { get; private set; }
        public string TagName2 { get; private set; }
        public override string TagGroupName { get; set; }

        public double TagScale { get; protected set; }

        T value1 = default(T);
        T value2 = default(T);

        PerformDoubleActionFunction<T> PerformAction;

        public T TagValue1 { get; private set; }
        public T TagValue2 { get; private set; }

        public TagGroup(string tagName1, string tagName2, PerformDoubleActionFunction<T> func,
                        string tagGroupName,
                        double tagScale = 1)
        {
            TagName1 = tagName1;
            TagName2 = tagName2;
            PerformAction = func;
            TagScale = tagScale;
            TagGroupName = tagGroupName;
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
                    flag = true;
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

        public override IEnumerable<Reading> GetReadings()
        {
            return new Reading[] {
                new Reading(TagName1, TagValue1, TagScale),
                new Reading(TagName2, TagValue2, TagScale)
            };
        }
    }

}
