using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

public static class Map22D {
    
    public interface Geomerty { 
        
        public static class Collections {
            public class Points : Collection<Point> { 
                public Point[] Geometries { get; protected init; } = [];
            }
            public class Lines : Collection<Line>, IReadOnlyDictionary<Point, Line[]> {
                
                public Line[] Geometries { get; protected init; } = [];

                private IReadOnlyDictionary<Point, Line[]> __dictionary__ = null;
                protected IReadOnlyDictionary<Point, Line[]> dictionary => 
                    __dictionary__ ??= FrozenDictionary.ToFrozenDictionary(
                        (
                            from line in Geometries
                            group line by line[0] into lines
                            select lines
                        )
                        , e => e.Key
                        , e => e.ToArray()
                    );

                public IEnumerable<Point> Keys => dictionary.Keys;
                public IEnumerable<Line[]> Values => dictionary.Values;
                public int Count => dictionary.Count;

                public Line[] this[Point key] => dictionary[key];

                public bool ContainsKey(Point key) => dictionary.ContainsKey(key);

                public bool TryGetValue(Point key, [MaybeNullWhen(false)] out Line[] value) => 
                    dictionary.TryGetValue(key, out value);

                public IEnumerator<KeyValuePair<Point, Line[]>> GetEnumerator() =>
                    dictionary.GetEnumerator();

            }
        }

        public interface Collection<T> : IReadOnlySet<T>, IEquatable<Collection<T>> where T : Geomerty {

            //public abstract static IGeometryCollection<T>? From(IEnumerable<T> geometries);

            // Operations to compare to IEnumerable<T>
            public static bool operator <=(Collection<T> @this, IEnumerable<T> other) => false;
            public static bool operator <(Collection<T> @this, IEnumerable<T> other) => false;
            
            public static bool operator >=(Collection<T> @this, IEnumerable<T> other) => false;
            public static bool operator >(Collection<T> @this, IEnumerable<T> other) => false;

            //public static virtual bool operator ==(IGeometryCollection<T> @this, IGeometryCollection<T> other) => @this.Geometries == other.Geometries;
            //public static virtual bool operator !=(IGeometryCollection<T> @this, IGeometryCollection<T> other) => @this.Geometries == other.Geometries;

            // Union
            public static Collection<T> operator |(Collection<T> @this, IEnumerable<T> other) => null;
            // Intersection
            public static Collection<T> operator &(Collection<T> @this, IEnumerable<T> other) => null;
            // Symmetric difference
            public static Collection<T> operator ^(Collection<T> @this, IEnumerable<T> other) => null;
            // Difference (left)
            public static Collection<T> operator /(Collection<T> @this, IEnumerable<T> other) => null;
            // Difference (right)
            public static Collection<T> operator %(Collection<T> @this, IEnumerable<T> other) => null;
            
            public T[] Geometries { get; }

            int IReadOnlyCollection<T>.Count => Geometries.Length;

            bool IReadOnlySet<T>.Contains(T item) => Geometries.Contains(item);
            
            bool IReadOnlySet<T>.IsProperSubsetOf(IEnumerable<T> other) => this <= other;
            bool IReadOnlySet<T>.IsSubsetOf(IEnumerable<T> other) => this < other;

            bool IReadOnlySet<T>.IsProperSupersetOf(IEnumerable<T> other) => this >= other;
            bool IReadOnlySet<T>.IsSupersetOf(IEnumerable<T> other) => this > other;
            
            bool IReadOnlySet<T>.Overlaps(IEnumerable<T> other) => ( this & other ).Geometries is not [];

            bool IReadOnlySet<T>.SetEquals(IEnumerable<T> other) => this == other;

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => Geometries.AsEnumerable().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => Geometries.GetEnumerator();

            bool IEquatable<Collection<T>>.Equals(Collection<T>? other) => Geometries == other?.Geometries;

        }

        public int X { get; }
        public int Y { get; }

        public record struct Point( int X, int Y ) : Geomerty {
            public static implicit operator Point((int X, int Y) tuple) => new Point(tuple.X, tuple.Y);
            public static implicit operator (int X, int Y)(Point point) => (point.X, point.Y);
        };
        
        public class Line : Collections.Points, Geomerty, IReadOnlyList<Point> { 
            public int X { get => Geometries[0].X; }
            public int Y { get => Geometries[0].Y; }

            public Point this[int index] => Geometries[index];

            public Line(params Point[] points) => 
                Geometries = points.Length > 1 ? points : throw new Exception("Need at least two points to be a line");
        }
        
