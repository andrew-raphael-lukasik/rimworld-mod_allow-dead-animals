using System;

namespace AllowDeadAnimals
{
	
	internal class RingBuffer <T>
	{
		protected readonly T[] _array;
		protected readonly int Length;
		protected int _index;
		[System.Obsolete("don't",true)] public RingBuffer () {}
		public RingBuffer ( int length )
		{
			this._array = new T[ length ];
			this.Length = length;
			this._index = 0;
		}
		public void Push ( T value )
		{
			_array[ _index++ ] = value;
			if( _index==Length ) _index = 0;
		}
		public T[] AsArray () => _array;
	}
	
	internal class RingBufferInt16 : RingBuffer<Int16>
	{
		public RingBufferInt16 ( int length ) : base( length:length ) {}
		public bool Contains ( Int16 value )
		{
			bool result = false;
			for( int i=0 ; i<Length ; i++ )
				result |= _array[i]==value;
			// linear search isn't the best, but for 128 elements it takes less than 1/1000 ms (1e-6 s), so it's good enough for now
			return result;
		}
	}
	
}
