using Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command.Preset
{

    public class PresetConfiguration { 
        public PresetDefinition[] Presets { get; set; }

        public string Topic { get; set; }
    }

    public class PresetDefinition
    {
        public string Name { get; set; }
        public string InitialValue { get; set; }
        public string MaxValue { get; set; }
        public string MinValue { get; set; }
        public string DataType { get; set; }
    }

    public abstract class Preset {
        public abstract void Initialize();
        public string Name { get; set; }

        public string Category { get; set; }

        public abstract object CurrentValueObject { get; }
        public abstract object SetPointValueObject { get; }

        public abstract string DataType { get; }

        public abstract PresetMessage GetPresetMessage();
    }

    public class Preset<T> : Preset where T : struct, IComparable<T>
    {
        public T InitialValue { get; set; }
        public T MaxValue { get; set; }
        public T MinValue { get; set; }

        public T CurrentValue { get; private set; }

        public T SetPointValue { get; private set; }

        public override object CurrentValueObject => CurrentValue;

        public override object SetPointValueObject => SetPointValue;

        public override string DataType => typeof(T).ToString();

        public void SetValue(T DesiredValue)
        {
            SetPointValue = DesiredValue;
            if (((IComparable)DesiredValue).CompareTo(MinValue) < 0) { 
                CurrentValue = MinValue;
                return;
            }
            if (((IComparable)DesiredValue).CompareTo(MaxValue) > 0)
            {
                CurrentValue = MaxValue;
                return;
            }

            CurrentValue = DesiredValue;
        }

        public override void Initialize()
        {
            CurrentValue = InitialValue;
            SetPointValue = InitialValue;
        }

        public override PresetMessage GetPresetMessage()
        {
            return
            new PresetMessage()
            {
                Category = this.Category,
                Name = this.Name,
                CurrentValue = this.CurrentValueObject,
                SetValue = this.SetPointValueObject,
                DataType = this.DataType
            };

        }
    }
}
