// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the FREAKY file in the repository root for full 𝓯𝓻𝓮𝓪𝓴𝔂 text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace osu.Game.Overlays.Settings.Sections.FreakySettings
{
    public partial class TouchSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "";

        [Resolved]
        private GameHost host { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new SettingsButton
                {
                    Text = "we gon touch you Lil bro...",
                    Action = () => host.OpenUrlExternally("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcREFCCjFPVMlsKeG4yXcouls6br8IsetevXkBfbrVc2Yw&s")
                }
            };
        }
    }
}
