using StudentsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seclab.Strategy
{
    public class SelectedParser
    {
        private IStrategy _parsingStrategy;

        public void SetParsingStrategy(IStrategy parsingStrategy)
        {
            _parsingStrategy = parsingStrategy;
        }

        public StudentsCollection Parse(string filePath)
        {
            var result = _parsingStrategy.Parse(filePath);
            return result;
        }
    }
}
