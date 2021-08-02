using UnityEngine;
using Verse;
using RimWorld;

namespace AllowDeadAnimals
{
	public class AllowDeadAnimalsMod : Mod
	{

		AllowDeadAnimalsModSettings _settings;

		// string _string_ticksBetweenUpdates = "1001";

		public AllowDeadAnimalsMod ( ModContentPack content )
			: base( content )
			=> this._settings = GetSettings<AllowDeadAnimalsModSettings>();

		public override string SettingsCategory () => "Allow Dead Animals";

		public override void DoSettingsWindowContents ( Rect rect )
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(rect);
			{
				// listing.GapLine();
				// listing.Label( "Basics" );
				// listing.GapLine();

				listing.CheckboxLabeled( "Allow" , ref _settings.allow , "Is auto-allow enabled?" );
				
				listing.CheckboxLabeled( "Notify" , ref _settings.notify , "Are notifications enabled?" );

				string mass_threshold_label =_settings.mass_threshold.ToString();
				listing.TextFieldNumericLabeled( "Mass Threshold" , ref _settings.mass_threshold , ref mass_threshold_label );
				// "Corpses below this mass will be ignored."
				
				// listing.GapLine();
				// listing.Label( "Advanced" );
				// listing.GapLine();

				// listing.Label( "Number of ticks between updates" );
				// listing.IntEntry( ref _settings.ticksBetweenUpdates , ref _string_ticksBetweenUpdates , 100 );

				// listing.Label( "_allowedAlready.length" , -1 , "Size of a buffer that stores which corpses were allowed already. Increase ONLY if it doesn't work due to number of allowed corpses being > 128 (rare case but possible). Restart to take effect." );
				// _settings.allowedAlreadyBufferLength = (int) listing.Slider( _settings.allowedAlreadyBufferLength , 128 , 1024 );

				// listing.GapLine();
			}
			listing.End();
			base.DoSettingsWindowContents(rect);
		}

	}
}
