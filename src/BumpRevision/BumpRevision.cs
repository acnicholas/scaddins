//
//  Program.cs
//
//  Author:
//       Andrew Nicholas <andrewnicholas@iinet.net.au>
//
//  Copyright (c) 2015 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BumpRevision
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            if (!File.Exists(args[0])){
                return;
            }
            var text = File.ReadAllText(args[0]);
            const string pattern = "(AssemblyVersion[(]\"[0-9]{1,3}[.][0-9]{1,3}[.][0-9]{1,3}[.])([0-9]{1,3})";
            var groups = Regex.Match(text, pattern).Groups;
            var build = int.Parse(groups[2].Value);
            var rev = groups[1].Value.ToString();
            build++;
            var result = Regex.Replace(text, pattern, rev + build.ToString());
            File.WriteAllText(args[0], result);
            Console.WriteLine(result);
        }
    }
}
