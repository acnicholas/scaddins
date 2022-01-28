// (C) Copyright 2019-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

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
            NumberOfErrors++;
        }

        public void AddSuccess(string message)
        {
            items.Add(new TranscactionLogItem() { Message = message });
            NumberOfItems++;
        }

        public string Summary()
        {
            string result = ToString();
            result += System.Environment.NewLine;
            foreach (var item in items)
            {
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
