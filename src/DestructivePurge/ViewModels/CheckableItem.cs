// (C) Copyright 2013-2014 by Andrew Nicholas andrewnicholas@iinet.net.au
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

namespace SCaddins.DestructivePurge.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Autodesk.Revit.DB;

    public class CheckableItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<CheckableItem> children;

        public CheckableItem(DeletableItem deletable)
        {
            this.Deletable = deletable;
            Children = new ObservableCollection<CheckableItem>();
            IsChecked = Visibility.Invisible;
        }

        public DeletableItem Deletable
        {
            get; set;
        }

        public Visibility IsChecked
        {
            get; set;
        }

        public string Name
        {
            get { return Deletable.Name; }
        }

        public ObservableCollection<CheckableItem> Children
        {
            get { return children; }
            set { children = value; }
        }

        public void AddChildren(Collection<DeletableItem> deletables)
        {
            if (deletables == null || deletables.Count < 1) return;
            foreach (var deletable in deletables)
            {
                Children.Add(new CheckableItem(deletable));   
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
