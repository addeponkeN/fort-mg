using Fort.MG.EntitySystem;
using Fort.MG.Gui.Components;
using Fort.Utility;
using Microsoft.Xna.Framework;

namespace Fort.MG.Systems;

public class SystemMessageSystem : EngineSystem, IFortDrawableGui
{
    public class SysMessage
    {
        public string DisplayText;
        public string Text;
        public float Timer;
        public bool IsError;
        public int Count = 1;
    }

    private List<SysMessage> _messages = new();
    private TextRenderer _textRenderer;

    public override void Start()
    {
        base.Start();
        _textRenderer = new();
    }

    public void AddMessage(string text, bool isError = false)
    {
        // Check for an existing identical message
        for (int i = 0; i < _messages.Count; i++)
        {
            var msg = _messages[i];
            if (msg.Text == text && msg.IsError == isError)
            {
                msg.Count++;
                msg.Timer = 5f;
                msg.DisplayText = $"{msg.Text} ({msg.Count})";
                return;
            }
        }

        var newMsg = PoolManager<SysMessage>.Spawn();
        newMsg.Timer = 5f;
        newMsg.Text = text;
        newMsg.DisplayText = text;
        newMsg.IsError = isError;
        newMsg.Count = 1;
        _messages.Add(newMsg);
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);

        float dt = t.Delta;

        for (int i = _messages.Count - 1; i >= 0; i--)
        {
            var msg = _messages[i];
            msg.Timer -= dt;
            if (msg.Timer <= 0)
            {
                _messages.RemoveAt(i);
                PoolManager<SysMessage>.Free(msg);
            }
        }
    }

    public void DrawGui()
    {
        var origin = new Vector2(Screen.Dimensions.X * 0.5f, Screen.Dimensions.Y * 0.2f);
        var lineHeight = 18f;

        for (int i = 0; i < _messages.Count; i++)
        {
            var msgIndex = _messages.Count - 1 - i;
            var message = _messages[msgIndex];

            _textRenderer.Text = msgIndex == _messages.Count - 1
                ? $"-- {message.DisplayText} --"
                : message.DisplayText;

            var size = _textRenderer.GetSize();
            _textRenderer.Color = message.IsError ? Color.Red : Color.WhiteSmoke;

            _textRenderer.Position = new Vector2(
                origin.X - size.X / 2f,
                origin.Y - (i * lineHeight)
            );

            _textRenderer.DrawText();
        }
    }
}
