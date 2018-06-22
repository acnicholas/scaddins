// (C) Copyright 2018 by Andrew Nicholas andrewnicholas@iinet.net.au
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
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;

    public class CheckableItem : Caliburn.Micro.PropertyChangedBase
    {
        private ObservableCollection<CheckableItem> children;
        private bool? isChecked;
        private CheckableItem parent;

        public CheckableItem(DeletableItem deletable, CheckableItem parent)
        {
            Deletable = deletable;
            Children = new ObservableCollection<CheckableItem>();
            IsChecked = false;
            this.parent = parent;
        }

        public DeletableItem Deletable
        {
            get; set;
        }

        public bool? IsChecked
        {
            get {
                    return isChecked;
            }
            set
            {
                //cascade through children
                if (value != isChecked) {
                    isChecked = value;
                    if (isChecked != null) {
                        foreach (var i in Children) {
                            i.IsChecked = value;
                        }
                    }
                    NotifyOfPropertyChange(() => IsChecked);
                    //cascade though siblings
                    if (parent != null) {
                        int n = 0;
                        foreach (var i in parent.Children) {
                            if (i.IsChecked.HasValue) {
                                n += i.IsChecked.Value == true ? 1 : -1;
                            }
                        }
                        bool? nb = null;
                        parent.IsChecked = (n * -1) == parent.Children.Count ? n > 0 : nb;
                    }
                }
            }
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

        public void AddChildren(List<DeletableItem> deletables)
        {
            if (deletables == null || deletables.Count < 1) return;
            foreach (var deletable in deletables)
            {
                Children.Add(new CheckableItem(deletable, this));   
            }
        }

        public void AddChild(CheckableItem deletable)
        {
            if (deletable == null) return;
            Children.Add(deletable);
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
