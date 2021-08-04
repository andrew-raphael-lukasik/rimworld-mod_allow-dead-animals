using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace AllowDeadAnimals
{
	public class AllowDeadAnimalsMapComponent : MapComponent
	{
		const int k_ticks_threshold = 1000;
		int _ticks = 0;

		/// <remarks> A Ring buffer to remember which corpses were allowed already so we don't do that again (in case player forbidden something again). </remarks>
		RingBufferInt16 _allowedAlready = null;

		AllowDeadAnimalsModSettings _settings = null;

		public AllowDeadAnimalsMapComponent ( Map map )
			: base(map)
		{
			_settings = LoadedModManager
				.GetMod<AllowDeadAnimalsMod>()
				.GetSettings<AllowDeadAnimalsModSettings>();
			_allowedAlready = new RingBufferInt16( length:128 );
			
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
			float massThreshold = _settings.mass_threshold;
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
					&&	corpse.GetStatValue(StatDefOf.Mass) > massThreshold
				)
				{
					Int16 hash = (Int16)( corpse.GetHashCode() % Int16.MaxValue );
					if( !_allowedAlready.Contains(hash) )
					{ 
						_allowedAlready.Push( hash );

						if( _settings.allow )
							corpse.SetForbidden( false );

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

	}
}
