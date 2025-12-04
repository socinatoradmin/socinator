using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary
{
    public class SpintaxParser
    {
        public static List<string> SpintaxGenerator(string message)
        {
            var stopWatch = new Stopwatch();
            var spintaxList = new List<string>();
            if (string.IsNullOrEmpty(message))
                return spintaxList;

            var spintext = new List<string>();
            var rnd = new Random();
            var spintaxMessage = message;

            // loop Count
            try
            {
                stopWatch.Start();
                // running while 10sec to get all possible combinations
                while (stopWatch.Elapsed.Seconds <= 5)
                {
                    var genspintext = message;

                    string pattern;
                    if (message.Contains("{"))
                        pattern = "{[^{}]*}";
                    else
                        pattern = @"\(([^)]*)\)";

                    var m = Regex.Match(genspintext, pattern);
                    if (genspintext.Contains('{'))
                        genspintext = genspintext.Substring(genspintext.IndexOf('{') + 1, genspintext.IndexOf('}') -
                            genspintext.IndexOf('{') - 1);
                    else
                        genspintext = genspintext.Substring(genspintext.IndexOf('(') + 1, genspintext.IndexOf(')') -
                            genspintext.IndexOf('(') - 1);
                    //    var seg = genspintext.Substring(m.Index + 1, m.Length - 2);
                    var choices = genspintext.Split('|');
                    genspintext = choices[rnd.Next(choices.Length)];
                    //genspintext = genspintext.Substring(0, m.Index) + choices[rnd.Next(choices.Length)] +
                    //              genspintext.Substring(m.Index + m.Length);
                    m = Regex.Match(genspintext, pattern);

                    //while (m.Success)
                    //{
                    //    // Get random choice and replace pattern match.
                    //    var seg = genspintext.Substring(m.Index + 1, m.Length - 2);
                    //    var choices = seg.Split('|');
                    //    genspintext = genspintext.Substring(0, m.Index) + choices[rnd.Next(choices.Length)] +
                    //                  genspintext.Substring(m.Index + m.Length);
                    //    m = Regex.Match(genspintext, pattern);
                    //}

                    if (!spintext.Contains(genspintext))
                        spintext.Add(genspintext);
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
        public static string GenerateMultilineMessageWithSpintext(string Message)
        {
            var AllMessages = new List<string>();
            try
            {
                var Messages = Regex.Split(Message, "\r\n");
                if(Messages != null)
                {
                    Messages.ForEach(message =>
                    {
                        if (message.Contains("|"))
                        {
                            var m = string.Empty;
                            var splitted = message?.Replace("(", "")?.Replace(")", "")?.Split('|');
                            try
                            {
                                m = splitted?.GetRandomItem();
                                var tag = Regex.Match(message, "(\\@[a-zA-Z\\d_-]+)(?!;)")?.Value;
                                if(!string.IsNullOrEmpty(tag))
                                    m = $"{tag} {m}";
                            }
                            catch
                            {
                                m = splitted.FirstOrDefault();
                            }
                            AllMessages.Add(m);
                        }
                        else
                            AllMessages.Add(message);
                    });
                }
            }
            catch { }
            return AllMessages.Count > 0 ? string.Join("\\n",AllMessages): Message;
        }
    }
}