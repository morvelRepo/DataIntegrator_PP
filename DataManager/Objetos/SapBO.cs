using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegrator.Objetos
{
    public class TipoCambio
    {
        private double _conversionRate = 0;

        public double conversionRate { get { return _conversionRate; } set { _conversionRate = value; } }
    }
}
