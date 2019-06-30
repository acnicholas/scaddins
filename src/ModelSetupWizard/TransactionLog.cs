////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

namespace SCaddins.ModelSetupWizard
{
    public class TransactionLog
    {
        //// private List<TranscactionLogItem> items;

        public TransactionLog(string name)
        {
            //// items = new List<TranscactionLogItem>();
            Name = name;
            NumberOfErrors = 0;
            NumberOfItems = 0;
        }

        public void AddSuccess(string Message)
        {
            NumberOfItems++;
        }

        public void AddFailure(string Message)
        {
            NumberOfItems++;
            NumberOfErrors++;
        }

        public int Successes {
            get
            {
                return NumberOfItems - NumberOfErrors;
            }
        }

        /// <summary>
        /// The name of this log.
        /// </summary>
        public string Name {
            get; set;
        }

        public int NumberOfItems
        {
            get; private set;
        }

        public int NumberOfErrors
        {
            get; private set;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}: {1} Successes, {2} Failures", Name, NumberOfItems, NumberOfErrors);
        }
    }
}