        public class Ring : Line { 
        
            public Ring(params Point[] points) => 
                Geometries = points.Length > 2 ? points : throw new Exception("Need at least three points to be a ring");

        }
        
        public class Area : Ring { 


        }
    }





    public interface IGeometry {
        public int X { get; }
        public int Y { get; }
    }
    
    public interface IGeometryCollectionFrom<T> where T : IGeometry {
        public static IGeometryCollection<T> From(params T[] geometries) => geometries switch {
            Ring[] rings => (IGeometryCollection<T>)new Area { Geometries = rings },
            Line[] lines => (IGeometryCollection<T>)new Lines { Geometries = lines },
            Point[] points => (IGeometryCollection<T>)new Points{ Geometries = points },
            _ => throw new InvalidCastException()
        };
    }

    public interface IGeometryCollection<T> : IReadOnlySet<T>, IEquatable<IGeometryCollection<T>> where T : IGeometry { 
        //public abstract static IGeometryCollection<T>? From(IEnumerable<T> geometries);

        // Operations to compare to IEnumerable<T>
        public static bool operator <=(IGeometryCollection<T> @this, IEnumerable<T> other) => false;
        public static bool operator <(IGeometryCollection<T> @this, IEnumerable<T> other) => false;
        
        public static bool operator >=(IGeometryCollection<T> @this, IEnumerable<T> other) => false;
        public static bool operator >(IGeometryCollection<T> @this, IEnumerable<T> other) => false;

        //public static virtual bool operator ==(IGeometryCollection<T> @this, IGeometryCollection<T> other) => @this.Geometries == other.Geometries;
        //public static virtual bool operator !=(IGeometryCollection<T> @this, IGeometryCollection<T> other) => @this.Geometries == other.Geometries;

        // Union
        public static IGeometryCollection<T> operator |(IGeometryCollection<T> @this, IEnumerable<T> other) => null;
        // Intersection
        public static IGeometryCollection<T> operator &(IGeometryCollection<T> @this, IEnumerable<T> other) => null;
        // Symmetric difference
        public static IGeometryCollection<T> operator ^(IGeometryCollection<T> @this, IEnumerable<T> other) => null;
        // Difference (left)
        public static IGeometryCollection<T> operator /(IGeometryCollection<T> @this, IEnumerable<T> other) => null;
        // Difference (right)
        public static IGeometryCollection<T> operator %(IGeometryCollection<T> @this, IEnumerable<T> other) => null;
        
        public T[] Geometries { get; }

        int IReadOnlyCollection<T>.Count => Geometries.Length;

        bool IReadOnlySet<T>.Contains(T item) => Geometries.Contains(item);
        
        bool IReadOnlySet<T>.IsProperSubsetOf(IEnumerable<T> other) => this <= other;
        bool IReadOnlySet<T>.IsSubsetOf(IEnumerable<T> other) => this < other;

        bool IReadOnlySet<T>.IsProperSupersetOf(IEnumerable<T> other) => this >= other;
        bool IReadOnlySet<T>.IsSupersetOf(IEnumerable<T> other) => this > other;
        
        bool IReadOnlySet<T>.Overlaps(IEnumerable<T> other) => ( this & other ).Geometries is not [];

        bool IReadOnlySet<T>.SetEquals(IEnumerable<T> other) => false;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Geometries.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Geometries.GetEnumerator();

        bool IEquatable<IGeometryCollection<T>>.Equals(IGeometryCollection<T>? other) => Geometries == other?.Geometries;
    }

    public record struct Point( int X, int Y ) : IGeometry, IGeometryCollectionFrom<Point>;

    public class Points : IGeometryCollection<Point> {
        public Point[] Geometries { get; init; } = [];
    }

    public class Line : Points, IGeometry, IGeometryCollectionFrom<Line> /* list */ { 
        public int X { get; }
        public int Y { get; }
    }

    public class Lines : IGeometryCollection<Line> {
        public Line[] Geometries { get; init; } = [];
    }

    public class Ring : Line { }
    public class Area : Ring, IGeometryCollection<Ring> { 
        public new Ring[] Geometries { get; init; } = [];

        IEnumerator<Ring> IEnumerable<Ring>.GetEnumerator() => Geometries.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Geometries.GetEnumerator();
    }

    [Flags]
    public enum Compass : byte {
        North = 0b0000_0001, N = North,
        South = 0b0000_0010, S = South,
        
