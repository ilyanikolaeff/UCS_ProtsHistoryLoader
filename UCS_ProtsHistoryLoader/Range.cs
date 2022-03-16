using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_ProtsHistoryLoader
{
    class Range 
    {
        private readonly double _minValue = double.NaN;
        private readonly double _maxValue = double.NaN;

        public Range(double minValue, double maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public bool IsContains(double value)
        {
            if (double.IsNaN(_minValue) && double.IsNaN(_maxValue))
                return false;
            if (double.IsNaN(_minValue))
                return value == _maxValue;
            else if (double.IsNaN(_maxValue))
                return value == _minValue;
            else
                return value >= _minValue && value <= _maxValue;
        }
    }
}
