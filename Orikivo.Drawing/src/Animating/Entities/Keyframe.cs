namespace Orikivo.Drawing.Animating
{
    public struct Keyframe
    {
        public static Keyframe GetDefault(long tick)
            => new Keyframe(tick);

        public Keyframe(long tick, float opacity = 1.0f, ImageTransform transform = null)
        {
            Tick = tick;
            Opacity = opacity;
            _transform = transform;
        }

        public Keyframe(long tick, float opacity, Vector2 position, AngleF rotation, Vector2 scale)
        {
            Tick = tick;
            Opacity = opacity;
            _transform = new ImageTransform(position, rotation, scale);
        }

        public long Tick { get; }

        private ImageTransform _transform;

        public ImageTransform Transform
        {
            get
            {
                if (_transform is null)
                    _transform = ImageTransform.Default;

                return _transform;
            }
            set => _transform = value;
        }

        public float Opacity { get; set; }

        public Vector2 Position
        {
            get => Transform.Position;
            set => Transform.Position = value;
        }

        public AngleF Rotation
        {
            get => Transform.Rotation;
            set => Transform.Rotation = value;
        }

        public Vector2 Scale
        {
            get => Transform.Scale;
            set => Transform.Scale = value;
        }
    }
}
