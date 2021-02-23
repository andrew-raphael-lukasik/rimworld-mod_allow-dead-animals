using System.Threading.Tasks;
// using UnityEngine;
using Verse;
using RimWorld;

namespace AllowDeadAnimals
{
    public class MyMapComponent : MapComponent
    {

        const int k_ticks_threshold = 1000;
        int _ticks = 0;

        public MyMapComponent ( Map map ) : base(map) {}

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
                Thing thing = list[i];
                Corpse corpse = (Corpse) thing;
                if(
                        corpse!=null
                    &&  corpse.IsForbidden(playerFaction)
                    &&  corpse.GetRotStage()==RotStage.Fresh
                    &&  corpse.InnerPawn!=null
                    &&  corpse.InnerPawn.RaceProps!=null
                    &&  corpse.InnerPawn.RaceProps.Animal
                )
                {
                    corpse.SetForbidden( false );
                    Messages.Message( text:"FreshCarrionSpotted".Translate((NamedArgument)corpse.LabelShort) , lookTargets:thing , def:MessageTypeDefOf.NeutralEvent );
                }
            }
        }

    }
}
