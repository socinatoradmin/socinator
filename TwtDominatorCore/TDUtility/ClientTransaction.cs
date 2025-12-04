using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class Cubic
{
    private readonly float a, b, c, d;

    public Cubic(List<float> values)
    {
        if (values.Count < 4)
            throw new ArgumentException("Cubic bezier requires 4 values");

        a = values[0];
        b = values[1];
        c = values[2];
        d = values[3];
    }

    public float GetValue(float t)
    {
        float mt = 1 - t;
        return mt * mt * mt * a + 3 * mt * mt * t * b + 3 * mt * t * t * c + t * t * t * d;
    }
}

public static class Utils
{
    public static List<float> Interpolate(List<float> from, List<float> to, float t)
    {
        return from.Zip(to, (f, toVal) => f + (toVal - f) * t).ToList();
    }

    public static List<float> ConvertRotationToMatrix(float degrees)
    {
        double radians = degrees * Math.PI / 180.0;
        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);
        return new List<float> { cos, -sin, sin, cos };
    }

    public static string FloatToHex(double x)
    {
        if (x == 0) return "0";
        var result = new List<string>();
        long intPart = (long)Math.Floor(x);
        double fracPart = x - intPart;

        while (intPart > 0)
        {
            long remainder = intPart % 16;
            result.Insert(0, remainder < 10 ? remainder.ToString() : ((char)(remainder - 10 + 'A')).ToString());
            intPart /= 16;
        }

        if (fracPart > 0)
        {
            result.Add(".");
            int limit = 6;
            while (fracPart > 0 && limit-- > 0)
            {
                fracPart *= 16;
                int digit = (int)Math.Floor(fracPart);
                result.Add(digit < 10 ? digit.ToString() : ((char)(digit - 10 + 'A')).ToString());
                fracPart -= digit;
            }
        }

        return string.Join("", result);
    }

    public static float IsOdd(int num) => num % 2 != 0 ? -1.0f : 0.0f;

    public static string Base64Encode(byte[] data) => Convert.ToBase64String(data);
}

public class ClientTransaction
{
    public static ClientTransaction Instance(string htmlContent)=> new ClientTransaction(htmlContent);
    private static readonly Regex ON_DEMAND_FILE_REGEX = new Regex("['\"]{1}ondemand\\.s['\"]{1}:\\s*['\"]{1}([\\w]*)['\"]{1}", RegexOptions.Multiline);
    private static readonly Regex INDICES_REGEX = new Regex("\\(\\w{1}\\[(\\d{1,2})\\],\\s*16\\)+", RegexOptions.Multiline);

    private readonly int additionalRandomNumber = 3;
    private readonly string defaultKeyword = "obfiowerehiring";
    private readonly int defaultRowIndex;
    private readonly List<int> defaultKeyBytesIndices;

    private readonly string key;
    private readonly List<byte> keyBytes;
    private readonly string animationKey;

    public ClientTransaction(string htmlContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        defaultRowIndex = GetIndices(htmlDoc, out defaultKeyBytesIndices);
        key = GetKey(htmlDoc);
        keyBytes = GetKeyBytes(key);
        animationKey = GetAnimationKey(keyBytes, htmlDoc);
    }

    private int GetIndices(HtmlDocument doc, out List<int> keyByteIndices)
    {
        keyByteIndices = new List<int>();
        var onDemandMatch = ON_DEMAND_FILE_REGEX.Match(doc.Text);
        if (onDemandMatch.Success)
        {
            string jsUrl = $"https://abs.twimg.com/responsive-web/client-web/ondemand.s.{onDemandMatch.Groups[1].Value}a.js";
            using (var client = new HttpClient())
            {
                var jsContent = client.GetStringAsync(jsUrl).Result;

                foreach (Match match in INDICES_REGEX.Matches(jsContent))
                {
                    if (int.TryParse(match.Groups[1].Value, out int index))
                        keyByteIndices.Add(index);
                }
                keyByteIndices = keyByteIndices.Skip(1).ToList();
            }
        }

        if (keyByteIndices.Count < 2)
            throw new Exception("Couldn't get KEY_BYTE indices");

        return keyByteIndices[0];
    }

    private string GetKey(HtmlDocument doc)
    {
        var meta = doc.DocumentNode.SelectSingleNode("//meta[@name='twitter-site-verification']");
        if (meta == null || meta.Attributes["content"] == null)
            throw new Exception("Couldn't get key from the page source");
        return meta.Attributes["content"].Value;
    }

