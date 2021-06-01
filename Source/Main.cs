using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace AllowDeadAnimals
{
	public class MyMapComponent : MapComponent
	{
		const int k_ticks_threshold = 1000;
		int _ticks = 0;

		/// <remarks> A Ring buffer to remember which corpses were allowed already so we don't do that again (in case player forbidden something again). </remarks>
		_RingBufferInt16 _allowedAlready = null;

		AllowDeadAnimalsModSettings _settings = null;

		public MyMapComponent ( Map map )
			: base(map)
		{
			_settings = LoadedModManager
				.GetMod<AllowDeadAnimalsMod>()
				.GetSettings<AllowDeadAnimalsModSettings>();
			_allowedAlready = new _RingBufferInt16( length:128 );
			
			// _allowedAlready, load saved buffer data:
			// {
			// 	var src = _settings.allowedAlreadyBufferState;
			// 	var dst = _allowedAlready.AsArray();
			// 	int len = Math.Min( dst.Length , src.Count );
			// 	for( int i=0 ; i<len ; i++ )
			// 		dst[i] = src[i];
			// }
		}

		public override void MapComponentTick ()
		{
			if( ++_ticks==k_ticks_threshold )
			{
				Task.Run( AllowFreshAnimalCorpses );
				_ticks = 0;
			}
		}

		/// <remarks> List is NOT thread-safe so EXPECT it can be changed by diffent CPU thread, mid-execution, anytime here.</remarks>
		void AllowFreshAnimalCorpses ()
		{
			var playerFaction = Faction.OfPlayer;
			var list = map.listerThings.ThingsInGroup( ThingRequestGroup.Corpse );
			for( int i=0 ; i<list.Count ; i++ )
			{
				Corpse corpse = (Corpse) list[i];
				if(
						corpse!=null
					&&  corpse.IsForbidden(playerFaction)
					&&  corpse.GetRotStage()==RotStage.Fresh
					&&  corpse.InnerPawn!=null
					&&  corpse.InnerPawn.RaceProps!=null
					&&  corpse.InnerPawn.RaceProps.Animal
				)
				{
					Int16 hash = (Int16) corpse.GetHashCode();
					if( !_allowedAlready.Contains(hash) )
					{
						if( _settings.allow )
							corpse.SetForbidden( false );
						_allowedAlready.Push( hash );

						if( _settings.notify )
							Messages.Message( text:"FreshCarrionSpotted".Translate((NamedArgument)corpse.LabelShort) , lookTargets:corpse , def:MessageTypeDefOf.NeutralEvent );
					}
				}
			}
		}

		// public override void ExposeData ()
		// {
		// 	var list = new List<short>( _allowedAlready.AsArray() );
		// 	Scribe_Collections.Look( ref list , "_allowedAlready._array" , LookMode.Reference );
		// 	base.ExposeData();
		// }

		class _RingBuffer <T>
		{
			protected readonly T[] _array;
			protected readonly int Length;
			protected int _index;
			[System.Obsolete("don't",true)] public _RingBuffer () {}
			public _RingBuffer ( int length )
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
		class _RingBufferInt16 : _RingBuffer<Int16>
		{
			public _RingBufferInt16 ( int length ) : base( length:length ) {}
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
}
