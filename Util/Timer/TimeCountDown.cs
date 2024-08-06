using UnityEngine;

namespace Util
{
    public class TimeCountDown
    {
        protected float fValue;
        protected float fCurValue;

        //值发生变化的委托
        public System.Action<float> pActionValue;


        public TimeCountDown(float value)
        {
            fValue = value;
            FillTime();
        }


        public void Tick(float delta)
        {
            fCurValue += delta;
        }

        public bool TimeOut
        {
            get => fCurValue >= fValue;
        }

        public void FillTime(bool addLast = false)
        {
            if (addLast)
            {
                float fRes = fCurValue;
                fCurValue = 0;
                fCurValue -= fRes;
            }
            else
            {
                fCurValue = 0;
            }
        }
    }
}