    private List<byte> GetKeyBytes(string key)
    {
        return Convert.FromBase64String(key).ToList();
    }
    public HtmlNodeCollection GetFrames(HtmlDocument doc)
    {
        var allNodes = doc.DocumentNode.SelectNodes("//*");
        if (allNodes == null)
            return null;

        var matchedFrames = allNodes
            .Where(node => node.Id != null && node.Id.StartsWith("loading-x-anim"))
            .ToList();

        var dummyParent = HtmlNode.CreateNode("<div></div>");
        foreach (var frame in matchedFrames)
        {
            dummyParent.AppendChild(frame);
        }

        return dummyParent.ChildNodes;
    }
    public List<List<byte>> Get2DArray(List<byte> keyBytes, HtmlDocument doc, HtmlNodeCollection frames = null)
    {
        if (frames == null)
        {
            frames = GetFrames(doc);
        }

        int index = keyBytes[5] % 4;

        // Get the path "d" attribute
        var frame = frames[index];
        var pathNode = frame
            .ChildNodes
            .FirstOrDefault(n => n.Name == "g")?
            .ChildNodes
            .FirstOrDefault(n => n.Name == "g")?
            .ChildNodes
            .FirstOrDefault(n => n.Name == "path");

        if (pathNode == null || !pathNode.Attributes.Contains("d"))
            throw new Exception("SVG path not found or missing 'd' attribute");

        string d = pathNode.GetAttributeValue("d", "");
        string[] segments = d.Substring(9).Split('C');  // Similar to Python: .get("d")[9:].split("C")

        var result = new List<List<byte>>();
        var numberRegex = new Regex(@"[^\d]+");

        foreach (string segment in segments)
        {
            var numbers = numberRegex.Replace(segment, " ")
                                     .Trim()
                                     .Split(' ', (char)StringSplitOptions.RemoveEmptyEntries)
                                     .Select(x => byte.Parse(x))
                                     .ToList();
            result.Add(numbers);
        }

        return result;
    }

    private string GetAnimationKey(List<byte> keyBytes, HtmlDocument doc)
    {
        float totalTime = 4096f;
        int rowIndex = keyBytes[defaultRowIndex] % 16;
        int frameTime = defaultKeyBytesIndices.Select(index => keyBytes[index] % 16).Aggregate((a, b) => a * b);
        var arr = Get2DArray(keyBytes, doc);
        var mockFrame = arr[rowIndex];
        float targetTime = frameTime / totalTime;

        return Animate(mockFrame, targetTime);
    }

    private string Animate(List<byte> frames, float targetTime)
    {
        List<float> fromColor = frames.Take(3).Select(x => (float)x).ToList();
        List<float> toColor = frames.Skip(3).Take(3).Select(x => (float)x).ToList();
        fromColor.Add(1); toColor.Add(1);

        List<float> fromRotation = new List<float>() { 0f };
        List<float> toRotation = new List<float>() { Solve(frames[6], 60, 360, true) };

        List<float> easing = frames.Skip(7).Select((val1, i) => Solve(val1, Utils.IsOdd(i), 1f, false)).ToList();
        Cubic cubic = new Cubic(easing);
        float val = cubic.GetValue(targetTime);

        var color = Utils.Interpolate(fromColor, toColor, val).Select(c => Math.Max(0, c)).ToList();
        var rotation = Utils.Interpolate(fromRotation, toRotation, val);
        var matrix = Utils.ConvertRotationToMatrix(rotation[0]);

        var strArr = color.Take(3).Select(c => ((int)Math.Round(c)).ToString("x")).ToList();
        strArr.AddRange(matrix.Select(m =>
        {
            var rounded = Math.Abs(Math.Round(m, 2));
            var hex = Utils.FloatToHex(rounded);
            return (hex.StartsWith(".") ? "0" + hex : hex).ToLower();
        }));

        strArr.AddRange(new[] { "0", "0" });
        return Regex.Replace(string.Join("", strArr), "[.-]", "");
    }

    private float Solve(float value, float min, float max, bool round)
    {
        float result = value * (max - min) / 255f + min;
        return round ? (float)Math.Floor(result) : (float)Math.Round(result, 2);
    }

    public string GenerateTransactionId(string method, string path, long? timeNow = null)
    {
        timeNow = timeNow ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1682924400;

        var timeNowBytes = BitConverter.GetBytes((int)timeNow);
        if (BitConverter.IsLittleEndian) Array.Reverse(timeNowBytes);

        var sha = SHA256.Create();
        string prehash = $"{method}!{path}!{timeNow}{defaultKeyword}{animationKey}";
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(prehash));

        var output = new List<byte>();
        int randomByte = new Random().Next(0, 256);
        output.Add((byte)randomByte);

        var combined = keyBytes
            .Concat(timeNowBytes)
            .Concat(hash.Take(16))
            .Concat(new byte[] { (byte)additionalRandomNumber });
        output.AddRange(combined.Select(b => (byte)(b ^ randomByte)));

        return Utils.Base64Encode(output.ToArray()).TrimEnd('=');
    }
}
