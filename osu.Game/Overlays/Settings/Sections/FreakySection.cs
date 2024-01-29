// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the FREAKY file in the repository root for full 𝓯𝓻𝓮𝓪𝓴𝔂 text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings.Sections.FreakySettings;

namespace osu.Game.Overlays.Settings.Sections
{
    public partial class FreakySection : SettingsSection
    {
        public override LocalisableString Header => "Freaky Mode";

        public override Drawable CreateIcon() => new SpriteIcon
        {
            Icon = FontAwesome.Solid.GrinTongue
        };

        public FreakySection()
        {
            Add(new TouchSettings());
        }
    }
}