        East  = 0b0000_0100, E = East,
        West  = 0b0000_1000, W = West,

        NorthEast = 0b0001_0000, NE = NorthEast,
        SouthEast = 0b0010_0000, SE = SouthEast,

        SouthWest = 0b0100_0000, SW = SouthWest,
        NorthWest = 0b1000_0000, NW = NorthWest,

        AllCardinalDirections   = North | East | South | West,
        AllOrdinalDirections    = NorthEast | SouthEast | SouthWest | NorthWest,

        AllDirections = AllCardinalDirections | AllOrdinalDirections, All = AllDirections,

        IsCardinal = 0b0000_1111,
        IsOrdinal  = 0b1111_0000,
    }

}


public static class Map2D {

    public interface IOrigin {
        public int X { get; }
        public int Y { get; }
        public Coordinate Origin => (X, Y);
    }

    public interface ICoordinateSpace {
        public Coordinate Max { get; }
        public Coordinate Min { get; }
    }

    public record Points : IReadOnlyList<Coordinate>, IReadOnlySet<Coordinate> { }
    public record Line : Cloud { }
    public record Lines : Cloud, IReadOnlySet<Line> { }
    public record Area : Cloud { }


    public record class Map<T>(T[] Line) : ICoordinateSpace, IReadOnlyList<T>, IReadOnlyDictionary<Coordinate, T>, IReadOnlyDictionary<int, T> {
        public Coordinate Max { get; } = (
            (int)Math.Sqrt(Int32.MaxValue), 
            (int)Math.Sqrt(Int32.MaxValue)
        );
        public Coordinate Min { get; } = (0, 0);

        public Map(T[] Line, Coordinate Max) : this(Line) => this.Max = Max;

        //
        // IReadOnlyList<T>
        //
        public int Count => Line.Length;
        public T this[int index] => Line[index];

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Line.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()=> Line.GetEnumerator();

        //
        // IReadOnlyDictionary<int, T>
        //
        IEnumerable<int> IReadOnlyDictionary<int, T>.Keys => Enumerable.Range(0, Count);
        IEnumerable<T> IReadOnlyDictionary<int, T>.Values => Line;

        int IReadOnlyCollection<KeyValuePair<int, T>>.Count => Line.Length;

        public bool ContainsKey(int key) => 0 <= key && key <= Line.Length;
        public bool TryGetValue(int key, out T value){
            if( ContainsKey(key) ){
                value = Line[key];
                return true;
            } else {
                value = default;
                return false;
            }
        }
        
        IEnumerator<KeyValuePair<int, T>> IEnumerable<KeyValuePair<int, T>>.GetEnumerator() => 
            Line
                .Select((field, index) => KeyValuePair.Create(index, field))
                .GetEnumerator();

        //
        // IReadOnlyDictionary<Coordinate, T>
        //
        IEnumerable<Coordinate> IReadOnlyDictionary<Coordinate, T>.Keys => ((IReadOnlyDictionary<int, T>)this).Keys.Select( index => IndexToCoordinate(this, index));

        IEnumerable<T> IReadOnlyDictionary<Coordinate, T>.Values => ((IReadOnlyDictionary<int, T>)this).Values;
        
        public T this[Coordinate key] => this[CoordinateToIndex(this, key)];

        public bool ContainsKey(Coordinate key) => Contains(this, key);
        public bool TryGetValue(Coordinate key, out T value){
            if( ContainsKey(key) ){
                value = this[key];
                return true;
            } else {
                value = default;
                return false;
            }
        }

        IEnumerator<KeyValuePair<Coordinate, T>> IEnumerable<KeyValuePair<Coordinate, T>>.GetEnumerator() => 
            Line
                .Select((field, index) => KeyValuePair.Create(IndexToCoordinate(this, index), field))
                .GetEnumerator();

        
    }

    public record class Map : Map<char> {
        public static Map From(IEnumerable<string> lines){

            var array = lines.ToArray();
            
            var (max_x, max_y) = 
                (
                    array.Select( line => line.Length ).Distinct().Single(),
                    array.Length
                );

            return new ( String.Concat(array).ToCharArray(), (max_x, max_y) );

        }

        protected Map(char[] Line, Coordinate Max) : base(Line, Max) { ;; }
    }

