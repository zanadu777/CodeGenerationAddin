using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

public class SvgCacheKey : IEquatable<SvgCacheKey>
{
    public string SvgName { get; }
    public Color[] Colors { get; }

    public SvgCacheKey(string svgName, Color[] colors)
    {
        SvgName = svgName;
        Colors = colors;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as SvgCacheKey);
    }

    public bool Equals(SvgCacheKey other)
    {
        if (other == null)
            return false;

        return SvgName == other.SvgName && Colors.SequenceEqual(other.Colors);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (SvgName?.GetHashCode() ?? 0);
            foreach (var color in Colors)
            {
                hash = hash * 23 + color.GetHashCode();
            }
            return hash;
        }
    }
}
