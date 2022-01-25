using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Globalization;
using TTT.Items;

namespace TTT.UI
{
    public class Effect : Panel
    {
        public IItem Item
        {
            get
            {
                return _item;
            }
            private set
            {
                _item = value;

                _nameLabel.UpdateTranslation(new TranslationData(_item?.LibraryName.ToUpper() ?? ""));
                _effectImage.Texture = _item != null ? Texture.Load(FileSystem.Mounted, $"/ui/weapons/{_item.LibraryName}.png", false) : null;

                if (_effectImage.Texture == null)
                {
                    _effectImage.Texture = Texture.Load(FileSystem.Mounted, $"/ui/none.png");
                }

                _label = _effectIconPanel.Add.Label();
                _label.AddClass("countdown");
                _label.AddClass("centered");
                _label.AddClass("text-shadow");
            }
        }

        private IItem _item;
        private readonly TranslationLabel _nameLabel;
        private readonly Panel _effectIconPanel;
        private readonly Image _effectImage;
        private Label _label;

        public Effect(Panel parent, IItem effect) : base(parent)
        {
            Parent = parent;

            AddClass("text-shadow");

            _effectIconPanel = new Panel(this);
            _effectIconPanel.AddClass("effect-icon-panel");

            _effectImage = _effectIconPanel.Add.Image();
            _effectImage.AddClass("effect-image");

            _nameLabel = Add.TranslationLabel(new TranslationData());
            _nameLabel.AddClass("name-label");

            Item = effect;
        }

        public override void Tick()
        {
            base.Tick();

            if (_label != null)
            {
                if (Item is TTTCountdownPerk countdownPerk)
                {
                    int currentCountdown = (countdownPerk.Countdown - countdownPerk.LastCountdown).CeilToInt();

                    if (currentCountdown == countdownPerk.Countdown.CeilToInt() || currentCountdown == 0)
                    {
                        _effectImage.SetClass("cooldown", false);
                        _label.Text = "";
                    }
                    else
                    {
                        _effectImage.SetClass("cooldown", true);
                        _label.Text = $"{currentCountdown:n0}";
                    }
                }
                else if (Item is TTTBoolPerk boolPerk)
                {
                    _label.Text = boolPerk.IsEnabled ? "ON" : "OFF";
                }
            }
        }
    }
}
