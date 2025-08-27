// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Manager;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Graphics.UserInterface;
using osu.Game.Localisation;
using osu.Game.Overlays.Dialog;

namespace osu.Game.Overlays.Settings.Sections.Audio
{
    public partial class AudioDevicesSettings : SettingsSubsection
    {
        protected override LocalisableString Header => AudioSettingsStrings.AudioDevicesHeader;

        [Resolved]
        private AudioManager audio { get; set; }

        private SettingsDropdown<AudioBackend> backend;
        private SettingsDropdown<string> device;

        private bool automaticBackendInUse;

        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager config, OsuGame game, IDialogOverlay dialogOverlay)
        {
            var currentBackend = config.GetBindable<AudioBackend>(FrameworkSetting.AudioBackend);
            automaticBackendInUse = currentBackend.Value == AudioBackend.Automatic;

            Children = new Drawable[]
            {
                backend = new SettingsEnumDropdown<AudioBackend>
                {
                    LabelText = AudioSettingsStrings.AudioBackend,
                    Keywords = new[] { "backend", "bass", "wasapi" },
                    Current = currentBackend,
                    Items = game.GetPreferredAudioBackendsForCurrentPlatform(),
                },
                device = new AudioDeviceSettingsDropdown
                {
                    LabelText = AudioSettingsStrings.OutputDevice,
                    Keywords = new[] { "speaker", "headphone", "output" },
                },
            };

            currentBackend.BindValueChanged(r =>
            {
                if (r.NewValue == game.ResolvedAudioBackend)
                    return;

                if (r.NewValue == AudioBackend.Automatic && automaticBackendInUse)
                    return;

                if (game?.RestartAppWhenExited() == true)
                {
                    game.AttemptExit();
                }
                else
                {
                    dialogOverlay?.Push(new ConfirmDialog(AudioSettingsStrings.ChangeAudioBackendConfirmation, () => game?.AttemptExit(), () =>
                    {
                        currentBackend.Value = automaticBackendInUse ? AudioBackend.Automatic : game?.ResolvedAudioBackend ?? AudioBackend.Automatic;
                    }));
                }
            });

            updateItems();

            audio.OnNewDevice += onDeviceChanged;
            audio.OnLostDevice += onDeviceChanged;
            device.Current = audio.AudioDevice;
        }

        private void onDeviceChanged(KeyValuePair<string, string> _) => updateItems();

        private void updateItems()
        {
            var deviceItems = new List<string> { string.Empty };
            deviceItems.AddRange(audio.AudioDevices.Keys);

            string preferredDeviceName = audio.AudioDevice.Value;
            if (deviceItems.All(kv => kv != preferredDeviceName))
                deviceItems.Add(preferredDeviceName);

            device.Items = deviceItems.ToList();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (audio != null)
            {
                audio.OnNewDevice -= onDeviceChanged;
                audio.OnLostDevice -= onDeviceChanged;
            }
        }

        private partial class AudioDeviceSettingsDropdown : SettingsDropdown<string>
        {
            protected override OsuDropdown<string> CreateDropdown() => new AudioDeviceDropdownControl();

            private partial class AudioDeviceDropdownControl : DropdownControl
            {
                [Resolved]
                private AudioManager audio { get; set; }

                protected override LocalisableString GenerateItemText(string item)
                    => string.IsNullOrEmpty(item)
                        ? CommonStrings.Default
                        : audio is AudioManager manager && manager.AudioDevices.TryGetValue(item, out string value)
                            ? value
                            : item;
            }
        }
    }
}
