using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;

namespace LinkedDominatorCore.LDUtility
{
    public class SpintaxParser
    {
        public List<string> GetSpindledList(List<string> inputList)
        {
            var spindledList = new List<string>();
            try
            {
                foreach (var item in inputList)
                {
                    var tempSpunList = SpintaxGenerator(item);
                    spindledList.AddRange(tempSpunList);
                }

                return spindledList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                spindledList = inputList;
                return spindledList;
            }
        }

        public List<string> SpintaxGenerator(string message)
        {
            var stopWatch = new Stopwatch();
            List<string> spintaxList;
            spintaxList = new List<string>();
            if (string.IsNullOrEmpty(message))
                return spintaxList;

            List<string> spintext;
            spintext = new List<string>();
            var rnd = new Random();
            string genspintext;
            string pattern;

            try
            {
                stopWatch.Start();
                long count = 0;
                while (stopWatch.Elapsed.Seconds <= 5)
                {
                    genspintext = message;

                    pattern = message.Contains("{") ? "{[^{}]*}" : @"\(([^)]*)\)";

                    var m = Regex.Match(genspintext, pattern);

                    while (m.Success)
                    {
                        // Get random choice and replace pattern match.
                        var seg = genspintext.Substring(m.Index + 1, m.Length - 2);
                        var choices = seg.Split('|');
                        genspintext = genspintext.Substring(0, m.Index) + choices[rnd.Next(choices.Length)] +
                                      genspintext.Substring(m.Index + m.Length);
                        m = Regex.Match(genspintext, pattern);
                    }

                    if (!spintext.Contains(genspintext))
                        spintext.Add(genspintext);
                    ++count;
                }

                spintext = spintext.OrderBy(x => Guid.NewGuid()).ToList();

                stopWatch.Reset();
                // if no pipes(|) found then add default(given message) to list
                if (spintext.Count == 0)
                    spintext.Add(message);
                return spintext;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return spintaxList;
        }

        public List<string> GetSpinnedList(List<string> inputList)
        {
            try
            {
                var tempList = new List<string>();
                foreach (var item in inputList)
                    tempList.Add(item);
                inputList.Clear();
                foreach (var item in tempList)
                {
                    var tempSpunList = GetSpinnedComments(item);
                    inputList.AddRange(tempSpunList);
                }

                inputList = inputList.OrderBy(x => Guid.NewGuid()).ToList();
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                inputList = null;
            }

            return inputList;
        }

        public List<string> GetSpinnedComments(string rawComment)
        {
            List<string> listRequiredComments;
            try
            {
                #region Using Dictionary

                var commentsHashTable = new Dictionary<Match, string[]>();

                //This is final possible combinations of comments
                var listModComments = new List<string>();

                //Put braces data in list of string array
                var listDataInsideBracesArray = new List<string[]>();

                //This Regex will fetch data within braces and put it in list of string array
                //var regex = new Regex(@"\(([^)]*)\)", RegexOptions.Compiled);

                var regex = new Regex(@"{[^{}]*}", RegexOptions.Compiled);

                foreach (Match data in regex.Matches(rawComment))
                    try
                    {
                        var innerData = data.Value.Replace("(", "").Replace(")", "");
                        var dataInsideBracesArray = innerData.Split('|');
                        commentsHashTable.Add(data, dataInsideBracesArray);
                        listDataInsideBracesArray.Add(dataInsideBracesArray);
                    }
                    catch
                    {
                        //
                    }

                var modifiedComment = rawComment;

                IDictionaryEnumerator en = commentsHashTable.GetEnumerator();

                var listModifiedComment = new List<string>();

                listModifiedComment.Add(modifiedComment);

                #region Assigning Values and adding in List

                foreach (var item in listDataInsideBracesArray)
                {
                    en.MoveNext();
                    foreach (var modItem in listModifiedComment)
                    foreach (var innerItem in item)
                        try
                        {
                            var modComment = modItem.Replace(en.Key.ToString(), innerItem);
                            listModComments.Add(modComment);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    listModifiedComment.AddRange(listModComments);
                }

                #endregion

                listRequiredComments = listModifiedComment.FindAll(s => !s.Contains("("));
                return listRequiredComments;

                #endregion
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                listRequiredComments = null;
            }

            return listRequiredComments;
        }
    }
}