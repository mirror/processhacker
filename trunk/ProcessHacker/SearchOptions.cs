/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public enum SearchType
    {
        Literal, Regex, String, Heap  
    }

    public class SearchOptions
    {                              
        private int _pid;
        private SearchType _type = SearchType.Literal;
        private Searcher _searcher;

        public SearchOptions(int PID, SearchType type)
        {
            _pid = PID;

            _searcher = new Searcher(_pid);

            // defaults
            _searcher.Params.Add("text", new byte[0]);
            _searcher.Params.Add("regex", "");
            _searcher.Params.Add("s_ms", "0x10");
            _searcher.Params.Add("h_ms", "0x400");
            _searcher.Params.Add("nooverlap", true);
            _searcher.Params.Add("ignorecase", false);
            _searcher.Params.Add("private", true);
            _searcher.Params.Add("image", false);
            _searcher.Params.Add("mapped", false);

            Type = type;  
        }  

        public int PID
        {
            get { return _pid; }
        }

        public SearchType Type
        {
            get { return _type; }
            set
            {
                _type = value;

                Dictionary<string, object> oldparams = _searcher.Params;
                List<string[]> oldresults = _searcher.Results;

                switch (_type)
                {
                    case SearchType.Literal:
                        _searcher = new LiteralSearcher(PID);
                        break;

                    case SearchType.Regex:
                        _searcher = new RegexSearcher(PID);
                        break;

                    case SearchType.String:
                        _searcher = new StringSearcher(PID);
                        break;

                    case SearchType.Heap:
                        _searcher = new HeapSearcher(PID);
                        break;

                    default:
                        _searcher = new Searcher(PID);
                        break;
                }

                foreach (string s in oldparams.Keys)
                {
                    _searcher.Params.Add(s, oldparams[s]);
                }

                _searcher.Results = oldresults;
            }
        }

        public Searcher Searcher
        {
            get { return _searcher; }
        }
    }
}