    public record class Coordinate(int X, int Y) : IOrigin {
        public static Coordinate operator +(Coordinate coordinate, Direction direction) => direction switch {
            Direction.North     => coordinate + ( 0, -1),
            Direction.NorthEast => coordinate + (+1, -1),
            Direction.East      => coordinate + (+1,  0),
            Direction.SouthEast => coordinate + (+1, +1),
            Direction.South     => coordinate + ( 0, +1),
            Direction.SouthWest => coordinate + (-1, +1),
            Direction.West      => coordinate + (-1,  0),
            Direction.NorthWest => coordinate + (-1, -1),
            _ => throw new Exception("Can only move in a single direction")
        };
        public static Coordinate operator -(Coordinate coordinate, Direction direction) => direction switch {
            Direction.North     => coordinate - ( 0, -1),
            Direction.NorthEast => coordinate - (+1, -1),
            Direction.East      => coordinate - (+1,  0),
            Direction.SouthEast => coordinate - (+1, +1),
            Direction.South     => coordinate - ( 0, +1),
            Direction.SouthWest => coordinate - (-1, +1),
            Direction.West      => coordinate - (-1,  0),
            Direction.NorthWest => coordinate - (-1, -1),
            _ => throw new Exception("Can only move in a single direction")
        };
        
        public static Coordinate operator +(Coordinate origin, Coordinate offset) => (origin.X + offset.X, origin.Y + offset.Y);
        public static Coordinate operator -(Coordinate origin, Coordinate offset) => (origin.X - offset.X, origin.Y - offset.Y);

        public static bool operator <=(Coordinate left, Coordinate? right) => left == right || left < right!;
        public static bool operator >=(Coordinate left, Coordinate? right) => left == right || left > right!;
        public static bool operator <(Coordinate left, Coordinate right) => left.Y < right.Y || (left.Y == right.Y && left.X < right.X);
        public static bool operator >(Coordinate left, Coordinate right) => left.Y > right.Y || (left.Y == right.Y && left.X > right.X);

        public static Cloud operator *(Coordinate left, Direction directions){ directions.HasFlag()

        }

        public static implicit operator Coordinate((int, int) coordinate) => coordinate;
    }

    [Flags]
    public enum Direction : byte {
        North = 0b0000_0001, N = North,
        South = 0b0000_0010, S = South,
        
        East  = 0b0000_0100, E = East,
        West  = 0b0000_1000, W = West,

        NorthEast = 0b0001_0000, NE = NorthEast,
        SouthEast = 0b0010_0000, SE = SouthEast,

        SouthWest = 0b0100_0000, SW = SouthWest,
        NorthWest = 0b1000_0000, NW = NorthWest,

        AllCardinalDirections   = North | East | South | West,
        AllOrdinalDirections    = NorthEast | SouthEast | SouthWest | NorthWest,

        AllDirections = AllCardinalDirections | AllOrdinalDirections, All = AllDirections,

        Cardinal = 0b0000_1111, IsCardinal = Cardinal,
        Ordinal  = 0b1111_0000, IsOrdinal = Ordinal
    }

    public static int CoordinateToIndex(this ICoordinateSpace @this, int x, int y) => x + (@this.Max.X * y);
    public static int CoordinateToIndex(this ICoordinateSpace @this, Coordinate c ) => @this.CoordinateToIndex(c.X, c.Y);
    public static Coordinate IndexToCoordinate(this ICoordinateSpace @this, int index) => (index % @this.Max.X, index / @this.Max.Y);

    public static bool Contains(this ICoordinateSpace @this, Coordinate coordinate) => true
        && @this.Min.X <= coordinate.X && coordinate.X < @this.Max.X
        && @this.Min.Y <= coordinate.Y && coordinate.Y < @this.Max.Y
        ;

    public static IEnumerable<Coordinate> Coordinates(this ICoordinateSpace @this) =>
        from y in Enumerable.Range(@this.Min.Y, @this.Max.Y - 1)
        from x in Enumerable.Range(@this.Min.X, @this.Max.X - 1)
        select (Coordinate)(x, y)
        ;

    // IEnumerable<(int x, int y)> Box((int x, int y) c) =>
    //     from coordinate in new (int, int)[]{
    //             (   c.x - 1, c.y - 1    ), (    c.x, c.y - 1    ), (    c.x + 1, c.y - 1    ),
    //             (   c.x - 1, c.y        ),                         (    c.x + 1, c.y        ),
    //             (   c.x - 1, c.y + 1    ), (    c.x, c.y + 1    ), (    c.x + 1, c.y + 1    )
    //         }
    //     where ValidCoordinates(coordinate)
    //     select coordinate
    // ;
}

public static class Map3D { }