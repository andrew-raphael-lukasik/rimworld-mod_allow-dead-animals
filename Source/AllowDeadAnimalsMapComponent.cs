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
		TickManager _tickManager = null;

		public AllowDeadAnimalsMapComponent ( Map map )
			: base(map)
		{
			_settings = LoadedModManager
				.GetMod<AllowDeadAnimalsMod>()
				.GetSettings<AllowDeadAnimalsModSettings>();
			_allowedAlready = new RingBufferInt16( length:128 );
			_tickManager = Find.TickManager;
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
			int ticksGame = _tickManager.TicksGame;
			
			bool allow = _settings.allow;
			bool notify = _settings.notify;
			bool allowAnimal = _settings.allowAnimal;
			bool allowInsect = _settings.allowInsect;
			bool allowHumanlike = _settings.allowHumanlike;
			bool allowMechanoid = _settings.allowMechanoid;
			int raceFilter = (allowAnimal?1:0)<<0 | (allowInsect?1:0)<<1 | (allowHumanlike?1:0)<<2 | (allowMechanoid?1:0)<<3;
			// Messages.Message( $"filter:{Convert.ToString(raceFilter,2)}" , lookTargets:null , def:MessageTypeDefOf.NeutralEvent );

			var list = map.listerThings.ThingsInGroup( ThingRequestGroup.Corpse );
			for( int i=0 ; i<list.Count ; i++ )
			{
				if(
						(Corpse) list[i] is Corpse corpse
					&&	corpse.IsForbidden(playerFaction)

					// make sure it's fresh one:
					&&	corpse.GetRotStage()==RotStage.Fresh

					// make sure corpse is not too fresh (so fresh that is still being eaten by it's predator):
					&&	ticksGame-corpse.timeOfDeath > k_ticks_threshold*2

					&&	corpse.InnerPawn is var innerPawn
					&&	corpse.InnerPawn.RaceProps is var raceProps

					// filter by race
					&&	( ( (raceProps.Animal?1:0)<<0 | (raceProps.Insect?1:0)<<1 | (raceProps.Humanlike?1:0)<<2 | (raceProps.IsMechanoid?1:0)<<3 ) is int raceMask )
					&&	(raceMask&raceFilter)!=0// is there any matching category
					&&	( (0b10&raceMask)!=0b10 || (0b10&raceFilter)==0b10 )// makes sure insects are not categorized as animals

					// filter by mass threshold:
					&&	corpse.GetStatValue(StatDefOf.Mass) > massThreshold
				)
				{
					Int16 hash = (Int16)( corpse.thingIDNumber % Int16.MaxValue );
					if( !_allowedAlready.Contains(hash) )
					{
						_allowedAlready.Push( hash );

						if( allow )
							corpse.SetForbidden( false );

						if( notify )
							Messages.Message( text:"FreshCarrionSpotted".Translate((NamedArgument)corpse.LabelShort) , lookTargets:corpse , def:MessageTypeDefOf.NeutralEvent );
							// Messages.Message( text:$"{corpse.LabelShort} [ {(raceProps.Animal?"animal":"")}{(raceProps.Insect?" insect":"")}{(raceProps.Humanlike?" human":"")}{(raceProps.IsMechanoid?" mech":"")} ] MASK:{Convert.ToString(raceMask,2)} AND:{Convert.ToString(AND,2)}" , lookTargets:corpse , def:MessageTypeDefOf.NeutralEvent );
					}
				}
			}
		}

	}
}
