// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input;
using osu.Game.Localisation;
using osuTK;

namespace osu.Game.Overlays.Settings.Sections.Input
{
    public class MouseSettings : SettingsSubsection
    {
        private readonly MouseHandler mouseHandler;

        protected override LocalisableString Header => MouseSettingsStrings.Mouse;

        private Bindable<Vector2> handlerSensitivity;
        private Bindable<Vector2> localSensitivity;

        private BindableNumber<double> sensitivity = new BindableNumber<double>(1) { MinValue = 0.1, MaxValue = 10, Precision = 0.01 };
        private BindableNumber<double> sensitivityX = new BindableNumber<double>(1) { MinValue = 0.1, MaxValue = 10, Precision = 0.01 };
        private BindableNumber<double> sensitivityY = new BindableNumber<double>(1) { MinValue = 0.1, MaxValue = 10, Precision = 0.01 };

        private Bindable<WindowMode> windowMode;
        private SettingsEnumDropdown<OsuConfineMouseMode> confineMouseModeSetting;
        private Bindable<bool> relativeMode;

        private Bindable<bool> showAxisSensitivity;

        private SettingsCheckbox highPrecisionMouse;

        private SensitivitySetting sensitivitySetting;
        private SensitivitySetting sensitivityXSetting;
        private SensitivitySetting sensitivityYSetting;

        public MouseSettings(MouseHandler mouseHandler)
        {
            this.mouseHandler = mouseHandler;
        }

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager osuConfig, FrameworkConfigManager config)
        {
            // use local bindable to avoid changing enabled state of game host's bindable.
            handlerSensitivity = mouseHandler.AxisSensitivity.GetBoundCopy();
            localSensitivity = handlerSensitivity.GetUnboundCopy();

            relativeMode = mouseHandler.UseRelativeMode.GetBoundCopy();
            windowMode = config.GetBindable<WindowMode>(FrameworkSetting.WindowMode);
            showAxisSensitivity = new Bindable<bool>(false);//osuConfig.GetBindable<bool>(OsuSetting.SeparateAxisSensitivity);

            Children = new Drawable[]
            {
                highPrecisionMouse = new SettingsCheckbox
                {
                    LabelText = MouseSettingsStrings.HighPrecisionMouse,
                    TooltipText = MouseSettingsStrings.HighPrecisionMouseTooltip,
                    Current = relativeMode,
                    Keywords = new[] { @"raw", @"input", @"relative", @"cursor" }
                },
                new SettingsCheckbox
                {
                    LabelText = MouseSettingsStrings.SeparateAxisSensitivity,
                    TooltipText = MouseSettingsStrings.SeparateAxisSensitivityTooltip,
                    Current = showAxisSensitivity
                },
                sensitivitySetting = new SensitivitySetting
                {
                    LabelText = MouseSettingsStrings.CursorSensitivity,
                    Current = sensitivity
                },
                sensitivityXSetting = new SensitivitySetting
                {
                    LabelText = MouseSettingsStrings.CursorXSensitivity,
                    Current = sensitivityX
                },
                sensitivityYSetting = new SensitivitySetting
                {
                    LabelText = MouseSettingsStrings.CursorYSensitivity,
                    Current = sensitivityY
                },
                confineMouseModeSetting = new SettingsEnumDropdown<OsuConfineMouseMode>
                {
                    LabelText = MouseSettingsStrings.ConfineMouseMode,
                    Current = osuConfig.GetBindable<OsuConfineMouseMode>(OsuSetting.ConfineMouseMode)
                },
                new SettingsCheckbox
                {
                    LabelText = MouseSettingsStrings.DisableMouseWheelVolumeAdjust,
                    TooltipText = MouseSettingsStrings.DisableMouseWheelVolumeAdjustTooltip,
                    Current = osuConfig.GetBindable<bool>(OsuSetting.MouseDisableWheel)
                },
                new SettingsCheckbox
                {
                    LabelText = MouseSettingsStrings.DisableMouseButtons,
                    Current = osuConfig.GetBindable<bool>(OsuSetting.MouseDisableButtons)
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            relativeMode.BindValueChanged(relative => localSensitivity.Disabled = !relative.NewValue, true);

            showAxisSensitivity.BindValueChanged(showAxisSensitivity =>
            {
                if(!showAxisSensitivity.NewValue)
                {
                    sensitivitySetting.Show();
                    sensitivityXSetting.Hide();
                    sensitivityYSetting.Hide();
                }
                else
                {
                    sensitivitySetting.Hide();
                    sensitivityXSetting.Show();
                    sensitivityYSetting.Show();
                }
            }, true);

            handlerSensitivity.BindValueChanged(val =>
            {
                bool disabled = localSensitivity.Disabled;

                localSensitivity.Disabled = false;
                localSensitivity.Value = val.NewValue;
                localSensitivity.Disabled = disabled;
            }, true);

            localSensitivity.BindValueChanged(val => handlerSensitivity.Value = val.NewValue);

            sensitivity.BindValueChanged(val => localSensitivity.Value = new Vector2((float)val.NewValue));
            sensitivityX.BindValueChanged(val => localSensitivity.Value = new Vector2((float)val.NewValue, localSensitivity.Value.Y));
            sensitivityY.BindValueChanged(val => localSensitivity.Value = new Vector2(localSensitivity.Value.X, (float)val.NewValue));

            windowMode.BindValueChanged(mode =>
            {
                bool isFullscreen = mode.NewValue == WindowMode.Fullscreen;

                if (isFullscreen)
                {
                    confineMouseModeSetting.Current.Disabled = true;
                    confineMouseModeSetting.TooltipText = MouseSettingsStrings.NotApplicableFullscreen;
                }
                else
                {
                    confineMouseModeSetting.Current.Disabled = false;
                    confineMouseModeSetting.TooltipText = string.Empty;
                }
            }, true);

            highPrecisionMouse.Current.BindValueChanged(highPrecision =>
            {
                if (RuntimeInfo.OS != RuntimeInfo.Platform.Windows)
                {
                    if (highPrecision.NewValue)
                        highPrecisionMouse.SetNoticeText(MouseSettingsStrings.HighPrecisionPlatformWarning, true);
                    else
                        highPrecisionMouse.ClearNoticeText();
                }
            }, true);
        }

        public class SensitivitySetting : SettingsSlider<double, SensitivitySlider>
        {
            public SensitivitySetting()
            {
                KeyboardStep = 0.01f;
                TransferValueOnCommit = true;
            }
        }

        public class SensitivitySlider : OsuSliderBar<double>
        {
            public override LocalisableString TooltipText => Current.Disabled ? MouseSettingsStrings.EnableHighPrecisionForSensitivityAdjust : $"{base.TooltipText}x";
        }
    }
}
