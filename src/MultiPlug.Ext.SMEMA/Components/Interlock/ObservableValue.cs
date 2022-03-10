using System;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class ObservableValue<T>
    {
        private T m_Value;

        internal event Action<T> Updated;

        internal T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if( ! m_Value.Equals(value))
                {
                    m_Value = value;
                    Updated?.Invoke(m_Value);
                }
            }
        }
    }
}
