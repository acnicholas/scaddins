////using System;

////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

namespace SCaddins.ModelSetupWizard
{
    using System.Collections.Generic;

    public class TransactionLog
    {
        private List<TranscactionLogItem> items;

        public TransactionLog(string name)
        {
            items = new List<TranscactionLogItem>();
            Name = name;
            NumberOfErrors = 0;
            NumberOfItems = 0;
        }

        /// <summary>
        /// The name of this log.
        /// </summary>
        public string Name
        {
            get; set;
        }

        public int NumberOfErrors
        {
            get; private set;
        }

        public int NumberOfItems
        {
            get; private set;
        }

        public int Successes
        {
            get
            {
                return NumberOfItems - NumberOfErrors;
            }
        }

        public void AddFailure(string message)
        {
            items.Add(new TranscactionLogItem() { Message = @"ERROR: " + message });
            NumberOfItems++;
            NumberOfErrors++;
        }

        public void AddSuccess(string message)
        {
            items.Add(new TranscactionLogItem() { Message = message });
            NumberOfItems++;
        }

        public string Summary()
        {
            string result = this.ToString();
            result += System.Environment.NewLine;
            foreach (var item in items) {
                result += item.Message;
                result += System.Environment.NewLine;
            }
            return result;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}: {1} Successes, {2} Failures", Name, NumberOfItems, NumberOfErrors);
        }
    }
}
