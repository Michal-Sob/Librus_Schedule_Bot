using System;
using System.Collections.Generic;

namespace LibrusScheduleHelper.Helpers;

public class RomanToNumbers
{
    public int RomanToInt(string s)
    {
        var sum = 0;
        var romanNumbersDict = new Dictionary<char, int>()
        {
            { 'I', 1 },
            { 'V', 5 },
            { 'X', 10 },
            { 'L', 50 },
            { 'C', 100 },
            { 'D', 500 },
            { 'M', 1000 }
        };
        for (var i = 0; i < s.Length; i++)
        {
            var currentRomanChar = s[i];

            romanNumbersDict.TryGetValue(currentRomanChar, out var num);

            if (num == 0)
                continue;
            
            if (i + 1 < s.Length && romanNumbersDict.ContainsKey(s[i + 1]) && romanNumbersDict[s[i + 1]] > romanNumbersDict[currentRomanChar])
            {
                sum -= num;
            }
            else
            {
                sum += num;
            }
        }
        return sum;
    }
}