using UnityEngine;

[System.Serializable]
public class Int2
{
    public int x, y;

    public Int2(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }
    public bool isDiagonal()
    {
        return (x != 0 && y != 0);
    }
    public Int2 TurnClockWise45()
    {
        if (x == 0) return new Int2(y, y);
        if (y == 0) return new Int2(x, -x);
        if (x == y) return new Int2(x, 0);
        if (x == -y) return new Int2(0, y);
        return Int2.zero;
    }

    public Int2 TurnClockWise90()
    {
        if (x == 0) return new Int2(y, 0);
        else return new Int2(0, -x);
    }

    //--------------------statics--------------------
    public static float SquareDistance(Int2 a, Int2 b)
    {
        return Mathf.Pow((float)a.x-(float)b.x, 2f) + Mathf.Pow((float)a.y-(float)b.y, 2f);
    }


    public static Int2 operator -(Int2 int2)
    {
        return new Int2(-int2.x, -int2.y);
    }

    public static Int2 operator -(Int2 intA, Int2 intB)
    {
        return new Int2(intA.x - intB.x, intA.y - intB.y);
    }

    public static Int2 operator +(Int2 intA, Int2 intB)
    {
        return new Int2(intA.x + intB.x, intA.y + intB.y);
    }

    public static bool operator ==(Int2 intA, Int2 intB)
    {
        return (intA.x == intB.x && intA.y == intB.y);
    }
    public static bool operator !=(Int2 intA, Int2 intB)
    {
        return (intA.x != intB.x || intA.y != intB.y);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Int2);
    }

    public bool Equals(Int2 p)
    {
        // If parameter is null, return false.
        if (Object.ReferenceEquals(p, null))
        {
            return false;
        }

        // Optimization for a common success case.
        if (Object.ReferenceEquals(this, p))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (this.GetType() != p.GetType())
        {
            return false;
        }

        // Return true if the fields match.
        // Note that the base class is not invoked because it is
        // System.Object, which defines Equals as reference equality.
        return (x == p.x) && (y == p.y);
    }

    public override int GetHashCode()
    {
        return x * 0x00010000 + y;
    }

    /*public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Int2)obj);
    }*/

    /*public override int GetHashCode()
    {
        unchecked
        {
            return (First * 397) ^ EqualityComparer<T1>.Default.GetHashCode(Second);
        }
    }*/

    public static Int2 zero     { get { return new Int2(0, 0); } }
    /*public static Int2 up       { get { return new Int2(0, 1); } }
    public static Int2 right    { get { return new Int2(1, 0); } }
    public static Int2 down     { get { return new Int2(0, -1); } }
    public static Int2 left     { get { return new Int2(-1, 0); } }*/

}